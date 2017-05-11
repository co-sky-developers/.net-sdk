using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using NFleet.Data;
using RestSharp;

namespace NFleet
{
    public class AppService
    {
        private static int retryWaitTimeFactor = 1000;
        private static int unavailableRetryWaitTimeFactor = 10000;
        private static int requestAttempts = 3;

        private readonly RestClient client;
        private readonly string clientKey;
        private readonly string clientSecret;

        public AppService( string appServiceAddress, string clientKey, string clientSecret )
        {
            this.clientKey = clientKey;
            this.clientSecret = clientSecret;
            client = new RestClient( appServiceAddress + "/appusers" ) { FollowRedirects = false };
        }

        public AppUserDataSet Root { get { return Navigate<AppUserDataSet>( new Link { Method = "GET", Uri = "", Type = "application/json" } ); } }

        public ResponseData Navigate( Link link, object data = null )
        {
            return Navigate<ResponseData>( link, data );
        }

        public T Navigate<T>( Link link, object data = null ) where T : IResponseData, new()
        {
            if ( link == null )
                throw new ArgumentNullException( "link" );
            return SendRequest<T>( link, data, requestAttempts );
        }

        private T SendRequest<T>( Link link, object data, int attempts ) where T : new()
        {
            client.ClearHandlers();

            var request = new RestRequest( link.Uri, link.Method.ToMethod() )
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new CustomConverter { ContentType = "application/json" }
            };

            InsertAuthorizationHeader( ref request, clientKey, clientSecret );

            if ( data == null && ( link.Method == "POST" || link.Method == "DELETE" ) )
            {
                request.AddParameter( "application/json", JsonConvert.SerializeObject( new { } ), ParameterType.RequestBody );
            }
            else
            {
                request.AddParameter( "application/json", JsonConvert.SerializeObject( data ), ParameterType.RequestBody );
            }
            request.OnBeforeDeserialization = resp => resp.ContentType = "application/json";
            var result = client.Execute<T>( request );

            if ( result.StatusCode == HttpStatusCode.Unauthorized )
            {
                ThrowException( result );
            }

            if ( result.StatusCode >= HttpStatusCode.InternalServerError && result.StatusCode < HttpStatusCode.BadGateway )
            {
                if ( attempts > 0 )
                {
                    var attempt = requestAttempts - attempts + 1;
                    var waiting = attempt * attempt * retryWaitTimeFactor;
                    Thread.Sleep( waiting );
                    return SendRequest<T>( link, data, attempts - 1 );
                }
                ThrowException( result );
            }
            else if ( result.StatusCode >= HttpStatusCode.BadGateway && result.StatusCode < HttpStatusCode.HttpVersionNotSupported )
            {
                if ( attempts > 0 )
                {
                    var attempt = requestAttempts - attempts + 1;
                    var waiting = attempt * attempt * unavailableRetryWaitTimeFactor;
                    Thread.Sleep( waiting );
                    return SendRequest<T>( link, data, attempts - 1 );
                }
                ThrowException( result );
            }
            else if ( result.StatusCode >= HttpStatusCode.HttpVersionNotSupported )
            {
                ThrowException( result );
            }

            if ( result.StatusCode != HttpStatusCode.NoContent && result.StatusCode == 0 )
                throw new IOException( String.Format( "Could not connect to server at {0}.", client.BaseUrl ) );

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
            else if ( code == HttpStatusCode.Created || code == HttpStatusCode.SeeOther )
            {
                var parameter = result.Headers.FirstOrDefault( h => h.Name == "Location" );
                if ( parameter == null || parameter.Value == null )
                    throw new IOException( "Server response missing Location header." );
                var value = parameter.Value.ToString();
                var entityLocation = new Uri( value ).AbsolutePath;
                var responseData = new ResponseData();
                responseData.Meta.Add( new Link { Method = "GET", Rel = "location", Uri = entityLocation } );
                if ( !( responseData is T ) )
                    throw new InvalidOperationException( String.Format( "The response from the given URL is not of the requested type {0}.", typeof( T ).Name ) );
                return (T)(IResponseData)responseData;
            }

            return result.Data;
        }

        private static void InsertAuthorizationHeader( ref RestRequest request, string clientKey, string clientSecret )
        {
            request.AddHeader( "Authorization", "Basic " + Convert.ToBase64String( Encoding.UTF8.GetBytes( String.Format( "{0}:{1}", clientKey, clientSecret ) ) ) );
        }

        private static void ThrowException<T>( IRestResponse<T> result ) where T : new()
        {
            var code = result.StatusCode;

            var errors = result.Data is ResponseData
                ? ( (ResponseData)(IResponseData)result.Data ).Items
                : new List<ErrorData>();

            throw new NFleetRequestException( String.Format( "{0} {1} from {2}", (int)code, result.StatusDescription, result.ResponseUri ) ) { Items = errors };
        }
    }
}