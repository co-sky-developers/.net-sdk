using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFleet.Data
{
    public class RoutingProblemSettingsUpdateRequest 
    {
        public int UserId { get; set; }
        public int ProblemId { get; set; }
        public string DefaultVehicleSpeedProfile { get; set; }
        public double DefaultVehicleSpeedFactor { get; set; }
        public string AlgorithmTree { get; set; }

        public double InsertionAggressiveness { get; set; }
    }
}
