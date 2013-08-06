using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class TaskEventDataSet : IResponseData
    {
        public List<TaskEventData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public TaskEventDataSet()
        {
            Items = new List<TaskEventData>();
            Meta = new List<Link>();
        }
    }
}
