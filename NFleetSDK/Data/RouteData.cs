using System.Collections.Generic;

namespace NFleet.Data
{
    public class RouteData : IResponseData
    {
        public int VersionNumber { get; set; }
        public List<int> Items { get; set; }
        public List<Link> Meta { get; set; }

        public RouteData()
        {
            Items = new List<int>();
            Meta = new List<Link>();
        }
    }
}
