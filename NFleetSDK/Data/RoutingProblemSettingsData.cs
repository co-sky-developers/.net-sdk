using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFleet.Data
{
    public class RoutingProblemSettingsData : IVersioned, IResponseData
    {
        public int VersionNumber { get; set; }
        public SpeedProfile DefaultVehicleSpeedProfile { get; set; }
        public double DefaultVehicleSpeedFactor { get; set; }
        public List<Link> Meta { get; set; }
    }
}
