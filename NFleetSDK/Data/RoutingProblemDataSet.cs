using System.Collections.Generic;

namespace NFleet.Data
{
    public class RoutingProblemDataSet : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.problemset";
        public static string MIMEVersion = "2.0";

        int IVersioned.VersionNumber { get; set; }
        public List<RoutingProblemData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public RoutingProblemDataSet()
        {
            Items = new List<RoutingProblemData>();
            Meta = new List<Link>();
        }
    }
}
