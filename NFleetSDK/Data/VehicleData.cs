using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NFleet.Data
{
    [DataContract]
    public class VehicleData : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.vehicle";
        public static string MIMEVersion = "2.0";

        [IgnoreDataMember]
        int IVersioned.VersionNumber { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Info1 { get; set; }
        [DataMember]
        public List<CapacityData> Capacities { get; set; }
        [DataMember]
        public List<TimeWindowData> TimeWindows { get; set; }
        [DataMember]
        public LocationData StartLocation { get; set; }
        [DataMember]
        public LocationData EndLocation { get; set; }
        [DataMember]
        public List<Link> Meta { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string VehicleType { get; set; }
        [DataMember]
        public string SpeedProfile { get; set; }
        [DataMember]
        public double SpeedFactor { get; set; }
        [DataMember]
        public double FixedCost { get; set; }
        [DataMember]
        public double KilometerCost { get; set; }
        [DataMember]
        public double HourCost { get; set; }
        [DataMember]
        public double MaxDrivingHours { get; set; }
        [DataMember]
        public double MaxWorkingHours { get; set; }
        [DataMember]
        public string RelocationType { get; set; }

        public VehicleData()
        {
            Capacities = new List<CapacityData>();
            TimeWindows = new List<TimeWindowData>();
            Meta = new List<Link>();
        }
    }
}
