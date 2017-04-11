using System;
using System.Collections.Generic;

namespace NFleet.Data
{
    public class TaskEventUpdateRequest
    {
        public string Type { get; set; }
        public string Style { get; set; }
        public LocationData Location { get; set; }
        public List<CapacityData> Capacities { get; set; }
        public List<TimeWindowData> TimeWindows { get; set; }
        public double ServiceTime { get; set; }
        public double StoppingTime { get; set; }

        public string VehicleId { get; set; }
        public int SequenceNumber { get; set; }

        public bool IsLocked { get; set; }
        public DateTime? PresetArrivalTime { get; set; }

        public TaskEventUpdateRequest()
        {
            Capacities = new List<CapacityData>();
            TimeWindows = new List<TimeWindowData>();
        }
    }
}
