using NFleetSDK.Data;

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
    }
}
