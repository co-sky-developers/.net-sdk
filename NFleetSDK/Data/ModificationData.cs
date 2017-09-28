using System.Collections.Generic;

namespace NFleet.Data
{
    public class ModificationData : IResponseData
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.modification";
        public static string MIMEVersion = "2.0";

        public int Id { get; set; }

        public string Name { get; set; }

        public string Info1 { get; set; }        

        public List<string> SpeedProfiles { get; set; }

        public double SpeedFactor { get; set; }

        public List<ModificationAreaData> Areas { get; set; }

        public List<Link> Meta { get; set; }

        public string DataSource { get; set; }

        public ModificationData()
        {
            Meta = new List<Link>();
            Areas = new List<ModificationAreaData>();
        }
    }
}