using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using NFleet.Data;

namespace NFleet.Example
{
    public static class Program
    {
        private static readonly string url = "";
        private static readonly string clientKey = "";
        private static readonly string clientSecret = "";

        static Program()
        {
            url = ConfigurationManager.AppSettings["url"];
            clientKey = ConfigurationManager.AppSettings["client-key"];
            clientSecret = ConfigurationManager.AppSettings["client-secret"];
        }

        public static void Main()
        {
            try
            {
                Run();
            }
            catch ( IOException e )
            {
                Console.WriteLine( e.Message );
            }
            catch ( Exception e )
            {
                Console.WriteLine( e.Message + Environment.NewLine + e.StackTrace );
            }
            Console.ReadLine();
        }

        private static void Run()
        {


            var api1 = new Api( url, clientKey, clientSecret );

            var tokenResponse = api1.Authenticate();

            var apiData = api1.Root;
            // create a new instance of Api and reuse previously received token
            var api2 = new Api( url, clientKey, clientSecret );

            tokenResponse = api2.Authorize( tokenResponse );
            var createdUser = api2.Navigate( apiData.GetLink( "create-user" ), new UserData() );
            var user = api2.Navigate<UserData>( createdUser.Location );
            var problems = api2.Navigate<RoutingProblemDataSet>( user.GetLink( "list-problems" ) );
            var created = api2.Navigate( user.GetLink( "create-problem" ), new RoutingProblemUpdateRequest { Name = "test" } );
            var problem = api2.Navigate<RoutingProblemData>( created.Location );
            var problem2 = api2.Navigate<RoutingProblemData>( created.Location );
            
            CreateDemoData( problem, api2 );

            // refresh to get up to date set of operations
            problem = api2.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );

            var res = api2.Navigate<ResponseData>( problem.GetLink( "toggle-optimization" ), new RoutingProblemUpdateRequest { Name = problem.Name, State = "Running" } );
            RoutingProblemData rb = null;
            while ( true )
            {
                Thread.Sleep( 1000 );
                rb = api2.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );
                Console.WriteLine( "State: {0}", rb.State );
                if ( rb.State == "Running" || rb.Progress == 100 ) break;
            }

            while ( true )
            {
                Thread.Sleep( 1000 );

                int start = 0;
                int end = 1;
                var routingProblem = api2.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );
                var queryParameters = new Dictionary<string, string>
                                              {
                                                  {"Start", start.ToString() },
                                                  {"End", end.ToString() }
                                              };
                var objectiveValues = api2.Navigate<ObjectiveValueDataSet>( problem.GetLink( "objective-values" ), queryParameters );



                Console.WriteLine( routingProblem.State + " (" + routingProblem.Progress + "%)" );
                Console.WriteLine( "---------------" );
                foreach ( var obj in objectiveValues.Items )
                {

                    Console.WriteLine( "Objective values from {0} to {1}: [{2}] {3}", start, end, obj.TimeStamp, obj.Value );
                }
                Console.WriteLine( "---------------" );


