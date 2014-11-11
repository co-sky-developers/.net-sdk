using System.Collections.Generic;

namespace NFleet.Data
{
    public class RoutingProblemSettingsData : IVersioned, IResponseData
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.problemsettings";
        public static string MIMEVersion = "2.0";

        public int VersionNumber { get; set; }
        public SpeedProfile DefaultVehicleSpeedProfile { get; set; }
        public double DefaultVehicleSpeedFactor { get; set; }
        public List<Link> Meta { get; set; }
    }
}
