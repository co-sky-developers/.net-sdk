using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using NFleet.Data;
using RestSharp;
using RestSharp.Contrib;

namespace NFleet
{
    public sealed class Api
    {
        private readonly Dictionary<string, object> cache = new Dictionary<string, object>();

        private const string versionNumberHeader = "If-None-Match";

        private readonly RestClient client;
        private readonly string username;
        private readonly string password;

        private TokenData currentToken;

        public ApiData Root { get { return Navigate<ApiData>( new Link { Method = "GET", Uri = "", Type = "application/json" } ); } }

        public Api( string url, string username, string password )
        {
            if ( String.IsNullOrEmpty( url ) ) throw new ArgumentException( "Missing value", "url" );
            if ( String.IsNullOrEmpty( username ) ) throw new ArgumentException( "Missing value", "username" );
            if ( String.IsNullOrEmpty( password ) ) throw new ArgumentException( "Missing value", "password" );

            client = new RestClient( url ) { FollowRedirects = false };

#if DEBUG
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
            client.ClearHandlers();
            client.AddHandler( link.Type, new CustomConverter() );
            var request = InitializeRequest( link, queryParameters );
            if (link.Method == "GET") request.AddHeader( "Accept", TypeHelper.GetSupportedType( link.Type ) );

            request.AddHeader( "Content-Type", TypeHelper.GetSupportedType( link.Type ) );

            InsertIfNoneMatchHeader( ref request, data, cache, queryParameters );
            InsertAuthorizationHeader( ref request, currentToken );

            // when POSTing, if data is null, add an empty object to prevent error due to null payload
            if ( data == null )
            {
                var b = ( link.Method == "POST" || link.Method == "DELETE" ) ? new Empty() : data;
                request.AddParameter( link.Type, JsonConvert.SerializeObject( b ), ParameterType.RequestBody );
            }
            else
            {
                request.AddParameter( link.Type, JsonConvert.SerializeObject( data ), ParameterType.RequestBody );
            }
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
                    ThrowException( result );
            }

            if ( result.StatusCode != HttpStatusCode.NoContent && result.StatusCode == 0 )
                throw new IOException( string.Format( "Could not connect to server at {0}.", client.BaseUrl ) );

            if ( ( result.Content.Length > 0 && result.ResponseStatus != ResponseStatus.Completed ) )
            {
                var converter = new CustomConverter();
                var res = converter.Deserialize<T>( result );
                if ( res != null ) result.Data = res;
                else throw new IOException( result.ErrorMessage );
            }
                

            var code = result.StatusCode;

            if ( code >= HttpStatusCode.BadRequest )
            {
                ThrowException( result );
            }

            if ( code == HttpStatusCode.OK )
            {
                var etag = result.Headers.FirstOrDefault( h => h.Name == "ETag" );
                var version = etag != null ? Int32.Parse( etag.Value.ToString() ) : 0;

                var obj = result.Data as IVersioned;
                if ( obj != null ) obj.VersionNumber = version;
            }

            if ( result.StatusCode == HttpStatusCode.NotModified )
            {
                var key = BuildCacheKey( request.Resource, queryParameters );
                return (T)cache[key];
            }

            if ( code == HttpStatusCode.Created || code == HttpStatusCode.SeeOther )
            {
                var parameter = result.Headers.FirstOrDefault( h => h.Name == "Location" );
                var contentType = result.Headers.FirstOrDefault( h => h.Name == "Content-Type" );
                if ( parameter == null || parameter.Value == null )
                    throw new IOException( "Server response missing Location header." );

                if ( contentType == null || contentType.Value == null )
                    throw new IOException( "Server response missing Content-Type header." );

                var value = parameter.Value.ToString();
                var entityLocation = new Uri( value ).AbsolutePath;

                var responseData = new ResponseData();

                responseData.Meta.Add( new Link { Method = "GET", Rel = "location", Uri = entityLocation, Type = contentType.Value.ToString() } );

                if ( !( responseData is T ) )
                    throw new InvalidOperationException( string.Format( "The response from the given URL is not of the requested type {0}.", typeof( T ).Name ) );

                return (T)(IResponseData)responseData;
            }

            if ( !Equals( result.Data, default( T ) ) && !String.IsNullOrEmpty( request.Resource ) )
            {
                var key = BuildCacheKey( request.Resource, queryParameters );
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

        private static string BuildQuery( Dictionary<string, string> queryParameters )
        {
            var sb = new StringBuilder( "?" );
            var lst = new List<String>();
            foreach ( var queryParameter in queryParameters )
            {
                lst.Add( String.Format( "{0}={1}", queryParameter.Key, queryParameter.Value ) );
            }
            var arr = lst.ToArray();
            sb.Append( string.Join( "&", arr ) );

            return sb.ToString();
        }

        private static string BuildCacheKey( string resource, Dictionary<string, string> queryParameters )
        {
            var key = resource;
            if ( queryParameters != null && queryParameters.Count > 0 )
            {
                key += BuildQuery( queryParameters );
            }

            return key;
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
                JsonSerializer = new CustomConverter { ContentType = link.Type ?? "application/json" }
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

        private static void InsertIfNoneMatchHeader( ref RestRequest request, object data, Dictionary<string, object> cache, Dictionary<string, string> queryParameters )
        {
            var key = BuildCacheKey( request.Resource, queryParameters );

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

            var errors = result.Data is ResponseData
                ? ( (ResponseData)(IResponseData)result.Data ).Items
                : new List<ErrorData>();

            throw new NFleetRequestException( String.Format( "{0} {1} from {2}", (int)code, result.StatusDescription, result.ResponseUri ) ) { Items = errors };
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

                authLocation = new Uri( locationParameter.Value.ToString() ).AbsolutePath;
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

            return new Uri( tokenLocationParameter.Value.ToString() ).AbsolutePath;
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

