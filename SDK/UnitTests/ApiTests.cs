using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NFleetSDK.Data;

namespace NFleetSDK.UnitTests
{
    [TestFixture]
    internal class ApiTests
    {
        //TODO: read these from some sane place
        private string username, password;
        private Api api;

        [Test]
        public void AccessingApiTest()
        {
            //##BEGIN EXAMPLE accessingapi##
            api = new Api("https://api.co-sky.fi", username, password);
            var apiData = api.Root;
            //##END EXAMPLE##
        }

        [Test]
        public void OauthTest()
        {
            //##BEGIN EXAMPLE oauth##
            //var tokenResponse = api.Authorize()
            //##END EXAMPLE##
        }

        [Test]
        public void CreatingProblemTest()
        {
            //##BEGIN EXAMPLE creatingproblem##
            /*rikki:
            var problemData = new ProblemData { Name = "Second test" };
            var result = api.Navigate<ResultData>(problems.GetLink("create"), problemData); */
            //##END EXAMPLE##
        }

        [Test]
        public void AccessingProblemTest()
        {
            //##BEGIN EXAMPLE accessingproblem##
            var problems =
                api.Navigate<RoutingProblemDataSet>(new Link {Rel = "list-problems", Uri = "/problems", Method = "GET"});
            //##END EXAMPLE##
        }

        [Test]
        public void ListingTasksTest()
        {
            //##BEGIN EXAMPLE listingtasks##
            //var tasks = api.Navigate<TaskDataSet>(problem.GetLink("list-tasks")); 
            //##END EXAMPLE##
        }


        [Test]
        public void CreatingTaskTest()
        {
            //##BEGIN EXAMPLE creatingtask##
            /*
            var newTask = new TaskData { Name = "test name" };
            CapacityData capacity = new CapacityData { Name = "Weight", Amount = 20 };

            var pickup = new TaskEventData
            {
                Type = EventType.Pickup,
                Location = new LocationData
                {
                    Coordinate = new CoordinateData
                    {
                        Latitude = "62.244958",
                        Longitude = "25.747143"
                    }
                }
            };
            pickup.Capacities.Add(capacity);
            newTask.Events.Add(pickup);

            var delivery = new TaskEventData
            {
                Type = EventType.Delivery,
                Location = new LocationData
                {
                    Coordinate = new CoordinateData
                    {
                        Latitude = "62.244589",
                        Longitude = "25.74892"
                    }
                }
            };
            delivery.Capacities.Add(capacity);
            newTask.Events.Add(delivery);

            var taskCreationResult = api.Navigate<ResultData>(tasks.GetLink("create"), newTask); 
             */
            //##END EXAMPLE##
        }

        [Test]
        public void UpdatingTaskTest()
        {
            //##BEGIN EXAMPLE updatingtask##
            /*
            newTask = api.Navigate<TaskData>(taskCreationResult.Location);
            newTask.Name = "other name";
            api.Navigate(newTask.GetLink("update"), newTask); 
             */
            //##END EXAMPLE##
        }

        [Test]
        public void DeletingTaskTest()
        {
            //##BEGIN EXAMPLE deletingtask##
            //api.Navigate(newTask.GetLink("delete")); 
            //##END EXAMPLE##
        }

        [Test]
        public void ListingVehiclesTest()
        {
            //##BEGIN EXAMPLE listingvehicles##
            //var vehicles = api.Navigate<VehicleDataSet>(problem.GetLink("list-vehicles")); 
            //##END EXAMPLE##
        }

        [Test]
        public void AccessingTaskSeqTest()
        {
            //##BEGIN EXAMPLE accessingtaskseq##
            //var taskEvents = api.Navigate<TaskEventDataSet>(vehicle.GetLink("list-events")); 
            //##END EXAMPLE##
        }

        public void AccessingRouteTest()
        {
            //##BEGIN EXAMPLE accessingroute##
            //var taskEventIds = api.Navigate<TaskEventIds>(vehicle.GetLink("get-route")); 
            //##END EXAMPLE##
        }

        [Test]
        public void UpdatingRouteTest()
        {
            //##BEGIN EXAMPLE updatingroute##
            /*var points = { 8, 9, 12, 6 };
            route.Items = points;
            api.Navigate(route.GetLink("update"), route); */
            //##END EXAMPLE##
        }

        [Test]
        public void StartingOptTest()
        {
            //##BEGIN EXAMPLE startingopt##
            //var creation = api.navigate<ResultData>(problem.Links["start-new-optimization"]); 
            //##END EXAMPLE##
        }

        [Test]
        public void AccessingNewOptTest()
        {
            //##BEGIN EXAMPLE accessingnewopt##
            //var optimization = api.Navigate(creation.Location); 
            //##END EXAMPLE##
        }

        [Test]
        public void StoppingOptTest()
        {
            //##BEGIN EXAMPLE stoppingopt##
            //api.Navigate(optimization.GetLink("stop")); 
            //##END EXAMPLE##
        }

        [Test]
        public void GetOptStatusTest()
        {
            //##BEGIN EXAMPLE getoptstatus#
            //optimization = api.Navigate<OptimizationData>(optimization.GetLink("self")); 
            //##END EXAMPLE##
        }

        [Test]
        public void Test()
        {
            //##BEGIN EXAMPLE oauth##
            /*var problemData = new ProblemData();
            var result = api.Navigate<ResultData>(problems.GetLink("create"), problemData); */
            //##END EXAMPLE##
        }
    }
}
