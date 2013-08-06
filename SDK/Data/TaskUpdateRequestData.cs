using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class TaskUpdateRequestData
    {
        public int UserId { get; set; }
        public int ProblemId { get; set; }
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public List<TaskEventUpdateRequestData> TaskEvents { get; set; }

        public TaskUpdateRequestData()
        {
            TaskEvents = new List<TaskEventUpdateRequestData>();
        }
    }
}
