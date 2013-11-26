﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NFleet.Data
{
    [DataContract]
    public class VehicleData : IResponseData, IVersioned
    {
        [IgnoreDataMember]
        int IVersioned.VersionNumber { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
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

        public VehicleData()
        {
            Capacities = new List<CapacityData>();
            TimeWindows = new List<TimeWindowData>();
            Meta = new List<Link>();
        }
    }
}
