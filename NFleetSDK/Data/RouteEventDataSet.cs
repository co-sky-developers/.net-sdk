using System.Collections.Generic;

namespace NFleet.Data
{
    public class RouteEventDataSet : IResponseData, IVersioned
    {
        int IVersioned.VersionNumber { get; set; }

        public List<RouteEventData> Items { get; set; }

        public List<Link> Meta { get; set; }

        public RouteEventDataSet()
        {
            Items = new List<RouteEventData>();

            Meta = new List<Link>();
        }
    }
}
