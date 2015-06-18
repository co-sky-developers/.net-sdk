using System.Collections.Generic;

namespace NFleet.Data
{
    public class VehicleUpdateRequest
    {
        public string Info { get; set; }
        public string ActivityState { get; set; }
        public string Name { get; set; }
        public string VehicleType { get; set; }
        public List<string> IncompatibleVehicleTypes { get; set; }
        public LocationData StartLocation { get; set; }
        public LocationData EndLocation { get; set; }
        public List<CapacityData> Capacities { get; set; }
        public List<TimeWindowData> TimeWindows { get; set; }

        public string SpeedProfile { get; set; }
        public double SpeedFactor { get; set; }

        public string DataSource { get; set; }

        public double FixedCost { get; set; }
        public double KilometerCost { get; set; }
        public double HourCost { get; set; }
        public double MaxDrivingHours { get; set; }
        public double MaxWorkingHours { get; set; }
        public string RelocationType { get; set; }

        public string MimeType { get; set; }
        public string MimeVersion { get; set; } 

        public VehicleUpdateRequest()
        {
            Capacities = new List<CapacityData>();
            TimeWindows = new List<TimeWindowData>();
        }
    }
}
