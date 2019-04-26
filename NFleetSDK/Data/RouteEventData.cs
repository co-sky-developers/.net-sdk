using System;
using System.Collections.Generic;

namespace NFleet.Data
{
    public class RouteEventData : IResponseData, IVersioned
    {
        public static string MIMEType = "application/vnd.jyu.nfleet.routeevent";
        public static string MIMEVersion = "2.0";

        int IVersioned.VersionNumber { get; set; }

        public string State { get; set; }

        public string Name { get; set; }

        public string LockState { get; set; }

        public string TimeState { get; set; }

        public double WaitingTimeBefore { get; set; }

        public DateTime? ArrivalTime { get; set; }

        public DateTime? DepartureTime { get; set; }

        public List<Link> Meta { get; set; }

        public string DataState { get; set; }

        public string FeasibilityState { get; set; }

        public int TaskEventId { get; set; }

        public int SequenceNumber { get; set; }

        public int TaskId { get; set; }

        public KPIData KPIs { get; set; }

        public string Type { get; set; }

        public string Style { get; set; }

        public string Info { get; set; }

        public string Info2 { get; set; }

        public string Info3 { get; set; }

        public string Info4 { get; set; }

        public LocationData Location { get; set; }

        public List<CapacityData> Capacities { get; set; }

        public List<TimeWindowData> TimeWindows { get; set; }
    }
}
