using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class TaskData : IResponseData
    {
        public int VersionNumber { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public List<TaskEventData> TaskEvents { get; set; }
        public List<Link> Meta { get; set; }
        public bool IsActive { get; set; }

        public TaskData()
        {
            TaskEvents = new List<TaskEventData>();
            Meta = new List<Link>();
        }
    }
}
