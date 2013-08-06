using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class LocationDataSet : IResponseData
    {
        public List<Link> Meta { get; set; }
        public List<LocationData> Items { get; set; }

        public LocationDataSet()
        {
            Meta = new List<Link>();
            Items = new List<LocationData>();
        }
    }
}
