using System.Collections.Generic;

namespace NFleet.Data
{
    public class RouteData : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.route";
        public static string MIMEVersion = "2.0";

        public List<int> Items { get; set; }
        public List<Link> Meta { get; set; }

        public RouteData()
        {
            Items = new List<int>();
            Meta = new List<Link>();
        }


        int IVersioned.VersionNumber { get; set; }
    }
}
