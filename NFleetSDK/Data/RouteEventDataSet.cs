using System.Collections.Generic;

namespace NFleet.Data
{
    public class RouteEventDataSet : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.routeeventset";
        public static string MIMEVersion = "2.0";

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
