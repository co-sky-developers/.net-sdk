using System.Collections.Generic;

namespace NFleet.Data
{
    public class TaskUpdateRequest
    {
        public string Name { get; set; }
        public string Info { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }
        public string Info4 { get; set; }
        public double Priority { get; set; }
        public List<string> IncompatibleVehicleTypes { get; set; }
        public List<string> CompatibleVehicleTypes { get; set; }
        public List<string> IncompatibleDepotTypes { get; set; }
        public List<string> CompatibleDepotTypes { get; set; } 
        public List<TaskEventUpdateRequest> TaskEvents { get; set; }
        public string RelocationType { get; set; }
        public string ActivityState { get; set; }
        public bool IsLockedToVehicle { get; set; }
        public string MimeType { get; set; }
        public string MimeVersion { get; set; }


        public TaskUpdateRequest()
        {
            TaskEvents = new List<TaskEventUpdateRequest>();
            IncompatibleVehicleTypes = new List<string>();
            IncompatibleDepotTypes = new List<string>();
            CompatibleDepotTypes = new List<string>();
            CompatibleVehicleTypes = new List<string>();
        }
    }
}
