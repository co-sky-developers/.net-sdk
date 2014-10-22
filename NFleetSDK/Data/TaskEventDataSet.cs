using System.Collections.Generic;

namespace NFleet.Data
{
    public class TaskEventDataSet : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.taskeventset+json";
        public static string MIMEVersion = "2.0";

        int IVersioned.VersionNumber { get; set; }
        public List<TaskEventData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public TaskEventDataSet()
        {
            Items = new List<TaskEventData>();
            Meta = new List<Link>();
        }
    }
}
