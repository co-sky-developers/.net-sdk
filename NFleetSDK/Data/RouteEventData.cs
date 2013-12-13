using System;
using System.Collections.Generic;

namespace NFleet.Data
{
    public class RouteEventData : IResponseData, IVersioned
    {
        int IVersioned.VersionNumber { get; set; }

        public string State { get; set; }

        public string LockState { get; set; }

        public string TimeState { get; set; }

        public bool Pending { get; set; }

        public double WaitingTimeBefore { get; set; }

        public DateTime? ActualArrivalTime { get; set; }

        public DateTime? ActualDepartureTime { get; set; }

        public List<Link> Meta { get; set; }

        public string DataState { get; set; }

        public bool IsFeasible { get; set; }

        public int VehicleId { get; set; }

        public int TaskEventId { get; set; }

        public int SequenceNumber { get; set; }

        public double ActualServiceTime { get; set; }

        public double TravelTimeBefore { get; set; }

        public string InternalState { get; set; }
    }
}
