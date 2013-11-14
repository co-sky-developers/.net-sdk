using System;
using System.Collections.Generic;

namespace NFleet.Data
{
    public class TaskEventData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public string Type { get; set; }
        public List<CapacityData> Capacities { get; set; }
        public List<TimeWindowData> TimeWindows { get; set; }
        public LocationData Location { get; set; }
        public double ServiceTime { get; set; }

        public List<Link> Meta { get; set; }

        public TaskEventData()
        {
            TimeWindows = new List<TimeWindowData>();
            Capacities = new List<CapacityData>();
            Meta = new List<Link>();
        }
    }
}
