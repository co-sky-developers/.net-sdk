using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class VehicleUpdateRequestData
    {
        public int UserId { get; set; }
        public int ProblemId { get; set; }
        public int VehicleId { get; set; }

        public string Name { get; set; }
        public LocationData StartLocation { get; set; }
        public LocationData EndLocation { get; set; }
        public List<CapacityData> Capacities { get; set; }
        public List<TimeWindowData> TimeWindows { get; set; }

        public VehicleUpdateRequestData()
        {
            Capacities = new List<CapacityData>();
            TimeWindows = new List<TimeWindowData>();
        }
    }
}
