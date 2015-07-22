using System.Collections.Generic;

namespace NFleet.Data
{
    public class RoutingProblemSummaryDataSet : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.problemsummaryset";
        public static string MIMEVersion = "2.0";
        public List<RoutingProblemSummaryData> Items { get; set; }

        public RoutingProblemSummaryDataSet()
        {
            Items = new List<RoutingProblemSummaryData>();
            Meta = new List<Link>();
        }

        public List<Link> Meta { get; private set; }
        public int VersionNumber { get; set; }
    }
}
