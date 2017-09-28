using System.Collections.Generic;

namespace NFleet.Data
{
    public class ModificationDataSet : IResponseData
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.modificationset";
        public static string MIMEVersion = "2.0";

        public List<ModificationData> Items { get; set; }

        public List<Link> Meta { get; set; }

        public ModificationDataSet()
        {
            Items = new List<ModificationData>();
            Meta = new List<Link>();
        }
    }
}
