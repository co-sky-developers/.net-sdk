using System;
using System.Text;
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

            var uri = self.Uri + op.Uri;
            if (op.Uri.Contains("../"))
                uri = BuildUri(self.Uri, op.Uri);

            return new Link { Method = op.Method, Rel = rel, Uri = uri, Type = op.Type };
        }

        public static string BuildUri(string self, string op)
        {
            self = self.TrimStart('/'); // removes the first slash: /users/10/problems/1/vehicles/1/events/1 => users/10/problems/1/vehicles/1/events/1
            op = op.TrimEnd('/'); // removes the last slash: ../../../../ => ../../../..
            string[] selfParts = self.Split('/');
            string[] parts = op.Split('/');

            var dd = "..";
            var ddCount = 0;

            for (int i = 0; i < parts.Length; i++)
            {
                if (!dd.Equals(parts[i])) break;
                ddCount++;
            }

            StringBuilder newUri = new StringBuilder("");

            for (int i = 0; i < selfParts.Length-ddCount; i++)
            {
                newUri.Append("/" + selfParts[i]);
            }

            for (int i = ddCount; i < parts.Length; i++)
            {
                newUri.Append("/" + parts[i]);
            }

            return newUri.ToString();
        }
    }
}