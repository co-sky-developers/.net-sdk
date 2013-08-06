using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NFleetSDK;
using NFleetSDK.Data;

namespace NFleetExample
{
    public static class Program
    {
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
            var api = new Api( "url" );
            api.Authenticate( "username", "secret" );
            var apiData = api.Root;

            var problems = api.Navigate<RoutingProblemDataSet>( apiData.GetLink( "list-problems" ) );
            var created = api.Navigate<ResponseData>( problems.GetLink( "create" ), new RoutingProblemUpdateRequestData { Name = "test" } );
            var problem = api.Navigate<RoutingProblemData>( created.Location );

            CreateDemoData( problem, api );

            // refresh to get up to date set of operations
            problem = api.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );

            created = api.Navigate<ResponseData>( problem.GetLink( "start-new-optimization" ) );
            var optimization = api.Navigate<OptimizationData>( created.Location );

            while ( true )
            {
                Thread.Sleep( 1000 );

                optimization = api.Navigate<OptimizationData>( optimization.GetLink( "self" ) );

                Console.WriteLine( optimization.State );

                if ( optimization.State == "Stopped" )
                {
                    var optimizationResult = api.Navigate<VehicleDataSet>( optimization.GetLink( "results" ) );
                    foreach ( var vehicleData in optimizationResult.Items )
                    {
                        Console.Write( "Vehicle {0}: ", vehicleData.Id );
                        foreach ( var point in vehicleData.Route.Items )
                        {
                            Console.Write( "{0} ", point );
                        }
                        Console.WriteLine();
                    }
                    break;
                }
            }
        }

        private static void CreateDemoData( RoutingProblemData problem, Api api )
        {
            api.Navigate<ResponseData>( problem.GetLink( "create-vehicle" ), new VehicleUpdateRequestData
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
                }
            } );

            var newTask = new TaskUpdateRequestData { Name = "task" };
            var capacity = new CapacityData { Name = "Weight", Amount = 20 };

            var pickup = new TaskEventUpdateRequestData
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
                }
            };
            pickup.Capacities.Add( capacity );
            newTask.TaskEvents.Add( pickup );

            var delivery = new TaskEventUpdateRequestData
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
                }
            };
            delivery.Capacities.Add( capacity );
            newTask.TaskEvents.Add( delivery );

            api.Navigate<ResponseData>( problem.GetLink( "create-task" ), newTask );
        }
    }
}