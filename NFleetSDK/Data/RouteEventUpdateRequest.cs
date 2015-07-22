using System;

namespace NFleet.Data
{
    public class RouteEventUpdateRequest
    {
        public int EventSequenceNumber { get; set; }
        public DateTime? ActualArrivalTime { get; set; }
        public string State { get; set; }
    }
}
