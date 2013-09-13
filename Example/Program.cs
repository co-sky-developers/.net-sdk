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

            var link = new Link { Rel = "authenticate", Uri = "/tokens", Method = "POST" };
            var tokenResponse = api1.Navigate<TokenData>( link );

            // create a new instance of Api and reuse previously received token
            var api2 = new Api( url, username, password );

            tokenResponse = api2.Authorize( tokenResponse );

            var problems = api2.Navigate<RoutingProblemDataSet>( new Link { Rel = "list-problems", Uri = "/problems", Method = "GET" } );
            var created = api2.Navigate<ResponseData>( problems.GetLink( "create" ), new RoutingProblemUpdateRequest { Name = "test" } );
            var problem = api2.Navigate<RoutingProblemData>( created.Location );

            CreateDemoData( problem, api2 );

            // refresh to get up to date set of operations
            problem = api2.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );

            var locations = api2.Navigate<LocationDataSet>( problem.GetLink( "list-locations" ) );

            Console.WriteLine( locations.Items.Count );

            created = api2.Navigate<ResponseData>( problem.GetLink( "create-new-optimization" ) );
            var optimization = api2.Navigate<OptimizationData>( created.Location );
            var res = api2.Navigate<ResponseData>( optimization.GetLink( "start" ), new OptimizationUpdateRequest { } );

            // create multiple vehicles
            for ( int i = 0; i < 20; i++ )
            {
                api2.Navigate<ResponseData>( problem.GetLink( "create-vehicle" ), new VehicleUpdateRequest
                {
                    Name = "test"+i,
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
                    TimeWindows = { new TimeWindowData { Start = new DateTime( 2013, 5, 14, 8, 0, 0 ), End = new DateTime( 2013, 5, 14, 12, 0, 0 ) } }

                } );
            }
            var queryParams = new Dictionary<string, string>
                                  {
                                      {"limit","3"},
                                      {"offset","3"}
                                  };
            // navigate through vehicles using paging links
            var vehres = api2.Navigate<VehicleDataSet>( problem.GetLink( "list-vehicles" ), queryParams );
            var vehres2 = api2.Navigate<VehicleDataSet>( vehres.GetLink( "next" ) );
            var vehres3 = api2.Navigate<VehicleDataSet>( vehres2.GetLink( "next" ) );
            var vehres4 = api2.Navigate<VehicleDataSet>( vehres3.GetLink( "next" ) );


                while ( true )
                {
                    Thread.Sleep( 1000 );

                    optimization = api2.Navigate<OptimizationData>( optimization.GetLink( "self" ) );

                    Console.WriteLine( optimization.State + " (" + optimization.Progress + "%)" );

                    if ( optimization.State == "Stopped" )
                    {
                        var optimizationResult = api2.Navigate<VehicleDataSet>( optimization.GetLink( "results" ) );
                        var optimizationResulTasks = api2.Navigate<TaskDataSet>( optimization.GetLink( "resulttasks" ) );
                        foreach ( var vehicleData in optimizationResult.Items )
                        {
                            Console.Write( "Vehicle {0}({1}): ", vehicleData.Id, vehicleData.Name );
                            foreach ( var point in vehicleData.Route.Items )
                            {
                                TaskEventData data = FindTaskEventData( optimizationResulTasks, point );
                                Console.Write( "{0}: {1}-{2} ", point, data.PlannedArrivalTime, data.PlannedDepartureTime );
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
                TimeWindows = { new TimeWindowData { Start = new DateTime( 2013, 5, 14, 8, 0, 0 ), End = new DateTime( 2013, 5, 14, 12, 0, 0 ) } }

            } );

            var newTask = new TaskUpdateRequest { Name = "task" };
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
                TimeWindows = { new TimeWindowData { Start = new DateTime( 2013, 5, 14, 8, 0, 0 ), End = new DateTime( 2013, 5, 14, 12, 0, 0 ) } }
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
                        Latitude = 62.373658,
                        Longitude = 25.885506,
                        System = "Euclidian"
                    }
                },
                TimeWindows = { new TimeWindowData { Start = new DateTime( 2013, 5, 14, 8, 0, 0 ), End = new DateTime( 2013, 5, 14, 12, 0, 0 ) } }
            };
            delivery.Capacities.Add( capacity );
            newTask.TaskEvents.Add( delivery );

            api.Navigate<ResponseData>( problem.GetLink( "create-task" ), newTask );
        }
    }
}