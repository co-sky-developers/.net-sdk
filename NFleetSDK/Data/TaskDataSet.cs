using System.Collections.Generic;

namespace NFleet.Data
{
    public class TaskDataSet : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.taskset";
        public static string MIMEVersion = "2.2";

        int IVersioned.VersionNumber { get; set; }
        public List<TaskData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public TaskDataSet()
        {
            Items = new List<TaskData>();
            Meta = new List<Link>();
        }
    }
}
