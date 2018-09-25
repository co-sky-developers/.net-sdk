using System;
using System.Collections.Generic;

namespace NFleet.Data
{
    public class RoutingProblemSettingsData : IVersioned, IResponseData
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.problemsettings";
        public static string MIMEVersion = "2.3";

        public int VersionNumber { get; set; }
        public string DefaultVehicleSpeedProfile { get; set; }
        public double DefaultVehicleSpeedFactor { get; set; }
        public string AlgorithmTree { get; set; }

        public double InsertionAggressiveness { get; set; }
        public String DateTimeFormatString { get; set; }
        public List<Link> Meta { get; set; }
    }
}
