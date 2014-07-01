using System.Collections.Generic;

namespace NFleet.Data
{
    public class VehicleUpdateRequest
    {
        public int VehicleId { get; set; }
        public string Info { get; set; }
        public string State { get; set; }
        public string Name { get; set; }
        public string VehicleType { get; set; }
        public List<string> IncompatibleVehicleTypes { get; set; }
        public LocationData StartLocation { get; set; }
        public LocationData EndLocation { get; set; }
        public List<CapacityData> Capacities { get; set; }
        public List<TimeWindowData> TimeWindows { get; set; }
        public double FixedCost { get; set; }
        public double KilometerCost { get; set; }
        public double HourCost { get; set; }

        public VehicleUpdateRequest()
        {
            Capacities = new List<CapacityData>();
            TimeWindows = new List<TimeWindowData>();
        }
    }
}
