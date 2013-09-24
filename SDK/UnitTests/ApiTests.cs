using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Assert.IsNotNullOrEmpty(username);
            Assert.IsNotNullOrEmpty(password);
            Assert.IsNotNullOrEmpty(apiLocation);
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
            Trace.Write(JsonConvert.SerializeObject(rootLinks));
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
            
            Trace.Write(JsonConvert.SerializeObject(created));

            TestUtils.RoutingProblemsAreEqual( mockCreated, createdProblemData );
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
            Trace.Write(JsonConvert.SerializeObject(problem));
            TestUtils.RoutingProblemsAreEqual(mockProblem, problem );
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
            Trace.Write(JsonConvert.SerializeObject(tasks));
            TestUtils.TaskDataSetsAreEqual( mockTasks, tasks );
        }


        [Test]
        public void T04CreatingTaskTest()
        {
            var tasks = (TaskDataSet) testObjects["tasks"];
            //##BEGIN EXAMPLE creatingtask##
            var newTask = new TaskUpdateRequest { Name = "test name" };
            var capacity = new CapacityData { Name = "Weight", Amount = 20 };

            var pickup = new TaskEventUpdateRequest{
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
            delivery.Capacities.Add(capacity);
            newTask.TaskEvents.Add(delivery);

            var taskCreationResult = api.Navigate<ResponseData>(tasks.GetLink("create"), newTask); 
            //##END EXAMPLE##
            testObjects["taskCreationResult"] = taskCreationResult;

            var mockTaskCreationResult = TestUtils.GetMockResponse<ResponseData>(responses["creatingtaskresp"].json);

            TestUtils.ResponsesAreEqual( mockTaskCreationResult, taskCreationResult );
            Trace.Write(JsonConvert.SerializeObject(taskCreationResult));
        }

        [Test]
        public void T05UpdatingTaskTest()
        {
            var taskCreationResult = (ResponseData) testObjects["taskCreationResult"];
            var t = api.Navigate<TaskData>(taskCreationResult.Location);
            var oldTaskEvents = new List<TaskEventUpdateRequest>();

            foreach (var te in t.TaskEvents)
            {
                var teReq = new TaskEventUpdateRequest
                                {
                                    Capacities = te.Capacities,
                                    Location = te.Location,
                                    PlannedArrivalTime = te.PlannedArrivalTime,
                                    PlannedDepartureTime = te.PlannedDepartureTime,
                                    ServiceTime = te.ServiceTime,
                                    TaskEventId = te.Id,
                                    TimeWindows = te.TimeWindows,
                                    Type = te.Type

                                };
                oldTaskEvents.Add( teReq );
            }

            //##BEGIN EXAMPLE updatingtask##
            var oldTask = api.Navigate<TaskData>(taskCreationResult.Location);
            var newTaskRequest = new TaskUpdateRequest
                                     {
                                         Info = oldTask.Info,
                                         Name = "Other name",
                                         TaskEvents = oldTaskEvents,
                                         TaskId = oldTask.Id,
                                     };
            var newTaskLocation = api.Navigate<ResponseData>( oldTask.GetLink( "update" ), newTaskRequest ); 
            //##END EXAMPLE##
            var newTask = api.Navigate<TaskData>( newTaskLocation.Location );
            testObjects["newTask"] = newTask;

            var mockNewTask = TestUtils.GetMockResponse<TaskData>(responses["updatingtaskresp"].json);
            Trace.Write( JsonConvert.SerializeObject( newTask ) );
            TestUtils.TasksAreEqual( mockNewTask, newTask );
        }

        [Test]
        public void T06DeletingTaskTest()
        {
            Assert.True(false); 
            /*var newTask = (TaskData) testObjects["newTask"];
            //##BEGIN EXAMPLE deletingtask##
            var deleteResponse = api.Navigate<ResponseData>(newTask.GetLink("delete"));
            //##END EXAMPLE##

            var mockDeleteResponse = TestUtils.GetMockResponse<ResponseData>(responses["deletingtaskresp"].json);
            Trace.Write(JsonConvert.SerializeObject(deleteResponse));
            TestUtils.ResponsesAreEqual( mockDeleteResponse, deleteResponse );*/
        }

        [Test]
        public void T07ListingVehiclesTest()
        {
            var problem = (RoutingProblemData)testObjects["problem"];
            var vehicle = new VehicleUpdateRequest
            {
                Name = "vehicle 2",
                Capacities = new List<CapacityData>
                {
                    new CapacityData { Name = "Weight", Amount = 3500 }
                },
                StartLocation = new LocationData
                {
                    Coordinate = new CoordinateData
                    {
                        Latitude = 61.244958,
                        Longitude = 20.747143,
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
                TimeWindows = { new TimeWindowData { Start = new DateTime(2013, 5, 14, 8, 0, 0), End = new DateTime(2013, 5, 14, 12, 0, 0) } }

            };
            api.Navigate<ResponseData>( problem.GetLink( "create-vehicle" ), vehicle );
            //##BEGIN EXAMPLE listingvehicles##
            var vehicles = api.Navigate<VehicleDataSet>(problem.GetLink("list-vehicles")); 
            //##END EXAMPLE##
            testObjects["vehicles"] = vehicles;

            var mockVehicles = TestUtils.GetMockResponse<VehicleDataSet>(responses["listingvehiclesresp"].json);
            Trace.Write(JsonConvert.SerializeObject(vehicles));
            TestUtils.VehicleDataSetsAreEqual( mockVehicles, vehicles );
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
            Trace.Write(JsonConvert.SerializeObject(taskEvents));
            TestUtils.TaskEventDataSetsAreEqual( mockTaskEvents, taskEvents );
        }

        [Test]
        public void T09AccessingRouteTest()
        {
            var vehicles = (VehicleDataSet)testObjects["vehicles"];
            var vehicle = vehicles.Items.First();
            //##BEGIN EXAMPLE accessingroute##
            var taskEvents = api.Navigate<TaskEventDataSet>(vehicle.GetLink("get-route")); 
            //##END EXAMPLE##
            Trace.Write(JsonConvert.SerializeObject(taskEvents));
            var mockTaskEvents = TestUtils.GetMockResponse<TaskEventDataSet>(responses["acessingrouteresp"].json);
            TestUtils.TaskEventDataSetsAreEqual( mockTaskEvents, taskEvents );
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
            Trace.Write(JsonConvert.SerializeObject(creation));
            TestUtils.ResponsesAreEqual( mockCreation, creation );
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
            Trace.Write(JsonConvert.SerializeObject(optimization));
            TestUtils.OptimizationsAreEqual( mockOptimization, optimization );
        }

        [Test]
        public void T13StoppingOptTest()
        {
            var optimization = (OptimizationData) testObjects["optimization"];
            //##BEGIN EXAMPLE stoppingopt##
            var response = api.Navigate<ResponseData>(optimization.GetLink("stop")); 
            //##END EXAMPLE##
            var mockResponse = TestUtils.GetMockResponse<ResponseData>(responses["stoppingoptresp"].json);
            Trace.Write(JsonConvert.SerializeObject(response));
            TestUtils.ResponsesAreEqual( mockResponse, response );
        }

        [Test]
        public void T14GetOptStatusTest()
        {
            var optimization = (OptimizationData)testObjects["optimization"];
            //##BEGIN EXAMPLE getoptstatus#
            var optimizationResult = api.Navigate<OptimizationData>(optimization.GetLink("self")); 
            //##END EXAMPLE##
            var mockOptimizationResult = TestUtils.GetMockResponse<OptimizationData>(responses["getoptstatusresp"].json);
            Trace.Write(JsonConvert.SerializeObject(optimizationResult));
            TestUtils.OptimizationsAreEqual( mockOptimizationResult, optimizationResult );
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
