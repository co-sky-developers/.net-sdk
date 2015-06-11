using System;

namespace NFleet.Data
{
    public class RoutingProblemSettingsUpdateRequest 
    {
        public string DefaultVehicleSpeedProfile { get; set; }
        public double DefaultVehicleSpeedFactor { get; set; }
        public string AlgorithmTree { get; set; }
        public double InsertionAggressiveness { get; set; }
        public String DateTimeFormatString { get; set; }
    }
}
