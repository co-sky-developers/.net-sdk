using System;

namespace NFleet.Data
{
    public class RouteEventUpdateRequest
    {
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public int ProblemId { get; set; }
        public int VehicleId { get; set; }
        public int EventSequenceNumber { get; set; }
        public DateTime? ActualArrivalTime { get; set; }
        public string State { get; set; }
    }
}
