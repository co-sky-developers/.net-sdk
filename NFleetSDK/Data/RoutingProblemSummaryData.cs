using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFleet.Data
{
    public class RoutingProblemSummaryData : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.problemsummary";
        public static string MIMEVersion = "2.2";
        public int VersionNumber { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Modified { get; set; }
        public string State { get; set; }
        public int Progress { get; set; }
        public RoutingProblemSettingsData Settings { get; set; }
        public SummaryData Summary { get; set; }
        public List<Link> Meta { get; set; }
    }
}
