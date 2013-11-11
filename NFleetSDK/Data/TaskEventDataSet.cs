using System.Collections.Generic;

namespace NFleet.Data
{
    public class TaskEventDataSet : IResponseData
    {
        public int VersionNumber { get; set; }
        public List<TaskEventData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public TaskEventDataSet()
        {
            Items = new List<TaskEventData>();
            Meta = new List<Link>();
        }
    }
}
