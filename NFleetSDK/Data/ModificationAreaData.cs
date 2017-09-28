using System.Collections.Generic;

namespace NFleet.Data
{
    public class ModificationAreaData
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.modificationarea";
        public static string MIMEVersion = "2.0";

        public CoordinateData TopLeft { get; set; }

        public CoordinateData BottomRight { get; set; }

        public string DataSource { get; set; }

        public List<Link> Meta { get; set; }

        public ModificationAreaData()
        {
            Meta = new List<Link>();
        }
    }
}