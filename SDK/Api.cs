﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NFleetSDK.Data;
using RestSharp;
using RestSharp.Contrib;
using RestSharp.Deserializers;


namespace NFleetSDK
{
    public sealed class Api
    {
        private const string separator = "://";

        private readonly RestClient client;
        private readonly string baseUrl;

        private TokenData currentToken;

        private string username;
        private string password;
        private int counter = 0;

        public Api( string url, string username, string password )
        {
            baseUrl = url.Remove( 0, url.IndexOf( separator, StringComparison.Ordinal ) + separator.Length );
            baseUrl = baseUrl.Replace( "81", "82" );
            client = new RestClient( url ) { FollowRedirects = false };
            client.AddHandler("application/json", new JsonDeserializer()
            {
                DateFormat =
                    "yyyy-MM-ddTHH:mm:ss.fffffffzzz"
            });
#if DEBUG
            client.Timeout = Int32.MaxValue;

#endif
            this.username = username;
            this.password = password;
        }

        public T Navigate<T>( Link link, Dictionary<string, string> queryParameters ) where T : IResponseData, new()
        {
            return Navigate<T>(link, null, queryParameters);
        }

        public T Navigate<T>( Link link, object data = null, Dictionary<string,string> queryParameters = null ) where T : IResponseData, new()
        {
            var uri = link.Uri;
            if ( uri.Contains( "?" ) )
            {
                queryParameters = ParseQueryParameters( client.BaseUrl + uri );
                uri = uri.Substring( 0, uri.IndexOf( '?' ) );
            }


            var request = new RestRequest( uri, link.Method.ToMethod() ) { RequestFormat = DataFormat.Json };

            if ( link.Method == "POST" && link.Rel == "authenticate" && data == null )
            {
                Authenticate( username, password );
                return (T)(IResponseData)currentToken;
            }

            if ( currentToken != null )
                request.AddHeader( "Authorization", currentToken.TokenType + " " + currentToken.AccessToken );

            // when POSTing, if data is null, add an empty object to prevent 500 Internal Server Error due to null payload
            request.AddBody( data == null && link.Method == "POST" ? new Empty() : data );

            if (link.Method == "GET" && queryParameters != null)
            {
                foreach (var queryParameter in queryParameters)
                {
                    request.AddParameter(queryParameter.Key, queryParameter.Value);
                }
            }
                
            var result = client.Execute<T>( request );

            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                Authenticate(username, password);
                request = new RestRequest( link.Uri, link.Method.ToMethod() ) { RequestFormat = DataFormat.Json };

                if ( link.Method == "GET" && queryParameters != null )
                {
                    foreach ( var queryParameter in queryParameters )
                    {
                        request.AddParameter( queryParameter.Key, queryParameter.Value );
                    }
                }

                if ( currentToken != null )
                    request.AddHeader( "Authorization", currentToken.TokenType + " " + currentToken.AccessToken );
                    
                result = client.Execute<T>( request );
                if ( result.StatusCode == HttpStatusCode.Unauthorized )
                    throw new IOException( "Invalid username or password." );
            }

            if ( ( result.Content.Length > 0 && result.ResponseStatus != ResponseStatus.Completed ) || result.StatusCode == 0 )
                throw new IOException( result.ErrorMessage );
            var code = result.StatusCode;
            if ( code >= HttpStatusCode.BadRequest )
            {
                ThrowException( result );
            }
            if ( code == HttpStatusCode.Created || code == HttpStatusCode.SeeOther )
            {
                var parameter = result.Headers.FirstOrDefault( h => h.Name == "Location" );

                if ( parameter == null || parameter.Value == null ) throw new IOException( "Server response missing Location header." );    

                var value = parameter.Value.ToString();
                var entityLocation = value.Remove( 0, value.IndexOf( baseUrl, StringComparison.Ordinal ) + baseUrl.Length );

                var responseData = new ResponseData();
                responseData.Meta.Add( new Link { Method = "GET", Rel = "location", Uri = entityLocation } );
                return (T)(IResponseData)responseData;
            }
            return result.Data;
        }

        public TokenData Authorize( TokenData token )
        {
            currentToken = token;
            
            return currentToken;
        }

        public TokenData GetCurrent( TokenData token )
        {
            return currentToken;
        }