                if ( routingProblem.State == "Stopped" )
                {
                    var resultVehicles = api2.Navigate<VehicleDataSet>( routingProblem.GetLink( "list-vehicles" ) );
                    var resultTasks = api2.Navigate<TaskDataSet>( routingProblem.GetLink( "list-tasks" ) );

                    foreach ( var vehicleData in resultVehicles.Items )
                    {
                        var veh = api2.Navigate<VehicleData>( vehicleData.GetLink( "self" ) );
                        Console.Write( "Vehicle {0}({1}): ", vehicleData.Id, vehicleData.Name );
                        var routeEvents = api2.Navigate<RouteEventDataSet>( veh.GetLink( "list-events" ) );
                        var sequence = api2.Navigate<RouteData>( veh.GetLink( "get-route" ) );

                        sequence.Items.Insert( 0, veh.StartLocation.Id );
                        sequence.Items.Add( veh.EndLocation.Id );

                        for ( int i = 0; i < routeEvents.Items.Count; i++ )
                        {
                            var point = sequence.Items[i];
                            var routeEvent = routeEvents.Items[i];

                            Console.WriteLine( "{0}: {1}-{2} ", point, routeEvent.PlannedArrivalTime, routeEvent.PlannedDepartureTime );
                        }
                        Console.WriteLine();
                    }
                    break;
                }
            }
        }

        private static void CreateDemoData( RoutingProblemData problem, Api api )
        {
            // To build a test case we first need to create a vehicle.
            // We start by defining vehicle capacity.
            var vehicleCapacities = new List<CapacityData> {new CapacityData() {Name = "Weight", Amount = 100000}};
            // ...the time window(s)
            var vehicleTimeWindow = new List<TimeWindowData> { new TimeWindowData { Start = new DateTime( 2013, 5, 14, 7, 0, 0 ), End = new DateTime( 2013, 5, 14, 16, 0, 0 ) }};
            // ... the locations for pickup and delivery
            var vehiclePickup = new LocationData() {Coordinate = new CoordinateData { Latitude = 62.244588, Longitude = 25.742683, System = "WGS84" }};
            var vehicleDelivery = new LocationData() {Coordinate = new CoordinateData { Latitude = 62.244588, Longitude = 25.742683, System = "WGS84" }};
            // And then we wrap these into a single vehicle update request.
            var vehicleUpdateRequest = new VehicleUpdateRequest()
                                           {
                                               Name = "Vehicle1",
                                               Capacities = vehicleCapacities,
                                               StartLocation = vehiclePickup,
                                               EndLocation = vehicleDelivery,
                                               TimeWindows = vehicleTimeWindow
                                           };


            api.Navigate<ResponseData>( problem.GetLink( "create-vehicle" ), vehicleUpdateRequest );
            // This should have created a vehicle.

            // Next, we will create a pickup and delivery task.
            // The task will consist of two task events that are the pickup and delivery.
            // Define the capacity, location and time window for the pickup.
            var capacity = new CapacityData { Name = "Weight", Amount = 1 };
            var task1PickupLocation = new LocationData
                                          {
                                              Coordinate = new CoordinateData
                                                               {
                                                                   Latitude = 62.247906,
                                                                   Longitude = 25.867395,
                                                                   System = "WGS84"
                                                               }
                                          };
            var task1PickupTimeWindows = new List<TimeWindowData> { new TimeWindowData { Start = new DateTime(2013, 5, 14, 7, 0, 0), End = new DateTime(2013, 5, 14, 16, 0, 0) } };
            //... and wrap it in a task event update request.
            var pickup = new TaskEventUpdateRequest
            {
                Type = "Pickup",
                Location = task1PickupLocation,
                TimeWindows = task1PickupTimeWindows,
                Capacities = new List<CapacityData>() {capacity}
            };


            // Then we do the same for the delivery.
            var task1DeliveryLocation = new LocationData
                                            {
                                                Coordinate = new CoordinateData
                                                                 {
                                                                     Latitude = 61.386909,
                                                                     Longitude = 24.654106,
                                                                     System = "WGS84"
                                                                 }
                                            };
            var task1DeliveryTimeWindows = new List<TimeWindowData> { new TimeWindowData { Start = new DateTime(2013, 5, 14, 7, 0, 0), End = new DateTime(2013, 5, 14, 16, 0, 0) } };

            var delivery = new TaskEventUpdateRequest
            {
                Type = "Delivery",
                Location = task1DeliveryLocation,
                TimeWindows = task1DeliveryTimeWindows,
                Capacities = new List<CapacityData>() { capacity }
            };


            // And finally we contain the pickup and delivery in a task update request and send it.
            var newTask = new TaskUpdateRequest() {Name = "Task1"};
            newTask.TaskEvents.Add( pickup );
            newTask.TaskEvents.Add( delivery );

            api.Navigate<ResponseData>( problem.GetLink( "create-task" ), newTask );

            // And this is how we can create optimization cases.
        }
    }
}