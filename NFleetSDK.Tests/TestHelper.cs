using System.Configuration;
using NFleet.Data;

namespace NFleet.Tests
{
    public static class TestHelper
    {
        private static readonly string url = ConfigurationManager.AppSettings["url"];
        private static readonly string clientKey = ConfigurationManager.AppSettings["client-key"];
        private static readonly string clientSecret = ConfigurationManager.AppSettings["client-secret"];

        internal static Api Authenticate()
        {
            var api = new Api( url, clientKey, clientSecret );
            api.Authenticate();
            return api;
        }

        internal static UserData GetOrCreateUser( Api api )
        {
            var users = api.Navigate<EntityLinkCollection>( api.Root.GetLink( "list-users" ) );
            var userExists = users.Items.Exists( u => u.Id == 1 );
            UserData user = null;
            if ( !userExists )
            {
                var createdUser = api.Navigate( api.Root.GetLink( "create-user" ), new UserData() );
                user = api.Navigate<UserData>( createdUser.Location );
            }
            else
            {
                var entityLink = users.Items.Find(u => u.Id == 1);
                user = api.Navigate<UserData>( entityLink.GetLink( "self" ) );
            }            
            return user;
        }

        public static RoutingProblemData CreateProblem( Api api, UserData user )
        {
            var created = api.Navigate( user.GetLink( "create-problem" ), new RoutingProblemUpdateRequest { Name = "test" } );
            var problem = api.Navigate<RoutingProblemData>( created.Location );
            return problem;
        }

        public static RoutingProblemData CreateProblemWithDemoData( Api api, UserData user )
        {
            var p = CreateProblem( api, user );
            TestData.CreateDemoData( p, api );
            return p;
        }

        public static VehicleData GetVehicle( Api api, UserData user, RoutingProblemData problem )
        {
            TestData.CreateDemoData( problem, api );
            var vehicles = api.Navigate<EntityLinkCollection>( problem.GetLink( "list-vehicles" ) );
            var vehicleLink = vehicles.Items.Find( v => v.Id == 1 );
            var vehicle = api.Navigate<VehicleData>(vehicleLink.GetLink("self"));
            return vehicle;
        }

        internal static TaskData GetTask( Api api, RoutingProblemData problem )
        {
            var tasks = api.Navigate<EntityLinkCollection>( problem.GetLink( "list-tasks" ) );
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

            var taskCreationResult = api.Navigate<ResponseData>( problem.GetLink( "create-task" ), newTask );
            var task = api.Navigate<TaskData>( taskCreationResult.Location );
            return task;
        }
    }
}