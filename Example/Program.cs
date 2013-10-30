using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using NFleetSDK;
using NFleetSDK.Data;

namespace NFleetExample
{
    public static class Program
    {
        private static string url = "";
        private static string username = "";
        private static string password = "";

        static Program()
        {
            url = ConfigurationManager.AppSettings["url"];
            username = ConfigurationManager.AppSettings["username"];
            password = ConfigurationManager.AppSettings["password"];
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
            

            var api1 = new Api( url, username, password );
            var tokenResponse = api1.Authenticate();

            var apiData = api1.Root;
            // create a new instance of Api and reuse previously received token
            var api2 = new Api( url, username, password );

            tokenResponse = api2.Authorize( tokenResponse );
            var users = api2.Navigate<UserDataSet>( apiData.GetLink( "list-users" ), new UserSetRequest() );
            var user = users.Items.Find( u => u.Id == 1 );
            var problems = api2.Navigate<RoutingProblemDataSet>( user.GetLink( "list-problems" ) );
            var created = api2.Navigate<ResponseData>( problems.GetLink( "create" ), new RoutingProblemUpdateRequest { Name = "test" } );
            var problem = api2.Navigate<RoutingProblemData>( created.Location );
            var problem2 = api2.Navigate<RoutingProblemData>( created.Location);
            CreateDemoData( problem, api2 );

            // refresh to get up to date set of operations
            problem = api2.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );

            var res = api2.Navigate<ResponseData>(problem.GetLink("update"), new RoutingProblemUpdateRequest { Name = problem.Name, State = "Running", VersionNumber = problem.VersionNumber } );
            RoutingProblemData rb = null;
            while (true)
            {
                Thread.Sleep( 1000 );
                rb = api2.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );
                Console.WriteLine("State: {0}", rb.State);
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
                    Console.WriteLine("---------------");
                    foreach ( var obj in objectiveValues.Items )
                    {
                        
                        Console.WriteLine( "Objective values from {0} to {1}: [{2}] {3}", start, end, obj.TimeStamp, obj.Value);
                    }
                    Console.WriteLine( "---------------" );


                    if ( routingProblem.State == "Stopped" )
                    {
                        var resultVehicles = api2.Navigate<VehicleDataSet>( routingProblem.GetLink( "list-vehicles" ) );
                        var resultTasks = api2.Navigate<TaskDataSet>( routingProblem.GetLink( "list-tasks" ) );

                        foreach ( var vehicleData in resultVehicles.Items )
                        {
                            var veh = api2.Navigate<VehicleData>(vehicleData.GetLink("self"));
                            Console.Write( "Vehicle {0}({1}): ", vehicleData.Id, vehicleData.Name );
                            var routeEvents = api2.Navigate<RouteEventDataSet>(veh.GetLink("list-events"));
                            var routeEvents2 = api2.Navigate<RouteEventDataSet>( veh.GetLink( "list-events" ) );
                            var sequence = api2.Navigate<RouteData>( veh.GetLink( "get-route" ) );

                            sequence.Items.Insert(0, veh.StartLocation.Id);
                            sequence.Items.Add( veh.EndLocation.Id );

                            for ( int i = 0; i < routeEvents.Items.Count; i++ )
                            {
                                var point = sequence.Items[i];
                                var routeEvent = routeEvents.Items[i];

                                Console.Write( "{0}: {1}-{2} ", point, routeEvent.PlannedArrivalTime, routeEvent.PlannedDepartureTime );
                            }
                            Console.WriteLine();
                        }
                        break;
                    }
                }
        }

        private static TaskEventData FindTaskEventData( TaskDataSet set, int id )
        {
            return set.Items.SelectMany( taskData => taskData.TaskEvents ).FirstOrDefault( taskEventData => taskEventData.Id == id );
        }

        private static void CreateDemoData( RoutingProblemData problem, Api api )
        {
            api.Navigate<ResponseData>( problem.GetLink( "create-vehicle" ), new VehicleUpdateRequest
            {
                Name = "test",
                Capacities = new List<CapacityData>
                {
                    new CapacityData { Name = "Weight", Amount = 100000 }
                },
                StartLocation = new LocationData
                {
                    Coordinate = new CoordinateData
                    {
                        Latitude = 62.244588,
                        Longitude = 25.742683,
                        System = "Euclidian"
                    }
                },
                EndLocation = new LocationData
                {
                    Coordinate = new CoordinateData
                    {
                        Latitude = 62.244588,
                        Longitude = 25.742683,
                        System = "Euclidian"
                    }
                },
                TimeWindows = { new TimeWindowData { Start = new DateTime( 2013, 5, 14, 7, 0, 0 ), End = new DateTime( 2013, 5, 14, 16, 0, 0 ) } }

            } );

            var newTask = new TaskUpdateRequest { Name = "task" };
            var capacity = new CapacityData { Name = "Weight", Amount = 1 };

            var pickup = new TaskEventUpdateRequest
            {
                Type = "Pickup",
                Location = new LocationData
                {
                    Coordinate = new CoordinateData
                    {
                        Latitude = 62.247906,
                        Longitude = 25.867395,
                        System = "Euclidian"
                    }
                },
                TimeWindows = { new TimeWindowData { Start = new DateTime( 2013, 5, 14, 7, 0, 0 ), End = new DateTime( 2013, 5, 14, 16, 0, 0 ) } }
            };
            pickup.Capacities.Add( capacity );
            newTask.TaskEvents.Add( pickup );

            var delivery = new TaskEventUpdateRequest
            {
                Type = "Delivery",
                Location = new LocationData
                {
                    Coordinate = new CoordinateData
                    {
                        Latitude = 61.386909,
                        Longitude = 24.654106,
                        System = "Euclidian"
                    }
                },
                TimeWindows = { new TimeWindowData { Start = new DateTime( 2013, 5, 14, 7, 0, 0 ), End = new DateTime( 2013, 5, 14, 16, 0, 0 ) } }
            };
            delivery.Capacities.Add( capacity );
            newTask.TaskEvents.Add( delivery );

            api.Navigate<ResponseData>( problem.GetLink( "create-task" ), newTask );
        }
    }
}