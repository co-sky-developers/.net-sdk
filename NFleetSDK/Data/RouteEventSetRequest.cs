using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFleet.Data
{
    public class RouteEventSetRequest
    {
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public int ProblemId { get; set; }
        public int VehicleId { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}
