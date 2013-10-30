using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NFleetSDK.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using RestSharp.Contrib;
using RestSharp.Deserializers;
using RestSharp.Serializers;


namespace NFleetSDK
{
    public sealed class Api
    {
        private Dictionary<string, object> CACHE = new Dictionary<string, object>();
 
        private const string separator = "://";
        private const string VersionNumberHeader = "If-None-Match";

        private readonly RestClient client;
        private readonly string baseUrl;

        private TokenData currentToken;

        private string username;
        private string password;

        public Api( string url, string username, string password )
        {
            baseUrl = url.Remove( 0, url.IndexOf( separator, StringComparison.Ordinal ) + separator.Length );
            baseUrl = baseUrl.Replace( "81", "82" );
            client = new RestClient( url ) { FollowRedirects = false };

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
            var request = InitializeRequest( link, queryParameters );

            InsertIfNoneMatchHeader(ref request, data, CACHE);
            InsertAuthorizationHeader( ref request, currentToken );

            // when POSTing, if data is null, add an empty object to prevent 500 Internal Server Error due to null payload
            request.AddBody( data == null && link.Method == "POST" ? new Empty() : data );
            
            var result = client.Execute<T>( request );

            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                Authenticate(username, password);
                request = InitializeRequest( link, queryParameters );
                InsertIfNoneMatchHeader( ref request, data, CACHE );
                InsertAuthorizationHeader(ref request, currentToken);
                    
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

            if ( result.Data is IVersioned )
            {
                var etag = result.Headers.FirstOrDefault( h => h.Name == "ETag" );
                var version = etag != null ? Int32.Parse( etag.Value.ToString() ) : 0;
                result.Data.VersionNumber = version;
            }

            if ( result.StatusCode == HttpStatusCode.NotModified )
            {
                var res = (T)CACHE[request.Resource];
                Console.WriteLine( "Returned entity from cache: {0}", request.Resource );
                return res;
            }

            if ( code == HttpStatusCode.Created || code == HttpStatusCode.SeeOther )
            {
                var parameter = result.Headers.FirstOrDefault( h => h.Name == "Location" );

                if ( parameter == null || parameter.Value == null ) throw new IOException( "Server response missing Location header." );    

                var value = parameter.Value.ToString();
                var entityLocation = value.Remove( 0, value.IndexOf( baseUrl, StringComparison.Ordinal ) + baseUrl.Length );

                var responseData = new ResponseData();
                responseData.Meta.Add( new Link { Method = "GET", Rel = "location", Uri = entityLocation } );

                //CACHE.Add( request.Resource, responseData );
                //Console.WriteLine( "Cached entity: {0}", request.Resource );
                return (T)(IResponseData)responseData;
            }

            if ( result.Data != null && !string.IsNullOrEmpty( request.Resource ) )
            {
                CACHE[request.Resource] = result.Data;
                //CACHE.Add( request.Resource, result.Data );
                Console.WriteLine( "Cached entity: {0}", request.Resource );
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

        private RestRequest InitializeRequest( Link link, Dictionary<string, string> queryParameters )
        {
            var uri = link.Uri;
            if ( uri.Contains( "?" ) )
            {
                queryParameters = ParseQueryParameters( client.BaseUrl + uri );
                uri = uri.Substring( 0, uri.IndexOf( '?' ) );
            }

            var request = new RestRequest( uri, link.Method.ToMethod() ) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new CustomConverter { ContentType = "application/json" };

            if ( link.Method == "GET" && queryParameters != null )
            {
                foreach ( var queryParameter in queryParameters )
                {
                    request.AddParameter( queryParameter.Key, queryParameter.Value );
                }
            }

            return request;
        }

        private static void InsertAuthorizationHeader(ref RestRequest request, TokenData token)
        {
            if ( token != null )
                request.AddHeader( "Authorization", token.TokenType + " " + token.AccessToken );
        }

        private static void InsertIfNoneMatchHeader( ref RestRequest request, object data, Dictionary<string, object> cache )
        {
            if ( data != null && ( data as IVersioned ) != null )
            {
                var d = data as IVersioned;
                request.AddHeader( VersionNumberHeader, d.VersionNumber.ToString( CultureInfo.InvariantCulture ) );
            } 
            else if ( cache.ContainsKey( request.Resource ) && cache[request.Resource] is IVersioned )
            {
                var d = cache[request.Resource] as IVersioned;
                request.AddHeader( VersionNumberHeader, d.VersionNumber.ToString( CultureInfo.InvariantCulture ) ); 
                Console.WriteLine( "Fetched version number from cache" );
            }
            
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

    internal class CustomConverter : ISerializer, IDeserializer
    {
        private static readonly JsonSerializerSettings SerializerSettings;

        static CustomConverter()
        {
            SerializerSettings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Converters = new List<JsonConverter> { new IsoDateTimeConverter() },
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public string Serialize( object obj )
        {
            return JsonConvert.SerializeObject( obj, Formatting.None, SerializerSettings );
        }

        public T Deserialize<T>( IRestResponse response )
        {
            var type = typeof( T );

            return (T)JsonConvert.DeserializeObject( response.Content, type, SerializerSettings );
        }

        string IDeserializer.RootElement { get; set; }
        string IDeserializer.Namespace { get; set; }
        string IDeserializer.DateFormat { get; set; }
        string ISerializer.RootElement { get; set; }
        string ISerializer.Namespace { get; set; }
        string ISerializer.DateFormat { get; set; }
        public string ContentType { get; set; }
    }
}