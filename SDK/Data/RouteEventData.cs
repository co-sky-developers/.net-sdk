using System;
using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class RouteEventData : IResponseData
    {
        public int VersionNumber { get; set; }

        public string State { get; set; }

        public bool Pending { get; set; }

        public double WaitingTimeBefore { get; set; }

        public DateTime? ActualArrivalTime { get; set; }

        public DateTime? ActualDepartureTime { get; set; }

        public DateTime? PlannedArrivalTime { get; set; }

        public DateTime? PlannedDepartureTime { get; set; }

        public List<Link> Meta { get; set; }
    }
}
