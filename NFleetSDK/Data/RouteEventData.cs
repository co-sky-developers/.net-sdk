using System;
using System.Collections.Generic;

namespace NFleet.Data
{
    public class RouteEventData : IResponseData, IVersioned
    {
        int IVersioned.VersionNumber { get; set; }

        public string State { get; set; }

        public double WaitingTimeBefore { get; set; }

        public DateTime? ArrivalTime { get; set; }

        public DateTime? DepartureTime { get; set; }

        public List<Link> Meta { get; set; }

        public string DataState { get; set; }

        public string FeasibilityState { get; set; }

        public int TaskEventId { get; set; }

        public KPIData KPIs { get; set; }
    }
}
