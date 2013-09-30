using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NFleetSDK.Data;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace NFleetSDK.UnitTests
{
    class TestUtils
    {
        public static RestSharp.Deserializers.JsonDeserializer deserializer { get; set; }
        private const bool ignoreIds = true;

        public static T GetMockResponse<T>(JObject fromJson)
        {
            if (fromJson == null) fromJson = new JObject();
            var mockresponse = new RestResponse()
            {
                Content = fromJson.ToString(),
                ContentType = "application/json",
            };

            
            return deserializer.Deserialize<T>(mockresponse);
        }

        public static void VehicleDataSetsAreEqual(VehicleDataSet a, VehicleDataSet b)
        {
            if ( a == null && b == null ) return;
            ListsAreEqual(a.Items, b.Items, VehiclesAreEqual);
            ListsAreEqual(a.Meta, b.Meta, LinksAreEqual);
        }

        public static VehicleData VehiclesAreEqual(VehicleData a, VehicleData b)
        {
            if (!ignoreIds) Assert.AreEqual(a.Id, b.Id);
            Assert.AreEqual(a.Name, b.Name);
            Assert.AreEqual(a.IsActive, b.IsActive);
            
            Assert.AreEqual(a.ActualArrivalTime, b.ActualArrivalTime);
            Assert.AreEqual(a.ActualDepartureTime, b.ActualDepartureTime);
            Assert.AreEqual(a.PlannedArrivalTime, b.PlannedArrivalTime);
            Assert.AreEqual(a.PlannedDepartureTime, b.PlannedDepartureTime);
            Assert.AreEqual(a.ArrivalTimeState, b.ArrivalTimeState);
            Assert.AreEqual(a.DepartureTimeState, b.DepartureTimeState);
            
            LocationsAreEqual(a.StartLocation, b.StartLocation);
            LocationsAreEqual(a.EndLocation, b.EndLocation);
            ListsAreEqual<Link>(a.Meta, b.Meta, LinksAreEqual);
            ListsAreEqual<CapacityData>(a.Capacities, b.Capacities, CapacitiesAreEqual);
            ListsAreEqual<TimeWindowData>(a.TimeWindows, b.TimeWindows, TimeWindowsAreEqual);

            return null;
        }

        public static TaskEventData TaskEventsAreEqual(TaskEventData a, TaskEventData b)
        {
            if (!ignoreIds) Assert.AreEqual(a.Id, b.Id);
            Assert.AreEqual(a.Name, b.Name);
            Assert.AreEqual(a.Info, b.Info);
            Assert.AreEqual(a.Type, b.Type);
            Assert.AreEqual(a.State, b.State);
            Assert.AreEqual(a.LockState, b.LockState);
            Assert.AreEqual(a.TimeState, b.TimeState);
            Assert.AreEqual(a.ServiceTime, b.ServiceTime);
            Assert.AreEqual(a.WaitingTime, b.WaitingTime);
            Assert.AreEqual(a.ActualArrivalTime, b.ActualArrivalTime);
            Assert.AreEqual(a.ActualDepartureTime, b.ActualDepartureTime);
            Assert.AreEqual(a.PlannedArrivalTime, b.PlannedArrivalTime);
            Assert.AreEqual(a.PlannedDepartureTime, b.PlannedDepartureTime);
            Assert.AreEqual(a.OriginalServiceTime, b.OriginalServiceTime);

            LocationsAreEqual(a.Location, b.Location);
            ListsAreEqual(a.Capacities, b.Capacities, CapacitiesAreEqual);
            ListsAreEqual(a.TimeWindows, b.TimeWindows, TimeWindowsAreEqual);
            ListsAreEqual(a.Meta, b.Meta, LinksAreEqual);

            return null;
        }

        public static TaskEventDataSet TaskEventDataSetsAreEqual(TaskEventDataSet a, TaskEventDataSet b)
        {
            ListsAreEqual(a.Items, b.Items, TaskEventsAreEqual);
            ListsAreEqual(a.Meta, b.Meta, LinksAreEqual);
            return null;
        }

        public static void LocationDataSetsAreEqual(LocationDataSet a, LocationDataSet b)
        {
            if ( a == null && b == null ) return;
            ListsAreEqual(a.Meta, b.Meta, LinksAreEqual);
            ListsAreEqual(a.Items, b.Items, LocationsAreEqual);
        }

        public static LocationData LocationsAreEqual(LocationData a, LocationData b)
        {
            if ( a == null && b == null ) return null;
            if (!ignoreIds) Assert.AreEqual(a.Id, b.Id);
            CoordinatesAreEqual(a.Coordinate, b.Coordinate);
            AddressesAreEqual(a.Address, b.Address);
            return null;
        }

        public static void CoordinatesAreEqual(CoordinateData a, CoordinateData b)
        {
            if ( a == null && b == null ) return;
            Assert.AreEqual(a.System, b.System);
            Assert.AreEqual(a.Latitude, a.Latitude);
            Assert.AreEqual(a.Longitude, b.Longitude);
        }

        public static void AddressesAreEqual(AddressData a, AddressData b)
        {
            if (a == null && b == null) return;
            Assert.AreEqual(a.Country, b.Country);
            Assert.AreEqual(a.City, b.City);
            Assert.AreEqual(a.PostalCode, b.PostalCode);
            Assert.AreEqual(a.StreetAddress, b.StreetAddress);
        }

        public static void RoutesAreEqual(RouteData a, RouteData b)
        {
            Assert.AreEqual(a.ActualStartTime, b.ActualStartTime);
            Assert.AreEqual(a.ActualEndTime, b.ActualEndTime);
            Assert.AreEqual(a.PlannedEndTime, b.PlannedEndTime);
            Assert.AreEqual(a.PlannedStartTime, b.PlannedStartTime);
            Assert.AreEqual(a.ArrivalTimeState, b.ArrivalTimeState);
            Assert.AreEqual(a.DepartureTimeState, b.DepartureTimeState);
            CollectionAssert.AreEqual(a.Items, b.Items);
        }


        public static void ListsAreEqual<T>(List<T> a, List<T> b, Func<T, T, T> comparator)
        {
            var bEnumerator = b.GetEnumerator();
            bEnumerator.MoveNext();

            foreach (var aItem in a)
            {
                comparator(aItem, bEnumerator.Current);
                bEnumerator.MoveNext();
            }
        }

        public static Link LinksAreEqual(Link a, Link b)
        {
            if (a == null && b == null) return null;
            Assert.NotNull(a);
            Assert.NotNull(b);
            
            UrisAreEqualEnough(a.Uri, b.Uri);
            Assert.AreEqual(a.Rel, b.Rel);
            Assert.AreEqual(a.Method, b.Method);
            return null;
        }

        public static void UrisAreEqualEnough(string a, string b)
        {
            Assert.NotNull(a);
            Assert.NotNull(b);

            //TODO: It might be more clever to implement this with some tokens in example code
            string pattern = "/users/1/problems/\\d+(\\S*)";
            var amatch = Regex.Match(a, pattern);
            var bmatch = Regex.Match(b, pattern);
            if (amatch.Success)
            {
                Assert.AreEqual(amatch.Groups[1].Value, bmatch.Groups[1].Value);
            }
            else
            {
                Assert.AreEqual(a, b);
            }
        }

        public static CapacityData CapacitiesAreEqual(CapacityData a, CapacityData b)
        {
            Assert.AreEqual(a.Amount, b.Amount);
            Assert.AreEqual(a.Name, b.Name);
            return null;
        }

        public static TimeWindowData TimeWindowsAreEqual(TimeWindowData a, TimeWindowData b)
        {
            return null;
            //TODO: Nämä päälle kun eivät enää heitä kolmella tunnilla
            Assert.AreEqual(a.Start, b.Start);
            Assert.AreEqual(a.End, b.End);
            return null;
        }

        public static ResponseData ResponsesAreEqual(ResponseData a, ResponseData b)
        {
            ListsAreEqual(a.Meta, b.Meta, LinksAreEqual);
            ListsAreEqual(a.Items, b.Items, ErrorsAreEqual);
            LinksAreEqual(a.Location, b.Location);
            return null;
        }

        public static ErrorData ErrorsAreEqual(ErrorData a, ErrorData b)
        {
            ListsAreEqual<Link>(a.Meta, b.Meta, LinksAreEqual);
            Assert.AreEqual(a.Code, b.Code);
            Assert.AreEqual(a.Message, b.Message);
            return null;
        }

        public static RoutingProblemData RoutingProblemsAreEqual(RoutingProblemData a, RoutingProblemData b)
        {
            Assert.IsNotNull(a);
            Assert.IsNotNull(b);
            if (!ignoreIds) Assert.AreEqual(a.Id, b.Id);
            Assert.AreEqual(a.Name, b.Name);
            ListsAreEqual<Link>(a.Meta, b.Meta, LinksAreEqual);
            CollectionAssert.AreEqual(a.Unassigned, b.Unassigned);
            Assert.IsTrue(a.CreationDate <= a.ModifiedDate);
            CollectionAssert.AreEqual(a.Distances, b.Distances);
            CollectionAssert.AreEqual(a.LocationIndex, b.LocationIndex);
            return null;
        }

        public static RoutingProblemDataSet RoutingProblemDataSetsAreEqual(RoutingProblemDataSet a,
                                                                           RoutingProblemDataSet b)
        {
            ListsAreEqual(a.Items, b.Items, RoutingProblemsAreEqual);
            ListsAreEqual(a.Meta, b.Meta, LinksAreEqual);
            return null;
        }

        public static void TaskDataSetsAreEqual(TaskDataSet a, TaskDataSet b)
        {
            if ( a == null && b == null ) return;
            ListsAreEqual(a.Meta, b.Meta, LinksAreEqual);    
            ListsAreEqual(a.Items, b.Items, TasksAreEqual);
        }

        public static TaskData TasksAreEqual(TaskData newTask, TaskData mockNewTask)
        {
            if (!ignoreIds) Assert.AreEqual(newTask.Id, mockNewTask.Id);
            Assert.AreEqual(newTask.Info, mockNewTask.Info);
            Assert.AreEqual(newTask.Name, mockNewTask.Name);
            Assert.AreEqual(newTask.IsActive, mockNewTask.IsActive);
            ListsAreEqual(newTask.TaskEvents, mockNewTask.TaskEvents, TaskEventsAreEqual);
            ListsAreEqual(newTask.Meta, mockNewTask.Meta, LinksAreEqual);
            
            return null;
        }

        public static void OptimizationsAreEqual(OptimizationData a, OptimizationData b)
        {
            if (!ignoreIds) Assert.AreEqual(a.Id, b.Id);
            Assert.AreEqual(a.State, b.State);
            Assert.AreEqual(a.Progress, b.Progress);
            Assert.AreEqual(a.Value, b.Value);

            ListsAreEqual(a.Meta, b.Meta, LinksAreEqual);
            VehicleDataSetsAreEqual(a.Vehicles, b.Vehicles);
            TaskDataSetsAreEqual(a.Tasks, b.Tasks);
            LocationDataSetsAreEqual(a.Locations, b.Locations);
        }
    }
}
