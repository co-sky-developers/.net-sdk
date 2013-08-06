using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class RoutingProblemDataSet : IResponseData
    {
        public List<RoutingProblemData> Items { get; set; }
        public List<Link> Meta { get; private set; }

        public RoutingProblemDataSet()
        {
            Items = new List<RoutingProblemData>();
            Meta = new List<Link>();
        }
    }
}
