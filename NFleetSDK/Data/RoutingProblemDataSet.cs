using System.Collections.Generic;

namespace NFleet.Data
{
    public class RoutingProblemDataSet : IResponseData, IVersioned
    {
        int IVersioned.VersionNumber { get; set; }
        public List<RoutingProblemData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public RoutingProblemDataSet()
        {
            Items = new List<RoutingProblemData>();
            Meta = new List<Link>();
        }
    }
}
