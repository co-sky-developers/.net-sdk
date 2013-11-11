using System.Collections.Generic;

namespace NFleet.Data
{
    public class LocationDataSet : IResponseData
    {
        public int VersionNumber { get; set; }
        public List<Link> Meta { get; set; }
        public List<LocationData> Items { get; set; }

        public LocationDataSet()
        {
            Meta = new List<Link>();
            Items = new List<LocationData>();
        }
    }
}
