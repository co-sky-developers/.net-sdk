using System.Collections.Generic;

namespace NFleet.Data
{
    public class TaskSetImportRequest
    {
        public List<TaskUpdateRequest> Items { get; set; }

        public TaskSetImportRequest()
        {
            Items = new List<TaskUpdateRequest>();
        }
    }
}