        private static void ThrowException<T>( IRestResponse<T> result ) where T : new()
        {
            var code = result.StatusCode;

            var errorData = result.Data is ResponseData ? ( (ResponseData)(IResponseData)result.Data ).Items.FirstOrDefault() : null;
            throw new IOException( String.Format( "{0} {1}{2}", (int)code, result.StatusDescription, errorData != null ? ": " + errorData.Message : "" ) );
        }

        private void Authenticate( string uname, string passw )
        {
            //  make an unsuccessful request to root to obtain authentication url
            var authenticationUrl = GetAuthLocation();

            // if we did not receive location, we do not have to (or cannot) authorize
            if ( authenticationUrl == null ) return;

            // post to the authentication url to create a token
            var tokenLocation = Authenticate( uname, passw, authenticationUrl );

            // request and store the token
            currentToken = RequestToken( tokenLocation );
        }                                                                                                                                                                                                                                                                                                                           

        private string GetAuthLocation()
        {
            var result = client.Execute<ApiData>( new RestRequest( "", Method.GET ) );

            var code = result.StatusCode;
            string authLocation = null;

            if ( code == HttpStatusCode.Unauthorized )
            {
                var locationParameter = result.Headers.FirstOrDefault( h => h.Name == "Location" );

                if ( locationParameter == null || locationParameter.Value == null )
                    throw new IOException( "Server response missing Location header." );

                var locationValue = locationParameter.Value.ToString();
                authLocation = locationValue.Remove( 0, locationValue.IndexOf( baseUrl, StringComparison.Ordinal ) + baseUrl.Length );
            }
            else if ( code >= HttpStatusCode.BadRequest )
            {
                throw new IOException( String.Format( "{0} {1}", (int)code, result.StatusDescription ) );
            }

            return authLocation;
        }

        private string Authenticate( string username, string password, string authLocation )
        {                                                            
            var authorizationRequest = new RestRequest( authLocation, Method.POST ) { RequestFormat = DataFormat.Json };
            authorizationRequest.AddHeader( "Authorization", "Basic " + Convert.ToBase64String( Encoding.UTF8.GetBytes( String.Format( "{0}:{1}", username, password ) ) ) );
            authorizationRequest.AddBody( new AuthenticationRequest { Scope = "data optimization" } );

            var response = client.Execute<AuthenticationData>( authorizationRequest );

            if ( response.StatusCode < HttpStatusCode.BadRequest && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.SeeOther )
                throw new IOException( "Unexpected response from server." );

            if ( response.StatusCode >= HttpStatusCode.BadRequest )
                ThrowException( response );

            var tokenLocationParameter = response.Headers.FirstOrDefault( h => h.Name == "Location" );

            if ( tokenLocationParameter == null || tokenLocationParameter.Value == null )
                throw new IOException( "Server response missing Location header." );

            var tokenLocationValue = tokenLocationParameter.Value.ToString();
            return tokenLocationValue.Remove( 0, tokenLocationValue.IndexOf( baseUrl, StringComparison.Ordinal ) + baseUrl.Length );
        }

        private TokenData RequestToken( string tokenLocation )
        {
            var tokenRequest = new RestRequest( tokenLocation, Method.GET ) { RequestFormat = DataFormat.Json };
            var tokenResponse = client.Execute<TokenData>( tokenRequest );
            if ( tokenResponse.ResponseStatus != ResponseStatus.Completed )
            {
                throw new IOException( tokenResponse.ErrorMessage );
            }
            if ( tokenResponse.StatusCode != HttpStatusCode.OK )
            {
                throw new IOException( "Error receiving authorization token." );
            }

            return tokenResponse.Data;
        }

        private Dictionary<string, string> ParseQueryParameters( string url )
        {
            var queryParams = new Dictionary<string, string>();


            Uri myUri = new Uri( url );
            var parameters = HttpUtility.ParseQueryString( myUri.Query );

            foreach ( var key in parameters.AllKeys )
            {
                queryParams.Add( key, parameters[key] );
            }

            return queryParams;
        }

        public ApiData Root
        {
            get { return Navigate<ApiData>( new Link { Method = "GET", Uri = "" } ); }
        }

        public TokenData Authenticate()
        {
            Authenticate( username, password );
            return currentToken;
        }
    }

    internal class Empty
    {
    }
}