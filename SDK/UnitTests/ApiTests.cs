using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NFleetSDK.Data;
using Newtonsoft.Json;

namespace NFleetSDK.UnitTests
{
    [TestFixture]
    internal class ApiTests
    {
        //TODO: read these from some sane place
        private string username = (string)Properties.Settings.Default["apiusername"];
        private string password = (string)Properties.Settings.Default["apipassword"];
        private string apiLocation = (string)Properties.Settings.Default["apiurl"];
        private string responsePath =
            @"C:\Users\Markus Vuorio\Documents\Visual Studio 2010\Projects\DocGen\DocGen\CodeExamples\responses";
        
        private Api api;
        private Dictionary<string, object> testObjects = new Dictionary<string, object>();
        private Dictionary<string, Response> responses;
        
        [SetUp]
        public void Setup()
        {

            //##BEGIN EXAMPLE accessingapi##
            api = new Api(apiLocation, username, password);
            var rootLinks = api.Root;
            //##END EXAMPLE##

            //##BEGIN EXAMPLE oauth##
            var tokenResponse = api.Navigate<TokenData>(rootLinks.GetLink("authenticate"));
            //##END EXAMPLE##

            responses = ResponseReader.readResponses(responsePath);

            testObjects["rootLinks"] = rootLinks;
            testObjects["tokenResponse"] = tokenResponse;
        }

        [Test]
        public void T00RootlinkTest()
        {
            var rootLinks = (ApiData) testObjects["rootLinks"];
            var rootLinksJson = JsonConvert.SerializeObject(rootLinks);
            Assert.Equals(rootLinksJson, responses["accessingapiresp"].json);
        }

        [Test]
        public void T01CreatingProblemTest()
        {
            var rootLinks = (ApiData)testObjects["rootLinks"];
            //##BEGIN EXAMPLE creatingproblem##
            var problems = api.Navigate<RoutingProblemDataSet>(rootLinks.GetLink("list-problems"));
            var created = api.Navigate<ResponseData>(problems.GetLink("create"), new RoutingProblemUpdateRequest { Name = "test" });
            //##END EXAMPLE##
            Assert.AreEqual(created.Location, apiLocation + "/problems/1");
            testObjects["created"] = created;
            testObjects["problems"] = problems;
        }

        [Test]
        public void T02AccessingProblemTest()
        {
            var created = (ResponseData)testObjects["created"];
            //##BEGIN EXAMPLE accessingproblem##
            var problem = api.Navigate<RoutingProblemData>(created.Location);
            //##END EXAMPLE##
            var problemjson = JsonConvert.SerializeObject(problem);
            Assert.AreEqual(problem.Id, 1);
            Assert.AreEqual(problemjson, responses["accessingproblemresp"]);
            testObjects["problem"] = problem;
        }

        [Test]
        public void T03ListingTasksTest()
        {
            var problem = (RoutingProblemData) testObjects["problem"];
            TestData.CreateDemoData(problem, api);
            //##BEGIN EXAMPLE listingtasks##
            var tasks = api.Navigate<TaskDataSet>(problem.GetLink("list-tasks")); 
            //##END EXAMPLE##
            testObjects["tasks"] = tasks;
        }


        [Test]
        public void T04CreatingTaskTest()
        {
            var tasks = (TaskDataSet) testObjects["tasks"];
            //##BEGIN EXAMPLE creatingtask##
            var newTask = new TaskData { Name = "test name" };
            var capacity = new CapacityData { Name = "Weight", Amount = 20 };

            var pickup = new TaskEventData
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
            pickup.Capacities.Add(capacity);
            newTask.TaskEvents.Add(pickup);

            var delivery = new TaskEventData
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
            delivery.Capacities.Add(capacity);
            newTask.TaskEvents.Add(delivery);

            var taskCreationResult = api.Navigate<ResponseData>(tasks.GetLink("create"), newTask); 
            //##END EXAMPLE##
            testObjects["taskCreationResult"] = taskCreationResult;
        }

