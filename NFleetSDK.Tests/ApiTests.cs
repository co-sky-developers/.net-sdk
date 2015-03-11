using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NFleet.Data;
using NUnit.Framework;
using Newtonsoft.Json;
using RestSharp.Deserializers;

namespace NFleet.Tests
{
    [TestFixture]
    internal class ApiTests
    {
        private Dictionary<string, Response> responses;
        private JsonDeserializer deserializer;

        [SetUp]
        public void Setup()
        {
            string url = ConfigurationManager.AppSettings["url"];
            string clientKey = ConfigurationManager.AppSettings["client-key"];
            string clientSecret = ConfigurationManager.AppSettings["client-secret"];
            string responsePath = ConfigurationManager.AppSettings["response-path"];

            Assert.IsNotNullOrEmpty(clientKey);
            Assert.IsNotNullOrEmpty(clientSecret);
            Assert.IsNotNullOrEmpty(url);
            // ReSharper disable UnusedVariable
            //##BEGIN EXAMPLE accessingapi##
            var api = new Api(url, clientKey, clientSecret);
            var tokenResponse = api.Authenticate();
            var rootLinks = api.Root;
            //##END EXAMPLE##
            // ReSharper restore UnusedVariable


            responses = ResponseReader.readResponses(responsePath);
            deserializer = new JsonDeserializer();
            TestUtils.Deserializer = deserializer;
             

            
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            string url = ConfigurationManager.AppSettings["url"];
            string clientKey = ConfigurationManager.AppSettings["client-key"];
            string clientSecret = ConfigurationManager.AppSettings["client-secret"];
            string responsePath = ConfigurationManager.AppSettings["response-path"];

            var api = new Api( url, clientKey, clientSecret );
            //##BEGIN EXAMPLE oauth##
            var tokenResponse = api.Authenticate();
            //##END EXAMPLE##
            var rootLinks = api.Root;

            var users = api.Navigate<UserDataSet>( rootLinks.GetLink( "list-users" ) );

            foreach ( var user in users.Items )
            {
                var u = api.Navigate<UserData>( user.GetLink( "self" ) );
                api.Navigate<ResponseData>( u.GetLink( "delete-user" ) );
            }
        }

        [Test]
        public void T00RootlinkTest()
        {
            var api = TestHelper.Authenticate();
            var rootLinks = api.Root;
            var mockRootLinks = TestUtils.GetMockResponse<ApiData>(responses["accessingapiresp"].json);
            Trace.Write(JsonConvert.SerializeObject(rootLinks));
            TestUtils.ListsAreEqual<Link>(rootLinks.Meta, mockRootLinks.Meta, TestUtils.LinksAreEqual);
        }

        [Test]
        public void T01CreatingProblemTest()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            //##BEGIN EXAMPLE creatingproblem##
            var created = api.Navigate(user.GetLink("create-problem"), new RoutingProblemUpdateRequest {Name = "test"});
            var problem = api.Navigate<RoutingProblemData>(created.Location);
            //##END EXAMPLE##

            var mockCreated = TestUtils.GetMockResponse<RoutingProblemData>(responses["accessingnewproblemresp"].json);

            Trace.Write(JsonConvert.SerializeObject(created));

            TestUtils.RoutingProblemsAreEqual(mockCreated, problem);
        }

        [Test]
        public void T02AccessingProblemTest()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var created = api.Navigate(user.GetLink("create-problem"), new RoutingProblemUpdateRequest {Name = "test"});
            //##BEGIN EXAMPLE accessingproblem##
            var problem = api.Navigate<RoutingProblemData>(created.Location);
            //##END EXAMPLE##

            //##BEGIN EXAMPLE accessingnewproblem##
            problem = api.Navigate<RoutingProblemData>(created.Location);
            //##END EXAMPLE##

