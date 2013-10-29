﻿using NFleetSDK.Data;

namespace NFleetSDK.UnitTests
{
    public class TestHelper
    {
        private static string username = (string)Properties.Settings.Default["apiusername"];
        private static string password = (string)Properties.Settings.Default["apipassword"];
        private static string apiLocation = (string)Properties.Settings.Default["apiurl"];

        internal static Api Authenticate()
        {
            var api = new Api( apiLocation, username, password );
            var tokenResponse = api.Authenticate();

            return api;
        }

        internal static UserData GetUser( Api api )
        {
            var rootLinks = api.Root;
            var users = api.Navigate<UserDataSet>( rootLinks.GetLink( "list-users" ), new UserSetRequest() );
            var user = users.Items.Find( u => u.Id == 1 );
            return user;
        }

        public static RoutingProblemData CreateProblem(Api api, UserData user)
        {
            var problems = api.Navigate<RoutingProblemDataSet>( user.GetLink( "list-problems" ) );
            var created = api.Navigate<ResponseData>( problems.GetLink( "create" ), new RoutingProblemUpdateRequest { Name = "test" } );
            var problem = api.Navigate<RoutingProblemData>( created.Location );
            return problem;
        }

        public static RoutingProblemData CreateProblemWithDemoData( Api api, UserData user )
        {
            var p = CreateProblem(api, user);
            TestData.CreateDemoData(p, api);
            return p;
        }

        public static VehicleData GetVehicle(Api api, UserData user, RoutingProblemData problem)
        {
            TestData.CreateDemoData( problem, api );
            var vehicles = api.Navigate<VehicleDataSet>( problem.GetLink( "list-vehicles" ) ); 
            var vehicle = vehicles.Items.Find( v => v.Id == 1 );

            return vehicle;
        }

        internal static TaskData GetTask( Api api, RoutingProblemData problem )
        {
            var tasks = api.Navigate<TaskDataSet>( problem.GetLink( "list-tasks" ) );
            var newTask = new TaskUpdateRequest { Name = "test name" };
            var capacity = new CapacityData { Name = "Weight", Amount = 20 };

            var pickup = new TaskEventUpdateRequest
            {
                Type = "Pickup",
                Location = new LocationData
                {
                    Coordinate = new CoordinateData
                    {
                        Latitude = 62.244958,
                        Longitude = 25.747143,
                        System = "Euclidian"
                    }
                }
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
                        Latitude = 62.244589,
                        Longitude = 25.74892,
                        System = "Euclidian"
                    }
                }
            };
            delivery.Capacities.Add( capacity );
            newTask.TaskEvents.Add( delivery );

            var taskCreationResult = api.Navigate<ResponseData>( tasks.GetLink( "create" ), newTask );
            var task = api.Navigate<TaskData>( taskCreationResult.Location );
            return task;
        }
    }
}