using System;
using System.Collections.Generic;

namespace NFleet.Data
{
    public class RoutingProblemData : IResponseData
    {
        public RoutingProblemData()
        {
            Meta = new List<Link>();
        }

        public int VersionNumber { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<Link> Meta { get; set; }

        public string State { get; set; }

        public int Progress { get; set; }
    }
}
