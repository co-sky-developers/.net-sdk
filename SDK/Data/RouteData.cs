using System;
using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class RouteData : IResponseData
    {
        public int VersionNumber { get; set; }
        public List<int> Items { get; set; }
        public List<Link> Meta { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public DateTime? PlannedStartTime { get; set; }
        public DateTime? PlannedEndTime { get; set; }
        public string ArrivalTimeState { get; set; }
        public string DepartureTimeState { get; set; }

        public RouteData()
        {
            Items = new List<int>();
            Meta = new List<Link>();
        }
    }
}
