using System.Collections.Generic;

namespace NFleet.Data
{
    public class VehicleDataSet : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.vehicleset";
        public static string MIMEVersion = "2.0";

        int IVersioned.VersionNumber { get; set; }
        public List<VehicleData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public VehicleDataSet()
        {
            Items = new List<VehicleData>();
            Meta = new List<Link>();
        }
    }
}
