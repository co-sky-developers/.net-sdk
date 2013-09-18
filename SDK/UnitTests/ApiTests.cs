using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NFleetSDK.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace NFleetSDK.UnitTests
{
    [TestFixture]
    internal class ApiTests
    {
        //TODO: read these from some sane place
        private string username = (string)Properties.Settings.Default["apiusername"];
        private string password = (string)Properties.Settings.Default["apipassword"];
        private string apiLocation = (string)Properties.Settings.Default["apiurl"];
        private string responsePath = (string) Properties.Settings.Default["responsepath"];
        
        private Api api;
        private Dictionary<string, object> testObjects = new Dictionary<string, object>();
        private Dictionary<string, Response> responses;
        private RestSharp.Deserializers.JsonDeserializer deserializer;
        
        [SetUp]
        public void Setup()
        {

            //##BEGIN EXAMPLE accessingapi##
            api = new Api(apiLocation, username, password);
            var tokenResponse = api.Authenticate();
            var rootLinks = api.Root;
            //##END EXAMPLE##

            //##BEGIN EXAMPLE oauth##
            //Fail
            //##END EXAMPLE##

            responses = ResponseReader.readResponses(responsePath);
            deserializer = new RestSharp.Deserializers.JsonDeserializer();
            TestUtils.deserializer = deserializer;

            testObjects["rootLinks"] = rootLinks;
            testObjects["tokenResponse"] = tokenResponse;
        }

        [Test]
        public void T00RootlinkTest()
        {
            var rootLinks = (ApiData) testObjects["rootLinks"];
            var mockRootLinks = TestUtils.GetMockResponse<ApiData>(responses["accessingapiresp"].json);
            TestUtils.ListsAreEqual<Link>(rootLinks.Meta, mockRootLinks.Meta, TestUtils.LinksAreEqual);
        }

        [Test]
        public void T01CreatingProblemTest()
        {
            var rootLinks = (ApiData)testObjects["rootLinks"];
            //##BEGIN EXAMPLE creatingproblem##
            var problems = api.Navigate<RoutingProblemDataSet>(rootLinks.GetLink("list-problems"));
            var created = api.Navigate<ResponseData>(problems.GetLink("create"), new RoutingProblemUpdateRequest { Name = "test" });
            var createdProblemData = api.Navigate<RoutingProblemData>(created.Location);
            //##END EXAMPLE##
            testObjects["created"] = created;
            testObjects["problems"] = problems;
            
            var mockCreated = TestUtils.GetMockResponse<RoutingProblemData>(responses["accessingnewproblemresp"].json);
            
            TestUtils.RoutingProblemsAreEqual(createdProblemData, mockCreated);
        }

        [Test]
        public void T02AccessingProblemTest()
        {
            var created = (ResponseData)testObjects["created"];
            //##BEGIN EXAMPLE accessingproblem##
            var problem = api.Navigate<RoutingProblemData>(created.Location);
            //##END EXAMPLE##
            testObjects["problem"] = problem;

            var mockProblem = TestUtils.GetMockResponse<RoutingProblemData>(responses["accessingproblemresp"].json);
            TestUtils.RoutingProblemsAreEqual(problem, mockProblem);
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

            var mockTasks = TestUtils.GetMockResponse<TaskDataSet>(responses["listingtasksresp"].json);
            TestUtils.TaskDataSetsAreEqual(tasks, mockTasks);
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

            var mockTaskCreationResult = TestUtils.GetMockResponse<ResponseData>(responses["creatingtaskresp"].json);

            TestUtils.ResponsesAreEqual(taskCreationResult, mockTaskCreationResult);
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

            var mockNewTask = TestUtils.GetMockResponse<TaskData>(responses["updatingtaskresp"].json);

            TestUtils.TasksAreEqual(newTask, mockNewTask);
        }

        [Test]
        public void T06DeletingTaskTest()
        {
            var newTask = (TaskData) testObjects["newTask"];
            //##BEGIN EXAMPLE deletingtask##
            var deleteResponse = api.Navigate<ResponseData>(newTask.GetLink("delete"));
            //##END EXAMPLE##

            var mockDeleteResponse = TestUtils.GetMockResponse<ResponseData>(responses["deletingtaskresp"].json);

            TestUtils.ResponsesAreEqual(deleteResponse, mockDeleteResponse);
        }

        [Test]
        public void T07ListingVehiclesTest()
        {
            var problem = (RoutingProblemData)testObjects["problem"];
            //##BEGIN EXAMPLE listingvehicles##
            var vehicles = api.Navigate<VehicleDataSet>(problem.GetLink("list-vehicles")); 
            //##END EXAMPLE##
            testObjects["vehicles"] = vehicles;

            var mockVehicles = TestUtils.GetMockResponse<VehicleDataSet>(responses["listingvehiclesresp"].json);
            TestUtils.ListsAreEqual(vehicles.Meta, mockVehicles.Meta, TestUtils.LinksAreEqual);
            TestUtils.VehicleDataSetsAreEqual(vehicles, mockVehicles);
        }

        [Test]
        public void T08AccessingTaskSeqTest()
        {
            var vehicles = (VehicleDataSet)testObjects["vehicles"];
            var vehicle = vehicles.Items.First();
            //##BEGIN EXAMPLE accessingtaskseq##
            var taskEvents = api.Navigate<TaskEventDataSet>(vehicle.GetLink("list-events")); 
            //##END EXAMPLE##
            var mockTaskEvents = TestUtils.GetMockResponse<TaskEventDataSet>(responses["accessingtaskseqresp"].json);
            TestUtils.TaskEventDataSetsAreEqual(taskEvents, mockTaskEvents);
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
            var mockCreation = TestUtils.GetMockResponse<ResponseData>(responses["startingoptresp"].json);

            TestUtils.ResponsesAreEqual(creation, mockCreation);
        }

        [Test]
        public void T12AccessingNewOptTest()
        {
            var creation = (ResponseData) testObjects["creation"];
            //##BEGIN EXAMPLE accessingnewopt##
            var optimization = api.Navigate<OptimizationData>(creation.Location); 
            //##END EXAMPLE##
            testObjects["optimization"] = optimization;

            var mockOptimization = TestUtils.GetMockResponse<OptimizationData>(responses["accessingnewoptresp"].json);
            TestUtils.OptimizationsAreEqual(optimization, mockOptimization);
        }

        [Test]
        public void T13StoppingOptTest()
        {
            var optimization = (OptimizationData) testObjects["optimization"];
            //##BEGIN EXAMPLE stoppingopt##
            var response = api.Navigate<ResponseData>(optimization.GetLink("stop")); 
            //##END EXAMPLE##
            var mockResponse = TestUtils.GetMockResponse<ResponseData>(responses["stoppingoptresp"].json);
            TestUtils.ResponsesAreEqual(response, mockResponse);
        }

        [Test]
        public void T14GetOptStatusTest()
        {
            var optimization = (OptimizationData)testObjects["optimization"];
            //##BEGIN EXAMPLE getoptstatus#
            var optimizationResult = api.Navigate<OptimizationData>(optimization.GetLink("self")); 
            //##END EXAMPLE##
            var mockOptimizationResult = TestUtils.GetMockResponse<OptimizationData>(responses["getoptstatusresp"].json);
            TestUtils.OptimizationsAreEqual(optimizationResult, mockOptimizationResult);
        }

        [Test]
        public void T15BadRequestTest()
        {
            var problems = (RoutingProblemDataSet)testObjects["problems"];
            try
            {
                //##BEGIN EXAMPLE oauth##
                var result = api.Navigate<ResponseData>(problems.GetLink("create"));
                //##END EXAMPLE##
            }
            catch (System.IO.IOException e)
            {
                Assert.IsTrue(e.Message.Contains("400 Bad Request"));
            }
        }
    }
}
