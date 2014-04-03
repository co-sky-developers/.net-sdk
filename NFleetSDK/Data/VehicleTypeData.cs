using System;
using System.Collections.Generic;

namespace NFleet.Data
{
    public class VehicleTypeData : IResponseData, IVersioned
    {
        public List<String> VehicleTypes { get; set; } 
        public List<Link> Meta { get; set; }
        public int VersionNumber { get; set; }
    }
}
