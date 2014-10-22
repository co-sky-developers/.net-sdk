using System;
using System.Collections.Generic;

namespace NFleet.Data
{
    public class VehicleTypeData : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.vehicletypes+json";
        public static string MIMEVersion = "2.0";

        public List<String> VehicleTypes { get; set; } 
        public List<Link> Meta { get; set; }
        public int VersionNumber { get; set; }
    }
}