        [Test]
        public void T05UpdatingTaskTest()
        {
            var taskCreationResult = (ResponseData) testObjects["taskCreationResult"];
            //##BEGIN EXAMPLE updatingtask##
            var newTask = api.Navigate<TaskData>(taskCreationResult.Location);
            newTask.Name = "other name";
            api.Navigate<TaskData>(newTask.GetLink("update"), newTask); 
            //##END EXAMPLE##
            testObjects["newTask"] = newTask;
        }

        [Test]
        public void T06DeletingTaskTest()
        {
            var newTask = (TaskData) testObjects["newTask"];
            //##BEGIN EXAMPLE deletingtask##
            api.Navigate<ResponseData>(newTask.GetLink("delete"));
            //##END EXAMPLE##
        }

        [Test]
        public void T07ListingVehiclesTest()
        {
            var problem = (RoutingProblemData)testObjects["problem"];
            //##BEGIN EXAMPLE listingvehicles##
            var vehicles = api.Navigate<VehicleDataSet>(problem.GetLink("list-vehicles")); 
            //##END EXAMPLE##
            testObjects["vehicles"] = vehicles;
        }

        [Test]
        public void T08AccessingTaskSeqTest()
        {
            var vehicles = (VehicleDataSet)testObjects["vehicles"];
            var vehicle = vehicles.Items.First();
            //##BEGIN EXAMPLE accessingtaskseq##
            var taskEvents = api.Navigate<TaskEventDataSet>(vehicle.GetLink("list-events")); 
            //##END EXAMPLE##
        }

        public void T09AccessingRouteTest()
        {
            var vehicles = (VehicleDataSet)testObjects["vehicles"];
            var vehicle = vehicles.Items.First();
            //##BEGIN EXAMPLE accessingroute##
            var taskEvents = api.Navigate<TaskEventDataSet>(vehicle.GetLink("get-route")); 
            //##END EXAMPLE##
            testObjects["taskEvents"] = taskEvents;
        }

        [Test]
        public void T10UpdatingRouteTest()
        {
            var taskEvents = (TaskEventDataSet) testObjects["taskEvents"];
            
            //##BEGIN EXAMPLE updatingroute##
            /*var points =  8, 9, 12, 6 ;
            taskEvents.Items = points;
            api.Navigate(route.GetLink("update"), route);*/
            //##END EXAMPLE##
        }

        [Test]
        public void T11StartingOptTest()
        {
            var problem = (RoutingProblemData)testObjects["problem"];
            //##BEGIN EXAMPLE startingopt##
            var creation = api.Navigate<ResponseData>(problem.GetLink("start-new-optimization")); 
            //##END EXAMPLE##
            testObjects["creation"] = creation;
        }

        [Test]
        public void T12AccessingNewOptTest()
        {
            var creation = (ResponseData) testObjects["creation"];
            //##BEGIN EXAMPLE accessingnewopt##
            var optimization = api.Navigate<OptimizationData>(creation.Location); 
            //##END EXAMPLE##
            testObjects["optimization"] = optimization;
        }

        [Test]
        public void T13StoppingOptTest()
        {
            var optimization = (OptimizationData) testObjects["optimization"];
            //##BEGIN EXAMPLE stoppingopt##
            api.Navigate<ResponseData>(optimization.GetLink("stop")); 
            //##END EXAMPLE##
        }

        [Test]
        public void T14GetOptStatusTest()
        {
            var optimization = (OptimizationData)testObjects["optimization"];
            //##BEGIN EXAMPLE getoptstatus#
            optimization = api.Navigate<OptimizationData>(optimization.GetLink("self")); 
            //##END EXAMPLE##
        }

        [Test]
        public void T15BadRequestTest()
        {
            var problems = (RoutingProblemDataSet)testObjects["problems"];
            
            //##BEGIN EXAMPLE oauth##
            var result = api.Navigate<ResponseData>(problems.GetLink("create"));
            //##END EXAMPLE##
        }
    }
}
