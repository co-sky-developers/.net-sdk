using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFleet.Data
{
    public class RouteEventRequest
    {
        public int UserId { get; set; }
        public int ProblemId { get; set; }
        public int VehicleId { get; set; }
        public int EventSequenceNumber { get; set; }
    }
}
