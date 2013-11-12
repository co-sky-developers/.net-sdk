using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NFleet.Data;
using RestSharp;
using RestSharp.Contrib;
using RestSharp.Deserializers;

namespace NFleet
{
    public sealed class Api
    {
        private readonly Dictionary<string, object> cache = new Dictionary<string, object>();

        private const string separator = "://";
        private const string versionNumberHeader = "If-None-Match";

        private readonly RestClient client;
        private readonly string baseUrl;
        private readonly string username;
        private readonly string password;

        private TokenData currentToken;

        public ApiData Root { get { return Navigate<ApiData>( new Link { Method = "GET", Uri = "" } ); } }

        public Api( string url, string username, string password )
        {
            if ( String.IsNullOrEmpty( url ) ) throw new ArgumentException( "Missing value", "url" );
            if ( String.IsNullOrEmpty( username ) ) throw new ArgumentException( "Missing value", "username" );
            if ( String.IsNullOrEmpty( password ) ) throw new ArgumentException( "Missing value", "password" );

            baseUrl = url.Remove( 0, url.IndexOf( separator, StringComparison.Ordinal ) + separator.Length );

            client = new RestClient( url ) { FollowRedirects = false };

#if DEBUG
            baseUrl = baseUrl.Replace( "81", "82" ); // Azure emulator hack
            client.Timeout = Int32.MaxValue;
#endif
            this.username = username;
            this.password = password;
        }


       public ResponseData Navigate( Link link, object data = null, Dictionary<string, string> queryParameters = null )
        {
            return Navigate<ResponseData>( link, data, queryParameters );
        }

        public T Navigate<T>( Link link, Dictionary<string, string> queryParameters ) where T : IResponseData, new()
        {
            return Navigate<T>( link, null, queryParameters );
        }

        public T Navigate<T>( Link link, object data = null, Dictionary<string, string> queryParameters = null ) where T : IResponseData, new()
        {
            if ( link == null )
                throw new ArgumentNullException( "link" );

            var request = InitializeRequest( link, queryParameters );

            InsertIfNoneMatchHeader( ref request, data, cache, queryParameters );
            InsertAuthorizationHeader( ref request, currentToken );

            // when POSTing, if data is null, add an empty object to prevent 500 Internal Server Error due to null payload
            request.AddBody( data == null && link.Method == "POST" ? new Empty() : data );
            request.OnBeforeDeserialization = resp => resp.ContentType = "application/json";
            var result = client.Execute<T>( request );

            if ( result.StatusCode == HttpStatusCode.Unauthorized )
            {
                Authenticate( username, password );
                request = InitializeRequest( link, queryParameters );
                InsertIfNoneMatchHeader( ref request, data, cache, queryParameters );
                InsertAuthorizationHeader( ref request, currentToken );

                result = client.Execute<T>( request );
                if ( result.StatusCode == HttpStatusCode.Unauthorized )
                    throw new IOException( "Invalid client key or client secret." );
            }

            if ( ( result.Content.Length > 0 && result.ResponseStatus != ResponseStatus.Completed ) || result.StatusCode == 0 )
                throw new IOException( result.ErrorMessage );

            var code = result.StatusCode;

            if ( code >= HttpStatusCode.BadRequest )
            {
                ThrowException( result );
            }

            if ( code == HttpStatusCode.OK )
            {
                var etag = result.Headers.FirstOrDefault( h => h.Name == "ETag" );
                var version = etag != null ? Int32.Parse( etag.Value.ToString() ) : 0;
                result.Data.VersionNumber = version;
            }

            if ( result.StatusCode == HttpStatusCode.NotModified )
            {
                var key = request.Resource;
                if ( queryParameters != null && queryParameters.Count > 0 )
                {
                    key += BuildQuery( queryParameters );
                }
                return (T)cache[key];
            }

            if ( code == HttpStatusCode.Created || code == HttpStatusCode.SeeOther )
            {
                var parameter = result.Headers.FirstOrDefault( h => h.Name == "Location" );

                if ( parameter == null || parameter.Value == null )
                    throw new IOException( "Server response missing Location header." );

                var value = parameter.Value.ToString();
                var entityLocation = value.Remove( 0, value.IndexOf( baseUrl, StringComparison.Ordinal ) + baseUrl.Length );

                var responseData = new ResponseData();
                responseData.Meta.Add( new Link { Method = "GET", Rel = "location", Uri = entityLocation } );

                if ( !( responseData is T ) )
                    throw new InvalidOperationException( string.Format( "The response from the given URL is not of the requested type {0}.", typeof( T ).Name ) );

                return (T)(IResponseData)responseData;
            }

            if ( !Equals( result.Data, default( T ) ) && !String.IsNullOrEmpty( request.Resource ) )
            {
                var key = request.Resource;
                if (queryParameters != null && queryParameters.Count > 0)
                {
                    key += BuildQuery(queryParameters);
                }
                cache[key] = result.Data;
            }


            return result.Data;
        }

