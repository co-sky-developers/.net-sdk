using System;
using System.Collections.Generic;
using System.Linq;
using NFleet.Data;

namespace NFleet.Tests
{
    class TestData
    {
        private static string GenerateRandomString( int length )
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat( chars, length )
                          .Select( s => s[random.Next( s.Length )] )
                          .ToArray() );
            return result;
        }

        public static void CreateDemoData(RoutingProblemData problem, Api api)
        {
            api.Navigate<ResponseData>(problem.GetLink("create-vehicle"), new VehicleUpdateRequest
            {
                Name = "TestVehicle-" + GenerateRandomString(8),
                Capacities = new List<CapacityData>
                {
                    new CapacityData { Name = "Weight", Amount = 5000 }
                },
                StartLocation = new LocationData
                {
                    Coordinate = new CoordinateData
                    {
                        Latitude = 62.244958,
                        Longitude = 25.747143,
                        System = "Euclidian"
                    }
                },
                EndLocation = new LocationData
                {
                    Coordinate = new CoordinateData
                    {
                        Latitude = 62.244958,
                        Longitude = 25.747143,
                        System = "Euclidian"
                    }
                },
                TimeWindows = { new TimeWindowData { Start = new DateTime(2013, 5, 14, 8, 0, 0), End = new DateTime(2013, 5, 14, 12, 0, 0) } },
                RelocationType = "None",
            });

            var newTask = new TaskUpdateRequest { Name = "task", RelocationType = "None", ActivityState = "Active" };
            var capacity = new CapacityData { Name = "Weight", Amount = 20 };

            var pickup = new TaskEventUpdateRequest
            {
                Type = "Pickup",
                Location = new LocationData
                {
                    Coordinate = new CoordinateData
                    {
                        Latitude = 62.282617,
                        Longitude = 25.797272,
                        System = "Euclidian"
                    }
                },
                TimeWindows = { new TimeWindowData { Start = new DateTime(2013, 5, 14, 8, 0, 0), End = new DateTime(2013, 5, 14, 12, 0, 0) } }
            };
            pickup.Capacities.Add(capacity);
            newTask.TaskEvents.Add(pickup);

            var delivery = new TaskEventUpdateRequest
            {
                Type = "Delivery",
                Location = new LocationData
                {
                    Coordinate = new CoordinateData
                    {
                        Latitude = 62.373658,
                        Longitude = 25.885506,
                        System = "Euclidian"
                    }
                },
                TimeWindows = { new TimeWindowData { Start = new DateTime(2013, 5, 14, 8, 0, 0), End = new DateTime(2013, 5, 14, 12, 0, 0) } }
            };
            delivery.Capacities.Add(capacity);
            newTask.TaskEvents.Add(delivery);

            api.Navigate<ResponseData>(problem.GetLink("create-task"), newTask);
        }
    }
}
