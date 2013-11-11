using System.Collections.Generic;
using NFleet.Data;

namespace NFleet
{
    public class ResponseData : IResponseData
    {
        public Link Location { get { return Meta.Find( l => l.Rel == "location" ); } }

        public ResponseData()
        {
            Meta = new List<Link>();
            Items = new List<ErrorData>();
        }

        public List<Link> Meta { get; set; }
        public List<ErrorData> Items { get; set; }

        #region Implementation of IVersioned

        public int VersionNumber { get; set; }

        #endregion
    }
}