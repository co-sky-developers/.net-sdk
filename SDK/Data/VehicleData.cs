using System;
using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class VehicleData : IResponseData
    {
        public int VersionNumber { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CapacityData> Capacities { get; set; }
        public List<TimeWindowData> TimeWindows { get; set; }
        public LocationData StartLocation { get; set; }
        public LocationData EndLocation { get; set; }
        public List<Link> Meta { get; set; }
        public string State { get; set; }

        public VehicleData()
        {
            Capacities = new List<CapacityData>();
            TimeWindows = new List<TimeWindowData>();
            Meta = new List<Link>();
        }
    }
}
