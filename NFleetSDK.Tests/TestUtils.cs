using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NFleet.Data;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Deserializers;

namespace NFleet.Tests
{
    public static class TestUtils
    {
        public static JsonDeserializer Deserializer { private get; set; }

        public static T GetMockResponse<T>( JObject fromJson )
        {
            if ( fromJson == null )
                fromJson = new JObject();

            var mockresponse = new RestResponse
            {
                Content = fromJson.ToString(),
                ContentType = "application/json",
            };

            return Deserializer.Deserialize<T>( mockresponse );
        }

        public static void UsersAreEqual(UserData expected, UserData actual )
        {
            if ( expected == null && actual == null ) return;
            Assert.NotNull( expected, "Expected UserData is null." );
            Assert.NotNull( actual, "Actual UserData is null." );
            ListsAreEqual( expected.Meta, actual.Meta, LinksAreEqual );
        }

        public static void RouteEventsAreEqual(RouteEventData expected, RouteEventData actual)
        {
            Assert.AreEqual( expected.State, actual.State );
            Assert.AreEqual( expected.State, actual.State );
            Assert.AreEqual( expected.WaitingTimeBefore, actual.WaitingTimeBefore );
            Assert.AreEqual( expected.ArrivalTime, actual.ArrivalTime );
            Assert.AreEqual( expected.DepartureTime, actual.DepartureTime );

        }

        private static VehicleData VehiclesAreEqual( VehicleData expected, VehicleData actual )
        {
            Assert.AreEqual( expected.Name, actual.Name );
            LocationsAreEqual( expected.StartLocation, actual.StartLocation );
            LocationsAreEqual( expected.EndLocation, actual.EndLocation );
            ListsAreEqual( expected.Meta, actual.Meta, LinksAreEqual );
            ListsAreEqual( expected.Capacities, actual.Capacities, CapacitiesAreEqual );
            ListsAreEqual( expected.TimeWindows, actual.TimeWindows, TimeWindowsAreEqual );
            return null;
        }

        private static TaskEventData TaskEventsAreEqual( TaskEventData expected, TaskEventData actual )
        {
            Assert.AreEqual( expected.Name, actual.Name );
            Assert.AreEqual( expected.Info, actual.Info );
            Assert.AreEqual( expected.Type, actual.Type );
            
            Assert.AreEqual( expected.ServiceTime, actual.ServiceTime );
            
            LocationsAreEqual( expected.Location, actual.Location );
            ListsAreEqual( expected.Capacities, actual.Capacities, CapacitiesAreEqual );
            ListsAreEqual( expected.TimeWindows, actual.TimeWindows, TimeWindowsAreEqual );
            ListsAreEqual( expected.Meta, actual.Meta, LinksAreEqual );

            return null;
        }

        public static void TaskEventDataSetsAreEqual( TaskEventDataSet expected, TaskEventDataSet actual )
        {
            ListsAreEqual( expected.Items, actual.Items, TaskEventsAreEqual );
            ListsAreEqual( expected.Meta, actual.Meta, LinksAreEqual );
        }

        public static void TaskDataSetsAreEqual(TaskDataSet expected, TaskDataSet actual)
        {
            ListsAreEqual(expected.Items, actual.Items, TasksAreEqual);
            ListsAreEqual(expected.Meta, actual.Meta, LinksAreEqual);
        }

        public static void VehicleDataSetsAreEqual(VehicleDataSet expected, VehicleDataSet actual)
        {
            ListsAreEqual(expected.Items, actual.Items, VehiclesAreEqual);
            ListsAreEqual(expected.Meta, actual.Meta, LinksAreEqual);
        }

        private static void LocationsAreEqual( LocationData expected, LocationData actual )
        {
            if ( expected == null && actual == null ) return;
            Assert.NotNull( expected );
            Assert.NotNull( actual );
            CoordinatesAreEqual( expected.Coordinate, actual.Coordinate );
            AddressesAreEqual( expected.Address, actual.Address );
        }

        private static void CoordinatesAreEqual( CoordinateData expected, CoordinateData actual )
        {
            if ( expected == null && actual == null ) return;
            Assert.NotNull( expected );
            Assert.NotNull( actual );
            Assert.AreEqual( expected.System, actual.System );
            Assert.AreEqual( expected.Latitude, expected.Latitude );
            Assert.AreEqual( expected.Longitude, actual.Longitude );
        }

