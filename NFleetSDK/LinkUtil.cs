using System;
using NFleet.Data;
using RestSharp;

namespace NFleet
{
    public static class LinkUtil
    {
        public static Method ToMethod( this string method )
        {
            switch ( method )
            {
                case "GET": return Method.GET;
                case "PUT": return Method.PUT;
                case "DELETE": return Method.DELETE;
                case "POST": return Method.POST;
                default: throw new NotSupportedException();
            }
        }

        public static Link GetLink( this IResponseData data, string rel )
        {
            var self = data.Meta.Find( l => l.Rel == "self" );
            if ( rel == self.Rel ) return self;
            var op = data.Meta.Find( l => l.Rel == rel );
            if ( op == null )
                throw new InvalidOperationException( String.Format( "Operation '{0}' cannot currently be performed on this object.", rel ) );
            return new Link { Method = op.Method, Rel = rel, Uri = self.Uri + op.Uri };
        }
    }
}