            var mockProblem = TestUtils.GetMockResponse<RoutingProblemData>(responses["accessingproblemresp"].json);
            Trace.Write(JsonConvert.SerializeObject(problem));
            TestUtils.RoutingProblemsAreEqual(mockProblem, problem);
        }

        [Test]
        public void T03ListingTasksTest()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);

            //##BEGIN EXAMPLE listingtasks##
            var tasks = api.Navigate<TaskDataSet>(problem.GetLink("list-tasks"));
            //##END EXAMPLE##

            var mockTasks = TestUtils.GetMockResponse<TaskDataSet>(responses["listingtasksresp"].json);
            Trace.Write(JsonConvert.SerializeObject(tasks));
            TestUtils.TaskDataSetsAreEqual(mockTasks, tasks);
        }


        [Test]
        public void T04CreatingTaskTest()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);
            var tasks = api.Navigate<TaskDataSet>(problem.GetLink("list-tasks"));
            //##BEGIN EXAMPLE creatingtask##
            var newTask = new TaskUpdateRequest { Name = "test name", RelocationType = "None", ActivityState = "Active" };
            var capacity = new CapacityData {Name = "Weight", Amount = 20};

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
                },
                TimeWindows = { new TimeWindowData { Start = new DateTime( 2013, 5, 14, 8, 0, 0 ), End = new DateTime( 2013, 5, 14, 12, 0, 0 ) } }
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
                },
                TimeWindows = { new TimeWindowData { Start = new DateTime( 2013, 5, 14, 8, 0, 0 ), End = new DateTime( 2013, 5, 14, 12, 0, 0 ) } }
            };
            delivery.Capacities.Add(capacity);
            newTask.TaskEvents.Add(delivery);

            var taskCreationResult = api.Navigate<ResponseData>(problem.GetLink("create-task"), newTask);
            //##END EXAMPLE##

            var mockTaskCreationResult = TestUtils.GetMockResponse<ResponseData>(responses["creatingtaskresp"].json);

            TestUtils.ResponsesAreEqual(mockTaskCreationResult, taskCreationResult);
            Trace.Write(JsonConvert.SerializeObject(taskCreationResult));
        }

        [Test]
        public void T05UpdatingTaskTest()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);
            var task = TestHelper.GetTask(api, problem);
            var oldTaskEvents = new List<TaskEventUpdateRequest>();

            foreach (var te in task.TaskEvents)
            {
                var teReq = new TaskEventUpdateRequest
                {
                    Capacities = te.Capacities,
                    Location = te.Location,
                    ServiceTime = te.ServiceTime,
                    TaskEventId = te.Id,
                    TimeWindows = te.TimeWindows,
                    Type = te.Type,
                };
                oldTaskEvents.Add(teReq);
            }

            //##BEGIN EXAMPLE updatingtask##
            var newTaskRequest = new TaskUpdateRequest
            {
                Info = task.Info,
                Name = "Other name",
                TaskEvents = oldTaskEvents,
                TaskId = task.Id,
                RelocationType = task.RelocationType,
                ActivityState = task.ActivityState
            };
            var newTaskLocation = api.Navigate<ResponseData>(task.GetLink("update"), newTaskRequest);
            //##END EXAMPLE##
            var newTask = api.Navigate<TaskData>(newTaskLocation.Location);

            Assert.AreEqual(newTaskRequest.Name, newTask.Name);
        }



        [Test]
        public void T07ListingVehiclesTest()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);

            var vehicle = new VehicleUpdateRequest
            {
                Name = "vehicle 2",
                RelocationType = "None",
                Capacities = new List<CapacityData>
                {
                    new CapacityData {Name = "Weight", Amount = 3500}
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
                TimeWindows =
                {
                    new TimeWindowData
                    {
                        Start = new DateTime(2013, 5, 14, 8, 0, 0),
                        End = new DateTime(2013, 5, 14, 12, 0, 0)
                    }
                }

            };
            api.Navigate<ResponseData>(problem.GetLink("create-vehicle"), vehicle);
            //##BEGIN EXAMPLE listingvehicles##
            var vehicles = api.Navigate<VehicleDataSet>(problem.GetLink("list-vehicles"));
            //##END EXAMPLE##

            var mockVehicles = TestUtils.GetMockResponse<VehicleDataSet>(responses["listingvehiclesresp"].json);
            Trace.Write(JsonConvert.SerializeObject(vehicles));
            TestUtils.VehicleDataSetsAreEqual(mockVehicles, vehicles);
        }

        [Test]
        public void T08AccessingTaskSeqTest()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);
            var vehicle = TestHelper.GetVehicle(api, user, problem);

            api.Navigate<RouteData>(vehicle.GetLink("get-route"));
            var routeReq = new RouteUpdateRequest
            {
                Items = new[] {11, 12}
            };
            api.Navigate<ResponseData>(vehicle.GetLink("set-route"), routeReq);
            //##BEGIN EXAMPLE accessingtaskseq##
            var taskEvents = api.Navigate<TaskEventDataSet>(vehicle.GetLink("list-events"));
            //##END EXAMPLE##
            var mockTaskEvents = TestUtils.GetMockResponse<TaskEventDataSet>(responses["accessingtaskseqresp"].json);
            Trace.Write(JsonConvert.SerializeObject(taskEvents));
            TestUtils.TaskEventDataSetsAreEqual(mockTaskEvents, taskEvents);
        }

        [Test]
        public void T09AccessingRouteTest()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);
            var vehicle = TestHelper.GetVehicle(api, user, problem);
            api.Navigate<RouteData>(vehicle.GetLink("get-route"));
            var routeReq = new RouteUpdateRequest
            {
                Items = new[] {11, 12}
            };
            api.Navigate<ResponseData>(vehicle.GetLink("set-route"), routeReq);

            //##BEGIN EXAMPLE accessingroute##
            var route = api.Navigate<RouteData>(vehicle.GetLink("get-route"));
            //##END EXAMPLE##
            Trace.Write(JsonConvert.SerializeObject(route));
            var mockRoute = TestUtils.GetMockResponse<RouteData>(responses["accessingrouteresp"].json);
            TestUtils.RoutesAreEqual(mockRoute, route);
        }

        [Test]
        public void T10UpdatingRouteTest()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);
            var vehicle = TestHelper.GetVehicle(api, user, problem);

            //##BEGIN EXAMPLE updatingroute##
            api.Navigate<RouteData>(vehicle.GetLink("get-route"));
            var routeReq = new RouteUpdateRequest
            {
                Items = new[] {11, 21, 12, 22}
            };
            api.Navigate<ResponseData>(vehicle.GetLink("set-route"), routeReq);
            //##END EXAMPLE##
            var route = api.Navigate<RouteData>(vehicle.GetLink("get-route"));
            Trace.Write(JsonConvert.SerializeObject(route));
            var mockRoute = new[] {11, 21, 12, 22};
            Assert.AreEqual(mockRoute, route.Items);
        }

        [Test]
        public void T11StartingOptTest()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);
            problem = api.Navigate<RoutingProblemData>(problem.GetLink("self"));
            //##BEGIN EXAMPLE startingopt##
            var res = api.Navigate<ResponseData>(problem.GetLink("toggle-optimization"),
                new RoutingProblemUpdateRequest {Name = problem.Name, State = "Running"});
            //##END EXAMPLE##

            var mockCreation = TestUtils.GetMockResponse<ResponseData>(responses["startingoptresp"].json);
            Trace.Write(JsonConvert.SerializeObject(res));
            TestUtils.ResponsesAreEqual(mockCreation, res);

            problem = api.Navigate<RoutingProblemData>(problem.GetLink("self"));

            res = api.Navigate<ResponseData>( problem.GetLink( "toggle-optimization" ), new RoutingProblemUpdateRequest { Name = problem.Name, State = "Stopped" } );
        }



        [Test]
        public void T13StoppingOptTest()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);
            problem = api.Navigate<RoutingProblemData>(problem.GetLink("self"));
            var res = api.Navigate<ResponseData>(problem.GetLink("toggle-optimization"),
                new RoutingProblemUpdateRequest {Name = problem.Name, State = "Running"});

            problem = api.Navigate<RoutingProblemData>(problem.GetLink("self"));
            //##BEGIN EXAMPLE stoppingopt##
            res = api.Navigate<ResponseData>(problem.GetLink("toggle-optimization"),
                new RoutingProblemUpdateRequest {Name = problem.Name, State = "Stopped"});
            //##END EXAMPLE##
            var mockResponse = TestUtils.GetMockResponse<ResponseData>(responses["stoppingoptresp"].json);
            Trace.Write(JsonConvert.SerializeObject(res));
            TestUtils.ResponsesAreEqual(mockResponse, res);
        }



        [Test]
        public void T15BadRequestTest()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            try
            {
                var result = api.Navigate<ResponseData>(user.GetLink("create-problem"));
                

                //##BEGIN EXAMPLE badrequest##
                var badRequest = api.Navigate<ResponseData>(user.GetLink("create-problem"));
                //##END EXAMPLE##
            }
            catch (System.IO.IOException e)
            {
                Assert.IsTrue(e.Message.Contains("400 Bad Request") || e.Message.Contains("400 BadRequest"));
            }
        }

        [Test]
        public void T16InvalidVersionNumber()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);

            //##BEGIN EXAMPLE invalidversionnumber##

            var result = api.Navigate<ResponseData>(problem.GetLink("toggle-optimization"),
                new RoutingProblemUpdateRequest
                {
                    Name = problem.Name,
                    State = "Running"
                });

            try
            {
                var result2 = api.Navigate<ResponseData>(problem.GetLink("toggle-optimization"),
                    new RoutingProblemUpdateRequest
                    {
                        Name = problem.Name,
                        State = "Running"
                    });
            }
            catch (System.IO.IOException ioe)
            {
                Assert.IsTrue("412 Precondition Failed".Equals(ioe.Message) ||
                              "412 PreconditionFailed".Equals(ioe.Message));
            }
            //##END EXAMPLE##

        }

        [Test]
        public void T17CreateUser()
        {
            var api = TestHelper.Authenticate();

            //##BEGIN EXAMPLE creatingauser##

            var users = api.Navigate<UserDataSet>(api.Root.GetLink("list-users"));
            var response = api.Navigate(users.GetLink("create"));
            var user = api.Navigate<UserData>(response.Location);
            //##END EXAMPLE##

            Trace.Write(JsonConvert.SerializeObject(user));
            Assert.NotNull(user);
        }

        [Test]
        public void T18GetProgress()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);

            //##BEGIN EXAMPLE getprogress##
            api.Navigate<ResponseData>(problem.GetLink("toggle-optimization"),
                new RoutingProblemUpdateRequest
                {
                    Name = problem.Name,
                    State = "Running"
                });

            while (true)
            {
                Thread.Sleep(1000);
                var progress = api.Navigate<RoutingProblemData>(problem.GetLink("self")).Progress;
                Console.WriteLine("Progress: " + progress + "%");
                if (progress >= 100) break;
            }
            //##END EXAMPLE##

        }

        [Test]
        public void T19GetRouteEvents()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);
            var vehicle = TestHelper.GetVehicle(api, user, problem);

            //##BEGIN EXAMPLE getrouteEvents##
            var events = api.Navigate<RouteEventDataSet>(vehicle.GetLink("list-events"));
            //##END EXAMPLE##
            Trace.Write(JsonConvert.SerializeObject(events));
            var mockevents = TestUtils.GetMockResponse<RouteEventDataSet>(responses["accessingrouteeventsresp"].json);
        }


        [Test]
        public void T20VehicleImportSpeedTest()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);

            var vehicleCapacities = new List<CapacityData> {new CapacityData() {Name = "Weight", Amount = 100000}};

            var vehicleTimeWindow = new List<TimeWindowData>
            {
                new TimeWindowData
                {
                    Start = new DateTime(2013, 5, 14, 7, 0, 0),
                    End = new DateTime(2013, 5, 14, 16, 0, 0)
                }
            };

            var vehiclePickup = new LocationData()
            {
                Coordinate = new CoordinateData {Latitude = 62.244588, Longitude = 25.742683, System = "WGS84"}
            };
            var vehicleDelivery = new LocationData()
            {
                Coordinate = new CoordinateData {Latitude = 62.244588, Longitude = 25.742683, System = "WGS84"}
            };
            var importRequest = new VehicleSetImportRequest
            {
                Items = new List<VehicleUpdateRequest>()
            };

            for (int i = 0; i < 10; i++)
            {
                var veh = new VehicleUpdateRequest()
                {
                    Name = "Vehicle" + i + 1,
                    Capacities = vehicleCapacities,
                    StartLocation = vehiclePickup,
                    EndLocation = vehicleDelivery,
                    TimeWindows = vehicleTimeWindow,
                    RelocationType = "None"
                };
                importRequest.Items.Add(veh);
            }

            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = api.Navigate<ResponseData>(problem.GetLink("import-vehicles"), importRequest);
            timer.Stop();
            Console.WriteLine("Time elapsed with set import: {0}", timer.Elapsed);



            var vehicleList = new List<VehicleUpdateRequest>();
            for (int i = 0; i < 10; i++)
            {
                var vehicleReq = new VehicleUpdateRequest()
                {
                    Name = "Car" + i + 1,
                    Capacities = vehicleCapacities,
                    StartLocation = vehiclePickup,
                    EndLocation = vehicleDelivery,
                    TimeWindows = vehicleTimeWindow,
                    RelocationType = "None"
                };
                vehicleList.Add(vehicleReq);
            }

            timer = new Stopwatch();
            timer.Start();
            foreach (var request in vehicleList)
            {
                api.Navigate<ResponseData>(problem.GetLink("create-vehicle"), request);
            }
            timer.Stop();
            Console.WriteLine("Time elapsed with 100 create operations: {0}", timer.Elapsed);
        }

        [Test]
        public void T21GetParentVehicleFromPlanData()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);

            //##BEGIN EXAMPLE queryrouteevents##
            var plan = api.Navigate<PlanData>(problem.GetLink("plan"));
            //##END EXAMPLE##
            Trace.Write(JsonConvert.SerializeObject(plan));
            var routeEvent = plan.Items[0].Events[0];
            var vehicle = api.Navigate<VehicleData>(routeEvent.GetLink("get-vehicle"));

        }

        [Test]
        public void T22VehicleMassImport()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);
            //vehiclemassimport

            var vehicleCapacities = new List<CapacityData> {new CapacityData() {Name = "Weight", Amount = 100000}};

            var vehicleTimeWindow = new List<TimeWindowData>
            {
                new TimeWindowData
                {
                    Start = new DateTime(2013, 5, 14, 7, 0, 0),
                    End = new DateTime(2013, 5, 14, 16, 0, 0)
                }
            };

            var vehiclePickup = new LocationData()
            {
                Coordinate = new CoordinateData {Latitude = 62.244588, Longitude = 25.742683, System = "WGS84"}
            };
            var vehicleDelivery = new LocationData()
            {
                Coordinate = new CoordinateData {Latitude = 62.244588, Longitude = 25.742683, System = "WGS84"}
            };

            //##BEGIN EXAMPLE importvehicleset##
            var importRequest = new VehicleSetImportRequest
            {
                Items = new List<VehicleUpdateRequest>()
            };

            for (int i = 0; i < 10; i++)
            {
                var veh = new VehicleUpdateRequest()
                {
                    Name = "Vehicle name "+i,
                    Capacities = vehicleCapacities,
                    StartLocation = vehiclePickup,
                    EndLocation = vehicleDelivery,
                    TimeWindows = vehicleTimeWindow,
                    RelocationType = "None"
                };
                importRequest.Items.Add(veh);
            }

            var result = api.Navigate<ResponseData>(problem.GetLink("import-vehicles"), importRequest);
            var vehicles = api.Navigate<VehicleDataSet>( problem.GetLink( "list-vehicles" ) );
            //##END EXAMPLE##
        }

        [Test]
        public void T24TaskMassImport()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblemWithDemoData(api, user);

            var capacity = new CapacityData {Name = "Weight", Amount = 20};

            var timeWindows = new List<TimeWindowData>
            {
                new TimeWindowData
                {
                    Start = new DateTime(2013, 5, 14, 7, 0, 0),
                    End = new DateTime(2013, 5, 14, 16, 0, 0)
                }
            };

            var vehiclePickup = new LocationData()
            {
                Coordinate = new CoordinateData {Latitude = 62.244588, Longitude = 25.742683, System = "WGS84"}
            };
            var vehicleDelivery = new LocationData()
            {
                Coordinate = new CoordinateData {Latitude = 62.244588, Longitude = 25.742683, System = "WGS84"}
            };

            //##BEGIN EXAMPLE importtaskset##
            var importRequest = new TaskSetImportRequest
            {
                Items = new List<TaskUpdateRequest>()
            };

            for (int i = 0; i < 10; i++)
            {
                var task = new TaskUpdateRequest {Name = "test name"};
                task.RelocationType = "None";
                task.ActivityState = "Active";

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
                    },
                    TimeWindows = { new TimeWindowData { Start = new DateTime( 2013, 5, 14, 8, 0, 0 ), End = new DateTime( 2013, 5, 14, 12, 0, 0 ) } }
                };
                pickup.Capacities.Add(capacity);
                task.TaskEvents.Add(pickup);

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
                    },
                    TimeWindows = { new TimeWindowData { Start = new DateTime( 2013, 5, 14, 8, 0, 0 ), End = new DateTime( 2013, 5, 14, 12, 0, 0 ) } }
                };
                delivery.Capacities.Add(capacity);
                task.TaskEvents.Add(delivery);
                importRequest.Items.Add(task);
            }

            var result = api.Navigate<ResponseData>(problem.GetLink("import-tasks"), importRequest);
            //##END EXAMPLE##
        }

        [Test]
        public void T25ImportVehiclesAndTasks()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblem(api, user, "import-problem");

            var vehicleSet = new VehicleSetImportRequest
            {
                Items = new List<VehicleUpdateRequest>()
            };

            for (int i = 0; i < 5; i++)
            {
                var vehicle = TestHelper.GenerateVehicleUpdateRequestWithName("Vehicle" + i);
                vehicleSet.Items.Add(vehicle);
            }
            var depotSet = new ImportDepotSetRequest { Items = new List<UpdateDepotRequest>() };

            var taskSet = new TaskSetImportRequest
            {
                Items = new List<TaskUpdateRequest>()
            };

            for (int i = 0; i < 5; i++)
            {
                var task = TestHelper.GenerateTaskUpdateRequestWithName("Task" + i);
                taskSet.Items.Add(task);
            }
            //##BEGIN EXAMPLE importtasksandvehicles##
            var request = new ImportRequest
            {
                Tasks = taskSet,
                Vehicles = vehicleSet,
                Depots = depotSet
            };

            var result = api.Navigate<ResponseData>(problem.GetLink("import-data"), request);
            //##END EXAMPLE##

            //##BEGIN EXAMPLE getimportresults##
            var import = api.Navigate<ImportData>(result.Location);
            //##END EXAMPLE##

            Assert.AreEqual("Success", import.State);
            Assert.AreEqual(0, import.ErrorCount);
            Assert.AreEqual(5, import.Vehicles.Count);
            Assert.AreEqual(5, import.Tasks.Count);
            Assert.IsTrue(import.Meta.Find(link => link.Rel.Equals("self")).Enabled);
            Assert.IsTrue(import.Meta.Find(link => link.Rel.Equals("apply-import")).Enabled);
        }

        [Test]
        public void T26ImportVehiclesAndTasksFails()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblem(api, user, "failing-import-problem2");

            var vehicleSet = new VehicleSetImportRequest
            {
                Items = new List<VehicleUpdateRequest>()
            };

            for (int i = 0; i < 4; i++)
            {
                var vehicle = TestHelper.GenerateVehicleUpdateRequestWithName("Vehicle" + i);
                vehicleSet.Items.Add(vehicle);
            }
            vehicleSet.Items.Add(TestHelper.GenerateVehicleUpdateRequestWithName(""));

            var taskSet = new TaskSetImportRequest
            {
                Items = new List<TaskUpdateRequest>()
            };

            for (int i = 0; i < 4; i++)
            {
                var task = TestHelper.GenerateTaskUpdateRequestWithName("Task" + i);
                taskSet.Items.Add(task);
            }
            taskSet.Items.Add(TestHelper.GenerateTaskUpdateRequestWithName(""));

            var depotSet = new ImportDepotSetRequest { Items = new List<UpdateDepotRequest>() };

            var request = new ImportRequest
            {
                Tasks = taskSet,
                Vehicles = vehicleSet,
                Depots = depotSet
            };
            var result = api.Navigate<ResponseData>(problem.GetLink("import-data"), request);


            var import = api.Navigate<ImportData>(result.Location);

            Assert.AreEqual("Fail", import.State);
            Assert.AreEqual(2, import.ErrorCount);
            Assert.AreEqual(5, import.Vehicles.Count);
            Assert.AreEqual(5, import.Tasks.Count);

            Assert.IsTrue(import.Meta.Find(link => link.Rel.Equals("self")).Enabled);
            Assert.IsFalse(import.Meta.Find(link => link.Rel.Equals("apply-import")).Enabled);
        }

        [Test]
        public void T27ApplyImportedVehiclesAndTasks()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblem(api, user, "applied-import-problem");

            var vehicleSet = new VehicleSetImportRequest
            {
                Items = new List<VehicleUpdateRequest>()
            };

            for (int i = 0; i < 5; i++)
            {
                var vehicle = TestHelper.GenerateVehicleUpdateRequestWithName("Vehicle" + i);
                vehicleSet.Items.Add(vehicle);
            }


            var taskSet = new TaskSetImportRequest
            {
                Items = new List<TaskUpdateRequest>()
            };

            for (int i = 0; i < 5; i++)
            {
                var task = TestHelper.GenerateTaskUpdateRequestWithName("Task" + i);
                taskSet.Items.Add(task);
            }

            var depotSet = new ImportDepotSetRequest { Items = new List<UpdateDepotRequest>() };

            var request = new ImportRequest
            {
                Tasks = taskSet,
                Vehicles = vehicleSet,
                Depots = depotSet
            };

            var result = api.Navigate<ResponseData>(problem.GetLink("import-data"), request);

            var import = api.Navigate<ImportData>(result.Location);

            Assert.AreEqual("Success", import.State);
            Assert.AreEqual(0, import.ErrorCount);
            Assert.AreEqual(5, import.Vehicles.Count);
            Assert.AreEqual(5, import.Tasks.Count);

            Assert.IsTrue(import.Meta.Find(link => link.Rel.Equals("self")).Enabled);
            Assert.IsTrue(import.Meta.Find(link => link.Rel.Equals("apply-import")).Enabled);
            //##BEGIN EXAMPLE applyimport##
            var appyResult = api.Navigate<ResponseData>(import.GetLink("apply-import"));
            //##END EXAMPLE##
            var vehicles = api.Navigate<VehicleDataSet>(problem.GetLink("list-vehicles"));
            var tasks = api.Navigate<VehicleDataSet>(problem.GetLink("list-tasks"));
            Assert.AreEqual(5, vehicles.Items.Count);
            Assert.AreEqual(5, tasks.Items.Count);
        }

        [Test]
        public void T28DifferentCompatibilityTypes()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblem(api, user, "Compatibility-problem");

            var pickupCoord = new CoordinateData {Latitude = 62.244958, Longitude = 25.747143, System = "Euclidian"};
            var deliveryCoord = new CoordinateData {Latitude = 62.244589, Longitude = 25.74892, System = "Euclidian"};


            var task1 = TestHelper.GenerateTaskUpdateRequestWithNameAndCoordinates("Task1", pickupCoord, deliveryCoord);
            task1.IncompatibleVehicleTypes.Add("Kaivuri");

            var task2 = TestHelper.GenerateTaskUpdateRequestWithNameAndCoordinates("Task2", pickupCoord, deliveryCoord);
            task2.IncompatibleVehicleTypes.Add("Rekka");


            var start = new CoordinateData {Latitude = 62.244589, Longitude = 25.74892, System = "Euclidian"};
            var vehicle = TestHelper.GenerateVehicleUpdateRequestWithNameAndCoordinates("Vehicle", start, start);
            vehicle.VehicleType = "Rekka";

            var result = api.Navigate<ResponseData>(problem.GetLink("create-vehicle"), vehicle);
            result = api.Navigate<ResponseData>(problem.GetLink("create-task"), task1);
            result = api.Navigate<ResponseData>(problem.GetLink("create-task"), task2);

            problem = api.Navigate<RoutingProblemData>(problem.GetLink("self"));

            result = api.Navigate<ResponseData>(problem.GetLink("toggle-optimization"), new RoutingProblemUpdateRequest
            {
                Name = problem.Name,
                State = "Running"
            });

            problem = api.Navigate<RoutingProblemData>(problem.GetLink("self"));
            Thread.Sleep(1000);
            while (problem.State.Equals("Running"))
            {
                Thread.Sleep(1000);
                problem = api.Navigate<RoutingProblemData>(problem.GetLink("self"));
            }

            var plan = api.Navigate<PlanData>(problem.GetLink("plan"));

            // the plan should contain four events because task2 is incompatible with the vehicle 
            Assert.AreEqual(4, plan.Items[0].Events.Count, "Plan did not contain expected number of events.");
        }

        [Test]
        public void T29UpdatingRoutingProblemSettings()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser( api );
            var problem = TestHelper.CreateProblem( api, user, "UpdatingSettings" );
            
            RoutingProblemSettingsData after = null;

            //##BEGIN EXAMPLE updatingroutingproblemsettings##
            var settings = api.Navigate<RoutingProblemSettingsData>( problem.GetLink( "view-settings" ) );
            var updatedSettings = new RoutingProblemSettingsUpdateRequest { DefaultVehicleSpeedFactor = 0.8, DefaultVehicleSpeedProfile = SpeedProfile.Max120Kmh.ToString() };
            //##END EXAMPLE##

            var response = api.Navigate<ResponseData>(settings.GetLink("update-settings"), updatedSettings);

            after = api.Navigate<RoutingProblemSettingsData>(problem.GetLink("view-settings"));

            Assert.AreNotEqual(settings.DefaultVehicleSpeedFactor, after.DefaultVehicleSpeedFactor);
        }

        [Test]
        public void T30RequestingVehicleTypesFromProblem()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser( api );
            var problem = TestHelper.CreateProblem( api, user, "VehicleTypeTest" );

            var vehicleTypes = new List<string> {"Rekka", "Auto", "Mopo"};
            var vehicleSet = new VehicleSetImportRequest
            {
                Items = new List<VehicleUpdateRequest>()
            };

            for ( int i = 0; i < 3; i++ )
            {
                var vehicle = TestHelper.GenerateVehicleUpdateRequestWithName( "Vehicle" + i );
                vehicle.VehicleType = vehicleTypes[i];
                vehicleSet.Items.Add( vehicle );
            }

            var response = api.Navigate<ResponseData>(problem.GetLink("import-vehicles"), vehicleSet);
            
            var types = api.Navigate<VehicleTypeData>(problem.GetLink("get-types"));

            Assert.AreEqual(types.VehicleTypes.Count, vehicleTypes.Count);
        }

        [Test]
        public void T31LockingTaskEventToVehicle()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblem(api, user, "LockingTaskEventToVehicle");

            var pickupCoord = new CoordinateData { Latitude = 62.244958, Longitude = 25.747143, System = "Euclidian" };
            var deliveryCoord = new CoordinateData { Latitude = 62.244589, Longitude = 25.74892, System = "Euclidian" };


            var task1 = TestHelper.GenerateTaskUpdateRequestWithNameAndCoordinates("Task1", pickupCoord, deliveryCoord);
            
            var task2 = TestHelper.GenerateTaskUpdateRequestWithNameAndCoordinates("Task2", pickupCoord, deliveryCoord);

            var vehicle1 = TestHelper.GenerateVehicleUpdateRequestWithNameAndCoordinates("Vehicle1", pickupCoord, pickupCoord);
            var vehicle2 = TestHelper.GenerateVehicleUpdateRequestWithNameAndCoordinates("Vehicle2", pickupCoord, pickupCoord);

            var response = api.Navigate<ResponseData>(problem.GetLink("create-vehicle"), vehicle1);
            response = api.Navigate<ResponseData>(problem.GetLink("create-vehicle"), vehicle2);
            response = api.Navigate<ResponseData>(problem.GetLink("create-task"), task1);
            response = api.Navigate<ResponseData>(problem.GetLink("create-task"), task2);

            var vehicles = api.Navigate<VehicleDataSet>(problem.GetLink("list-vehicles"));
            var tasks = api.Navigate<TaskDataSet>(problem.GetLink("list-tasks"));

            var v1 = vehicles.Items.Find(v => v.Name == "Vehicle1");
            var v2 = vehicles.Items.Find(v => v.Name == "Vehicle2");
            var vehicleResult1 = api.Navigate<VehicleData>(v1.GetLink("self"));
            var vehicleResult2 = api.Navigate<VehicleData>(v2.GetLink("self"));
            var t1 = tasks.Items.Find(t => t.Name == "Task1");
            var t2 = tasks.Items.Find(t => t.Name == "Task2");

            var route1 = t1.TaskEvents.Select(te => te.Id).ToArray();
            var route2 = t2.TaskEvents.Select(te => te.Id).ToArray();

            api.Navigate<RouteData>(vehicleResult1.GetLink("get-route"));
            api.Navigate<RouteData>(vehicleResult2.GetLink("get-route"));

            api.Navigate<ResponseData>(vehicleResult1.GetLink("set-route"), new RouteUpdateRequest { Items = route1 });
            api.Navigate<ResponseData>(vehicleResult2.GetLink("set-route"), new RouteUpdateRequest { Items = route2 });

            var events1 = api.Navigate<RouteEventDataSet>(vehicleResult1.GetLink("list-events"));
            var events2 = api.Navigate<RouteEventDataSet>(vehicleResult2.GetLink("list-events"));

            foreach (var item in events1.Items)
            {
                var @event = api.Navigate<RouteEventData>(item.GetLink("self"));
                if (@event.TaskEventId < 20000) api.Navigate<ResponseData>(@event.GetLink("lock-to-vehicle"), new RouteEventUpdateRequest
                {
                    State = "LockedToVehicle"
                });
            }
            foreach (var item in events2.Items)
            {
                var @event = api.Navigate<RouteEventData>(item.GetLink("self"));
                if (@event.TaskEventId < 20000) api.Navigate<ResponseData>(@event.GetLink("lock-to-vehicle"), new RouteEventUpdateRequest
                {
                    State = "LockedToVehicle"
                } );
            }

            problem = api.Navigate<RoutingProblemData>(problem.GetLink("self"));

            var result = api.Navigate<ResponseData>(problem.GetLink("toggle-optimization"), new RoutingProblemUpdateRequest
            {
                Name = problem.Name,
                State = "Running"
            });

            problem = api.Navigate<RoutingProblemData>(problem.GetLink("self"));
            Thread.Sleep(1000);
            while (problem.State.Equals("Running"))
            {
                Thread.Sleep(1000);
                problem = api.Navigate<RoutingProblemData>(problem.GetLink("self"));
            }

            var plan = api.Navigate<PlanData>(problem.GetLink("plan"));
            Assert.AreEqual(4, plan.Items[0].Events.Count);
            Assert.AreEqual(4, plan.Items[1].Events.Count);

            // unlocking

            vehicleResult1 = api.Navigate<VehicleData>(v1.GetLink("self"));
            vehicleResult2 = api.Navigate<VehicleData>(v2.GetLink("self"));

            events1 = api.Navigate<RouteEventDataSet>(vehicleResult1.GetLink("list-events"));
            events2 = api.Navigate<RouteEventDataSet>(vehicleResult2.GetLink("list-events"));

            foreach (var item in events1.Items)
            {
                var @event = api.Navigate<RouteEventData>(item.GetLink("self"));
                if (@event.TaskEventId < 20000) api.Navigate<ResponseData>(@event.GetLink("lock-to-vehicle"), new RouteEventUpdateRequest
                {
                    State = "UnlockedFromVehicle"
                });
            }
            foreach (var item in events2.Items)
            {
                var @event = api.Navigate<RouteEventData>(item.GetLink("self"));
                if (@event.TaskEventId < 20000) api.Navigate<ResponseData>(@event.GetLink("lock-to-vehicle"), new RouteEventUpdateRequest
                {
                    State = "UnlockedFromVehicle"
                });
            }

            problem = api.Navigate<RoutingProblemData>(problem.GetLink("self"));

            result = api.Navigate<ResponseData>(problem.GetLink("toggle-optimization"), new RoutingProblemUpdateRequest
            {
                Name = problem.Name,
                State = "Running"
            });

            problem = api.Navigate<RoutingProblemData>(problem.GetLink("self"));
            Thread.Sleep(1000);
            while (problem.State.Equals("Running"))
            {
                Thread.Sleep(1000);
                problem = api.Navigate<RoutingProblemData>(problem.GetLink("self"));
            }

            plan = api.Navigate<PlanData>(problem.GetLink("plan"));
            if ( plan.Items[0].Events.Count == 2 )
            {
                Assert.AreEqual( 6, plan.Items[1].Events.Count );
            }
            else
            {
                Assert.AreEqual( 6, plan.Items[0].Events.Count );    
            }

        }


        [Test]
        public void T32LockingRoute()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser( api );
            var problem = TestHelper.CreateProblem( api, user, "LockingRoute" );

            var pickupCoord = new CoordinateData { Latitude = 62.244958, Longitude = 25.747143, System = "Euclidian" };
            var deliveryCoord = new CoordinateData { Latitude = 62.244589, Longitude = 25.74892, System = "Euclidian" };


            var task1 = TestHelper.GenerateTaskUpdateRequestWithNameAndCoordinates( "Task1", pickupCoord, deliveryCoord );

            var task2 = TestHelper.GenerateTaskUpdateRequestWithNameAndCoordinates( "Task2", pickupCoord, deliveryCoord );

            var vehicle1 = TestHelper.GenerateVehicleUpdateRequestWithNameAndCoordinates( "Vehicle1", pickupCoord, pickupCoord );
            var vehicle2 = TestHelper.GenerateVehicleUpdateRequestWithNameAndCoordinates( "Vehicle2", pickupCoord, pickupCoord );

            var response = api.Navigate<ResponseData>( problem.GetLink( "create-vehicle" ), vehicle1 );
            response = api.Navigate<ResponseData>( problem.GetLink( "create-vehicle" ), vehicle2 );
            response = api.Navigate<ResponseData>( problem.GetLink( "create-task" ), task1 );
            response = api.Navigate<ResponseData>( problem.GetLink( "create-task" ), task2 );

            var vehicles = api.Navigate<VehicleDataSet>( problem.GetLink( "list-vehicles" ) );
            var tasks = api.Navigate<TaskDataSet>( problem.GetLink( "list-tasks" ) );

            var v1 = vehicles.Items.Find( v => v.Name == "Vehicle1" );
            var v2 = vehicles.Items.Find( v => v.Name == "Vehicle2" );
            var vehicleResult1 = api.Navigate<VehicleData>( v1.GetLink( "self" ) );
            var vehicleResult2 = api.Navigate<VehicleData>( v2.GetLink( "self" ) );
            var t1 = tasks.Items.Find( t => t.Name == "Task1" );
            var t2 = tasks.Items.Find( t => t.Name == "Task2" );

            var route1 = t1.TaskEvents.Select( te => te.Id ).ToArray();
            var route2 = t2.TaskEvents.Select( te => te.Id ).ToArray();

            api.Navigate<RouteData>( vehicleResult1.GetLink( "get-route" ) );
            api.Navigate<RouteData>( vehicleResult2.GetLink( "get-route" ) );

            api.Navigate<ResponseData>( vehicleResult1.GetLink( "set-route" ), new RouteUpdateRequest { Items = route1 } );
            api.Navigate<ResponseData>( vehicleResult2.GetLink( "set-route" ), new RouteUpdateRequest { Items = route2 } );

            var events1 = api.Navigate<RouteEventDataSet>( vehicleResult1.GetLink( "list-events" ) );
            var events2 = api.Navigate<RouteEventDataSet>( vehicleResult2.GetLink( "list-events" ) );

            response = api.Navigate<ResponseData>( events1.Items[events1.Items.Count - 2].GetLink( "lock" ), new RouteEventUpdateRequest { State = "Locked"} );
            response = api.Navigate<ResponseData>( events2.Items[events2.Items.Count - 2].GetLink( "lock" ), new RouteEventUpdateRequest { State = "Locked" } );

            problem = api.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );

            var result = api.Navigate<ResponseData>( problem.GetLink( "toggle-optimization" ), new RoutingProblemUpdateRequest
            {
                Name = problem.Name,
                State = "Running"
            } );

            problem = api.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );
            Thread.Sleep( 1000 );
            while ( problem.State.Equals( "Running" ) )
            {
                Thread.Sleep( 1000 );
                problem = api.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );
            }

            var plan = api.Navigate<PlanData>( problem.GetLink( "plan" ) );
            Assert.AreEqual( 4, plan.Items[0].Events.Count );
            Assert.AreEqual( 4, plan.Items[1].Events.Count );

            // unlocking

            vehicleResult1 = api.Navigate<VehicleData>( v1.GetLink( "self" ) );
            vehicleResult2 = api.Navigate<VehicleData>( v2.GetLink( "self" ) );

            events1 = api.Navigate<RouteEventDataSet>( vehicleResult1.GetLink( "list-events" ) );
            events2 = api.Navigate<RouteEventDataSet>( vehicleResult2.GetLink( "list-events" ) );

            response = api.Navigate<ResponseData>( events1.Items[1].GetLink( "unlock" ), new RouteEventUpdateRequest { State = "Unlocked" } );
            response = api.Navigate<ResponseData>( events2.Items[1].GetLink( "unlock" ), new RouteEventUpdateRequest { State = "Unlocked" } );


            problem = api.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );

            result = api.Navigate<ResponseData>( problem.GetLink( "toggle-optimization" ), new RoutingProblemUpdateRequest
            {
                Name = problem.Name,
                State = "Running"
            } );

            problem = api.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );
            Thread.Sleep( 1000 );
            while ( problem.State.Equals( "Running" ) )
            {
                Thread.Sleep( 1000 );
                problem = api.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );
            }

            plan = api.Navigate<PlanData>( problem.GetLink( "plan" ) );

            if ( plan.Items[0].Events.Count == 2 )
            {
                Assert.AreEqual( 6, plan.Items[1].Events.Count );
            }
            else
            {
                Assert.AreEqual( 6, plan.Items[0].Events.Count );
            }
        }

        [Test]
        public void T33DeleteAllTasks()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser( api );
            var problem = TestHelper.CreateProblem( api, user, "DeleteTasks" );

            var pickupCoord = new CoordinateData { Latitude = 62.244958, Longitude = 25.747143, System = "Euclidian" };
            var deliveryCoord = new CoordinateData { Latitude = 62.244589, Longitude = 25.74892, System = "Euclidian" };


            var task1 = TestHelper.GenerateTaskUpdateRequestWithNameAndCoordinates( "Task1", pickupCoord, deliveryCoord );

            var task2 = TestHelper.GenerateTaskUpdateRequestWithNameAndCoordinates( "Task2", pickupCoord, deliveryCoord );

            var vehicle1 = TestHelper.GenerateVehicleUpdateRequestWithNameAndCoordinates( "Vehicle1", pickupCoord, pickupCoord );
            var vehicle2 = TestHelper.GenerateVehicleUpdateRequestWithNameAndCoordinates( "Vehicle2", pickupCoord, pickupCoord );

            var response = api.Navigate<ResponseData>( problem.GetLink( "create-vehicle" ), vehicle1 );
            response = api.Navigate<ResponseData>( problem.GetLink( "create-vehicle" ), vehicle2 );
            response = api.Navigate<ResponseData>( problem.GetLink( "create-task" ), task1 );
            response = api.Navigate<ResponseData>( problem.GetLink( "create-task" ), task2 );

            var tasksSet = api.Navigate<TaskDataSet>( problem.GetLink( "list-tasks" ) );
            Assert.AreEqual( 2, tasksSet.Items.Count );

            problem = api.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );
            response = api.Navigate<ResponseData>( problem.GetLink( "delete-tasks" ) );

            tasksSet = api.Navigate<TaskDataSet>( problem.GetLink( "list-tasks" ) );
            Assert.AreEqual( 0, tasksSet.Items.Count );
        }

        [Test]
        public void T34DeleteASingleTasks()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser( api );
            var problem = TestHelper.CreateProblem( api, user, "DeleteASingleTasks" );

            var pickupCoord = new CoordinateData { Latitude = 62.244958, Longitude = 25.747143, System = "Euclidian" };
            var deliveryCoord = new CoordinateData { Latitude = 62.244589, Longitude = 25.74892, System = "Euclidian" };


            var task1 = TestHelper.GenerateTaskUpdateRequestWithNameAndCoordinates( "Task1", pickupCoord, deliveryCoord );

            var task2 = TestHelper.GenerateTaskUpdateRequestWithNameAndCoordinates( "Task2", pickupCoord, deliveryCoord );

            var vehicle1 = TestHelper.GenerateVehicleUpdateRequestWithNameAndCoordinates( "Vehicle1", pickupCoord, pickupCoord );
            var vehicle2 = TestHelper.GenerateVehicleUpdateRequestWithNameAndCoordinates( "Vehicle2", pickupCoord, pickupCoord );

            var response = api.Navigate<ResponseData>( problem.GetLink( "create-vehicle" ), vehicle1 );
            response = api.Navigate<ResponseData>( problem.GetLink( "create-vehicle" ), vehicle2 );
            response = api.Navigate<ResponseData>( problem.GetLink( "create-task" ), task1 );
            response = api.Navigate<ResponseData>( problem.GetLink( "create-task" ), task2 );

            
            var tasksSet = api.Navigate<TaskDataSet>( problem.GetLink( "list-tasks" ) );
            Assert.AreEqual( 2, tasksSet.Items.Count );
            //##BEGIN EXAMPLE deletingtask##

            problem = api.Navigate<RoutingProblemData>( problem.GetLink( "self" ) );
            var task = api.Navigate<TaskData>( response.Location );
            api.Navigate<ResponseData>( task.GetLink( "delete" ) );
            //##END EXAMPLE##
            //response = api.Navigate<ResponseData>( problem.GetLink( "delete-tasks" ), new DeleteTasksRequest() );

            tasksSet = api.Navigate<TaskDataSet>( problem.GetLink( "list-tasks" ) );
            Assert.AreEqual( 1, tasksSet.Items.Count );
        }

        [Test]
        public void T35CreateDepot()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblem(api, user, "CreateDepot");
            //##BEGIN EXAMPLE createdepot##
            var depot = new UpdateDepotRequest
            {
                Name = "Depot02",
                Location = new LocationData { Coordinate = new CoordinateData { Latitude = 0, Longitude = 0, System = "Euclidian" } },
                Capacities = new List<CapacityData> { new CapacityData { Amount = 10, Name = "weight" }, new CapacityData { Amount = 30, Name = "volume" } },
                Info1 = "Info",
                DataSource = "",
                Type = "SomeType"
            };

            var response = api.Navigate<ResponseData>(problem.GetLink("create-depot"), depot);

            var depotData = api.Navigate<DepotData>(response.Location);
            //##END EXAMPLE##
            Assert.AreEqual(depot.Name, depotData.Name);

            Assert.AreEqual(JsonConvert.SerializeObject(depot.Location.Coordinate), JsonConvert.SerializeObject(depotData.Location.Coordinate));
            Assert.AreEqual(JsonConvert.SerializeObject(depot.Capacities), JsonConvert.SerializeObject(depotData.Capacities));

        }

        [Test]
        public void T36CreateDepotSet()
        {
            var api = TestHelper.Authenticate();
            var user = TestHelper.GetOrCreateUser(api);
            var problem = TestHelper.CreateProblem(api, user, "CreateDepotSet");

            //##BEGIN EXAMPLE importdepots##

            var request = new ImportDepotSetRequest
            {
                Items = new List<UpdateDepotRequest>()
            };

            for (int i = 1; i < 4; i++)
            {
                var depot = new UpdateDepotRequest
                {
                    Name = "Depot0"+i,
                    Location = new LocationData { Coordinate = new CoordinateData { Latitude = 0, Longitude = 0, System = "Euclidian" } },
                    Capacities = new List<CapacityData> { new CapacityData { Amount = 10, Name = "weight" }, new CapacityData { Amount = 30, Name = "volume" } },
                    Info1 = "Info",
                    DataSource = "",
                    Type = "SomeType"
                };
                request.Items.Add(depot);
            }

            var response = api.Navigate<ResponseData>(problem.GetLink("import-depots"), request);

            var result = api.Navigate<DepotDataSet>(problem.GetLink("list-depots"));
            //##END EXAMPLE##

            Assert.AreEqual(3, result.Items.Count);
        }
    }
}
