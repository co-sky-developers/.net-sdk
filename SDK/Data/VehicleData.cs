using System;
using System.Collections.Generic;

namespace NFleetSDK.Data
{
    public class VehicleData : IResponseData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CapacityData> Capacities { get; set; }
        public List<TimeWindowData> TimeWindows { get; set; }
        public LocationData StartLocation { get; set; }
        public LocationData EndLocation { get; set; }
        public List<Link> Meta { get; set; }
        public RouteData Route { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ActualArrivalTime { get { return Route.ActualEndTime; } set { Route.ActualEndTime = value; } }
        public DateTime? ActualDepartureTime { get { return Route.ActualStartTime; } set { Route.ActualStartTime = value; } }
        public DateTime? PlannedArrivalTime { get { return Route.PlannedEndTime; } set { Route.PlannedEndTime = value; } }
        public DateTime? PlannedDepartureTime { get { return Route.PlannedStartTime; } set { Route.PlannedStartTime = value; } }
        public string DepartureTimeState { get { return Route.DepartureTimeState; } set { Route.DepartureTimeState = value; } } 
        public string ArrivalTimeState { get { return Route.ArrivalTimeState; } set { Route.ArrivalTimeState = value; } } 

        public VehicleData()
        {
            Capacities = new List<CapacityData>();
            TimeWindows = new List<TimeWindowData>();
            Meta = new List<Link>();
            Route = new RouteData();
            IsActive = true;
        }
    }
}