        public TokenData Authenticate()
        {
            Authenticate( username, password );
            return currentToken;
        }

        public TokenData Authorize( TokenData token )
        {
            if ( token == null )
                throw new ArgumentNullException( "token" );

            currentToken = token;
            return currentToken;
        }

        private static string BuildQuery(Dictionary<string, string> queryParameters )
        {
            var sb = new StringBuilder("?");
            var lst = new List<String>();
            foreach (var queryParameter in queryParameters)
            {
                lst.Add( String.Format("{0}={1}", queryParameter.Key, queryParameter.Value) );
            }
            var arr = lst.ToArray();
            sb.Append(string.Join("&",arr) );

            return sb.ToString();
        }

        private RestRequest InitializeRequest( Link link, Dictionary<string, string> queryParameters )
        {
            var uri = link.Uri;
            if ( uri.Contains( "?" ) )
            {
                queryParameters = ParseQueryParameters( client.BaseUrl + uri );
                uri = uri.Substring( 0, uri.IndexOf( '?' ) );
            }

            var request = new RestRequest( uri, link.Method.ToMethod() )
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new CustomConverter { ContentType = "application/json" }
            };

            if ( link.Method == "GET" && queryParameters != null )
            {
                foreach ( var queryParameter in queryParameters )
                {
                    request.AddParameter( queryParameter.Key, queryParameter.Value );
                }
            }

            return request;
        }

        private static void InsertAuthorizationHeader( ref RestRequest request, TokenData token )
        {
            if ( token != null )
                request.AddHeader( "Authorization", token.TokenType + " " + token.AccessToken );
        }

        private static void InsertIfNoneMatchHeader( ref RestRequest request, object data, Dictionary<string, object> cache, Dictionary<string,string> queryParameters  )
        {
            var key = request.Resource;
            if ( queryParameters != null && queryParameters.Count > 0 )
            {
                key += BuildQuery( queryParameters );
            }
            if ( data != null && ( data as IVersioned ) != null )
            {
                var d = data as IVersioned;
                request.AddHeader( versionNumberHeader, d.VersionNumber.ToString( CultureInfo.InvariantCulture ) );
            }
            else if ( cache.ContainsKey( key ) && cache[key] is IVersioned )
            {
                var d = cache[key] as IVersioned;
                request.AddHeader( versionNumberHeader, d.VersionNumber.ToString( CultureInfo.InvariantCulture ) );
            }
        }

        private static void ThrowException<T>( IRestResponse<T> result ) where T : new()
        {
            var code = result.StatusCode;

            var errorData = result.Data is ResponseData ? ( (ResponseData)(IResponseData)result.Data ).Items.FirstOrDefault() : null;
            throw new IOException( String.Format( "{0} {1}{2}", (int)code, result.StatusDescription, errorData != null ? ": " + errorData.Message : "" ) );
        }

        private void Authenticate( string key, string secret )
        {
            //  make an unsuccessful request to root to obtain authentication url
            var authenticationUrl = GetAuthLocation();

            // if we did not receive location, we do not have to (or cannot) authorize
            if ( authenticationUrl == null ) return;

            // post to the authentication url to create a token
            var tokenLocation = Authenticate( key, secret, authenticationUrl );

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

        private string Authenticate( string key, string secret, string authLocation )
        {
            var authorizationRequest = new RestRequest( authLocation, Method.POST ) { RequestFormat = DataFormat.Json };
            authorizationRequest.AddHeader( "Authorization", "Basic " + Convert.ToBase64String( Encoding.UTF8.GetBytes( String.Format( "{0}:{1}", key, secret ) ) ) );
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
            var myUri = new Uri( url );
            var parameters = HttpUtility.ParseQueryString( myUri.Query );
            return parameters.AllKeys.ToDictionary( key => key, key => parameters[key] );
        }
    }

    internal class Empty
    {
    }
    
}

