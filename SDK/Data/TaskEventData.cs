using System;
using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class TaskEventData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public string LockState { get; set; }
        public string TimeState { get; set; }
        public List<CapacityData> Capacities { get; set; }
        public List<TimeWindowData> TimeWindows { get; set; }
        public LocationData Location { get; set; }
        public double ServiceTime { get; set; }
        public double WaitingTime { get; set; }
        public DateTime? ActualArrivalTime { get; set; }
        public DateTime? ActualDepartureTime { get; set; }
        public DateTime? PlannedArrivalTime { get; set; }
        public DateTime? PlannedDepartureTime { get; set; }
        public List<Link> Meta { get; set; }
        public double OriginalServiceTime { get; set; }

        public TaskEventData()
        {
            TimeWindows = new List<TimeWindowData>();
            Capacities = new List<CapacityData>();
            Meta = new List<Link>();
        }
    }
}
