using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class VehicleDataSet : IResponseData
    {
        public int VersionNumber { get; set; }
        public List<VehicleData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public VehicleDataSet()
        {
            Items = new List<VehicleData>();
            Meta = new List<Link>();
        }
    }
}
