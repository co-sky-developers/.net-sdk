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

        public static void RouteEventsAreEqual(RouteEventData a, RouteEventData b)
        {
            Assert.AreEqual( a.State, b.State );
            Assert.AreEqual( a.LockState, b.LockState );
            Assert.AreEqual( a.TimeState, b.TimeState );
            Assert.AreEqual( a.WaitingTimeBefore, b.WaitingTimeBefore );
            Assert.AreEqual( a.ActualArrivalTime, b.ActualArrivalTime );
            Assert.AreEqual( a.ActualDepartureTime, b.ActualDepartureTime );
            Assert.AreEqual( a.PlannedArrivalTime, b.PlannedArrivalTime );
            Assert.AreEqual( a.PlannedDepartureTime, b.PlannedDepartureTime );

        }

        public static void VehicleDataSetsAreEqual( VehicleDataSet a, VehicleDataSet b )
        {
            if ( a == null && b == null ) return;
            Assert.NotNull( a );
            Assert.NotNull( b );
            ListsAreEqual( a.Items, b.Items, VehiclesAreEqual );
            ListsAreEqual( a.Meta, b.Meta, LinksAreEqual );
        }

        private static VehicleData VehiclesAreEqual( VehicleData a, VehicleData b )
        {
            Assert.AreEqual( a.Name, b.Name );
            LocationsAreEqual( a.StartLocation, b.StartLocation );
            LocationsAreEqual( a.EndLocation, b.EndLocation );
            ListsAreEqual( a.Meta, b.Meta, LinksAreEqual );
            ListsAreEqual( a.Capacities, b.Capacities, CapacitiesAreEqual );
            ListsAreEqual( a.TimeWindows, b.TimeWindows, TimeWindowsAreEqual );
            return null;
        }

        private static TaskEventData TaskEventsAreEqual( TaskEventData a, TaskEventData b )
        {
            Assert.AreEqual( a.Name, b.Name );
            Assert.AreEqual( a.Info, b.Info );
            Assert.AreEqual( a.Type, b.Type );
            
            Assert.AreEqual( a.ServiceTime, b.ServiceTime );
            
            LocationsAreEqual( a.Location, b.Location );
            ListsAreEqual( a.Capacities, b.Capacities, CapacitiesAreEqual );
            ListsAreEqual( a.TimeWindows, b.TimeWindows, TimeWindowsAreEqual );
            ListsAreEqual( a.Meta, b.Meta, LinksAreEqual );

            return null;
        }

        public static void TaskEventDataSetsAreEqual( TaskEventDataSet a, TaskEventDataSet b )
        {
            ListsAreEqual( a.Items, b.Items, TaskEventsAreEqual );
            ListsAreEqual( a.Meta, b.Meta, LinksAreEqual );
        }

        private static void LocationsAreEqual( LocationData a, LocationData b )
        {
            if ( a == null && b == null ) return;
            Assert.NotNull( a );
            Assert.NotNull( b );
            CoordinatesAreEqual( a.Coordinate, b.Coordinate );
            AddressesAreEqual( a.Address, b.Address );
        }

        private static void CoordinatesAreEqual( CoordinateData a, CoordinateData b )
        {
            if ( a == null && b == null ) return;
            Assert.NotNull( a );
            Assert.NotNull( b );
            Assert.AreEqual( a.System, b.System );
            Assert.AreEqual( a.Latitude, a.Latitude );
            Assert.AreEqual( a.Longitude, b.Longitude );
        }

        private static void AddressesAreEqual( AddressData a, AddressData b )
        {
            if ( a == null && b == null ) return;
            Assert.NotNull( a );
            Assert.NotNull( b );
            Assert.AreEqual( a.Country, b.Country );
            Assert.AreEqual( a.City, b.City );
            Assert.AreEqual( a.PostalCode, b.PostalCode );
            Assert.AreEqual( a.StreetAddress, b.StreetAddress );
        }

        public static void RoutesAreEqual( RouteData a, RouteData b )
        {
            CollectionAssert.AreEqual( a.Items, b.Items, "Route sequence mismatch." );
        }

        public static void ListsAreEqual<T>( IEnumerable<T> a, List<T> b, Func<T, T, T> comparator )
        {
            var bEnumerator = b.GetEnumerator();
            bEnumerator.MoveNext();

            foreach ( var aItem in a )
            {
                comparator( aItem, bEnumerator.Current );
                bEnumerator.MoveNext();
            }
        }

        public static Link LinksAreEqual( Link a, Link b )
        {
            if ( a == null && b == null ) return null;
            Assert.NotNull( a );
            Assert.NotNull( b );

            UrisAreEqualEnough( a.Uri, b.Uri );
            Assert.AreEqual( a.Rel, b.Rel );
            Assert.AreEqual( a.Method, b.Method );
            return null;
        }

        private static void UrisAreEqualEnough( string a, string b )
        {
            Assert.NotNull( a );
            Assert.NotNull( b );

            //TODO: It might be more clever to implement this with some tokens in example code
            const string userPattern = "/users/\\d+(\\S*)";
            const string problemsPattern = "/users/\\d+(\\S*)/problems/\\d+(\\S*)";
            string pattern = userPattern;

            if (a.Contains("/problems/") && b.Contains("/problems/"))
            {
                pattern = problemsPattern;
            }

            var amatch = Regex.Match( a, pattern );
            var bmatch = Regex.Match( b, pattern );
            if ( amatch.Success )
            {
                Assert.AreEqual( amatch.Groups[1].Value, bmatch.Groups[1].Value );
            }
            else
            {
                Assert.AreEqual( a, b );
            }
        }

        private static CapacityData CapacitiesAreEqual( CapacityData a, CapacityData b )
        {
            Assert.AreEqual( a.Amount, b.Amount );
            Assert.AreEqual( a.Name, b.Name );
            return null;
        }

        private static TimeWindowData TimeWindowsAreEqual( TimeWindowData a, TimeWindowData b )
        {
            Assert.AreEqual( a.Start, b.Start );
            Assert.AreEqual( a.End, b.End );
            return null;
        }

        public static void ResponsesAreEqual( ResponseData a, ResponseData b )
        {
            ListsAreEqual( a.Meta, b.Meta, LinksAreEqual );
            ListsAreEqual( a.Items, b.Items, ErrorsAreEqual );
            LinksAreEqual( a.Location, b.Location );
        }

        private static ErrorData ErrorsAreEqual( ErrorData a, ErrorData b )
        {
            ListsAreEqual( a.Meta, b.Meta, LinksAreEqual );
            Assert.AreEqual( a.Code, b.Code );
            Assert.AreEqual( a.Message, b.Message );
            return null;
        }

        public static void RoutingProblemsAreEqual(RoutingProblemData a, RoutingProblemData b)
        {
            Assert.IsNotNull( a );
            Assert.IsNotNull( b );
            Assert.AreEqual( a.Name, b.Name );
            Assert.AreEqual( a.VersionNumber, b.VersionNumber );
            ListsAreEqual( a.Meta, b.Meta, LinksAreEqual );
            Assert.IsTrue( a.CreationDate <= a.ModifiedDate );
        }

        public static void TaskDataSetsAreEqual( TaskDataSet a, TaskDataSet b )
        {
            if ( a == null && b == null ) return;
            Assert.NotNull( a );
            Assert.NotNull( b );
            ListsAreEqual( a.Meta, b.Meta, LinksAreEqual );
            ListsAreEqual( a.Items, b.Items, TasksAreEqual );
        }

        public static TaskData TasksAreEqual( TaskData expected, TaskData actual )
        {
            Assert.AreEqual( expected.VersionNumber, actual.VersionNumber, "Task version number mismatch" );
            Assert.AreEqual( expected.Info, actual.Info, "Task info mismatch" );
            Assert.AreEqual( expected.Name, actual.Name, "Task name mismatch" );
            Assert.AreEqual( expected.State, actual.State, "Task state mismatch" );
            ListsAreEqual( expected.TaskEvents, actual.TaskEvents, TaskEventsAreEqual );
            ListsAreEqual( expected.Meta, actual.Meta, LinksAreEqual );

            return null;
        }
    }
}