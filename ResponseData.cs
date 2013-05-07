using System.Collections.Generic;
using Sky.NFleet.Service.Response;

namespace Sky.NFleet.Service.SDK
{
    public class ResponseData : IResponseData
    {
        public Link Location { get { return Meta.Find( l => l.Rel == "location" ); } }

        public ResponseData()
        {
            Meta = new List<Link>();
        }

        public List<Link> Meta { get; private set; }
    }
}