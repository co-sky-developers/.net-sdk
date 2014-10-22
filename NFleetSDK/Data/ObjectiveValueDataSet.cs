using System.Collections.Generic;

namespace NFleet.Data
{
    public class ObjectiveValueDataSet : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.objectivevalueset+json";
        public static string MIMEVersion = "2.0";

        int IVersioned.VersionNumber { get; set; }
        public List<ObjectiveValueData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public ObjectiveValueDataSet()
        {
            Items = new List<ObjectiveValueData>();
            Meta = new List<Link>();
        }
    }
}
