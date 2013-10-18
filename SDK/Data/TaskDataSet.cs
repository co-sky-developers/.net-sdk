using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class TaskDataSet : IResponseData
    {
        public int VersionNumber { get; set; }
        public List<TaskData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public TaskDataSet()
        {
            Items = new List<TaskData>();
            Meta = new List<Link>();
        }
    }
}