        private static void AddressesAreEqual( AddressData expected, AddressData actual )
        {
            if ( expected == null && actual == null ) return;
            Assert.NotNull( expected );
            Assert.NotNull( actual );
            Assert.AreEqual( expected.Country, actual.Country );
            Assert.AreEqual( expected.City, actual.City );
            Assert.AreEqual( expected.PostalCode, actual.PostalCode );
            Assert.AreEqual( expected.Street, actual.Street );
        }

        public static void RoutesAreEqual( RouteData expected, RouteData actual )
        {
            CollectionAssert.AreEqual( expected.Items, actual.Items, "Route sequence mismatch." );
        }

        public static void ListsAreEqual<T>( IEnumerable<T> expected, List<T> actual, Func<T, T, T> comparator )
        {
            var bEnumerator = actual.GetEnumerator();
            bEnumerator.MoveNext();

            foreach ( var aItem in expected )
            {
                comparator( aItem, bEnumerator.Current );
                bEnumerator.MoveNext();
            }
        }

        public static Link LinksAreEqual( Link expected, Link actual )
        {
            if ( expected == null && actual == null ) return null;
            Assert.NotNull( expected );
            Assert.NotNull( actual );

            UrisAreEqualEnough( expected.Uri, actual.Uri );
            Assert.AreEqual( expected.Rel, actual.Rel );
            Assert.AreEqual( expected.Method, actual.Method );
            Assert.AreEqual(expected.Type, actual.Type);
            return null;
        }

        private static void UrisAreEqualEnough( string expected, string actual )
        {
            Assert.NotNull( expected );
            Assert.NotNull( actual );

            //TODO: It might be more clever to implement this with some tokens in example code
            const string userPattern = "/users/\\d+(\\S*)";
            const string problemsPattern = "/users/\\d+(\\S*)/problems/\\d+(\\S*)";
            string pattern = userPattern;

            if (expected.Contains("/problems/") && actual.Contains("/problems/"))
            {
                pattern = problemsPattern;
            }

            var amatch = Regex.Match( expected, pattern );
            var bmatch = Regex.Match( actual, pattern );
            if ( amatch.Success )
            {
                Assert.AreEqual( amatch.Groups[1].Value, bmatch.Groups[1].Value );
            }
            else
            {
                Assert.AreEqual( expected, actual );
            }
        }

        private static CapacityData CapacitiesAreEqual( CapacityData expected, CapacityData actual )
        {
            Assert.AreEqual( expected.Amount, actual.Amount );
            Assert.AreEqual( expected.Name, actual.Name );
            return null;
        }

        private static TimeWindowData TimeWindowsAreEqual( TimeWindowData expected, TimeWindowData actual )
        {
            Assert.AreEqual(expected.Start.ToLocalTime(), actual.Start.ToLocalTime());
            Assert.AreEqual(expected.End.ToLocalTime(), actual.End.ToLocalTime());
            return null;
        }

        public static void ResponsesAreEqual( ResponseData expected, ResponseData actual )
        {
            ListsAreEqual( expected.Meta, actual.Meta, LinksAreEqual );
            ListsAreEqual( expected.Items, actual.Items, ErrorsAreEqual );
            LinksAreEqual( expected.Location, actual.Location );
        }

        private static ErrorData ErrorsAreEqual( ErrorData expected, ErrorData actual )
        {
            ListsAreEqual( expected.Meta, actual.Meta, LinksAreEqual );
            Assert.AreEqual( expected.Code, actual.Code );
            Assert.AreEqual( expected.Message, actual.Message );
            return null;
        }

        public static void RoutingProblemsAreEqual(RoutingProblemData expected, RoutingProblemData actual)
        {
            Assert.IsNotNull( expected );
            Assert.IsNotNull( actual );
            Assert.AreEqual( expected.Name, actual.Name );
            ListsAreEqual( expected.Meta, actual.Meta, LinksAreEqual );
            Assert.IsTrue( expected.CreationDate <= expected.ModifiedDate );
        }

 
        public static TaskData TasksAreEqual( TaskData expected, TaskData actual )
        {
            Assert.AreEqual( expected.Info, actual.Info, "Task info mismatch" );
            Assert.AreEqual( expected.Name, actual.Name, "Task name mismatch" );
            Assert.AreEqual( expected.State, actual.State, "Task state mismatch" );
            ListsAreEqual( expected.TaskEvents, actual.TaskEvents, TaskEventsAreEqual );
            ListsAreEqual( expected.Meta, actual.Meta, LinksAreEqual );

            return null;
        }
    }
}