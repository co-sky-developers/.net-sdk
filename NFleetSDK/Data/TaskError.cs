using System.Collections.Generic;

namespace NFleet.Data
{
    public class TaskError
    {
        public TaskData Task { get; set; }
        public List<ErrorData> Errors { get; set; }
    }
}
