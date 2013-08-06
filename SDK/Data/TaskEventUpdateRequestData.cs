using System;
using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class TaskEventUpdateRequestData
    {
        public int TaskEventId { get; set; }
        public string Type { get; set; }
        public LocationData Location { get; set; }
        public List<CapacityData> Capacities { get; set; }
        public List<TimeWindowData> TimeWindows { get; set; }
        public double ServiceTime { get; set; }
        public DateTime? PlannedArrivalTime { get; set; }
        public DateTime? PlannedDepartureTime { get; set; }

        public TaskEventUpdateRequestData()
        {
            Capacities = new List<CapacityData>();
            TimeWindows = new List<TimeWindowData>();
        }
    }
}
