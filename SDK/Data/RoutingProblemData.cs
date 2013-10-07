using System;
using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class RoutingProblemData : IResponseData
    {
        public RoutingProblemData()
        {
            Meta = new List<Link>();
            Unassigned = new List<int>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<Link> Meta { get; set; }
        public List<int> Unassigned { get; set; }

        public string State { get; set; }

        public int Progress { get; set; }

        public double[,] Distances { get; set; }
        public int[] LocationIndex { get; set; }
    }
}
