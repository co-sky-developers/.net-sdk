using System.Collections.Generic;

namespace NFleet.Data
{
    public class TaskData : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.task";
        public static string MIMEVersion = "2.0";

        int IVersioned.VersionNumber { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public List<string> IncompatibleVehicleTypes { get; set; } 
        public List<TaskEventData> TaskEvents { get; set; }
        public List<Link> Meta { get; set; }
        public string State { get; set; }
        public double Priority { get; set; }
        public string RelocationType { get; set; }
        public string ActivityState { get; set; }

        public TaskData()
        {
            TaskEvents = new List<TaskEventData>();
            Meta = new List<Link>();
        }
    }
}
