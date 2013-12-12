using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NFleet.Tests
{
    [TestFixture]
    class UtilTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestBuildUriWithSingleDoubleDotSlash()
        {
            var self = "/users/10/problems/1/vehicles/1/events/1";
            var op = "../";
            var expected = "/users/10/problems/1/vehicles/1/events";
            var actual = LinkUtil.BuildUri(self,op);
            Assert.AreEqual(expected, actual, "Uris did not match.");
        }

        [Test]
        public void TestBuildUriWithTwoDoubleDotSlash()
        {
            var self = "/users/10/problems/1/vehicles/1/events/1";
            var op = "../../";
            var expected = "/users/10/problems/1/vehicles/1";
            var actual = LinkUtil.BuildUri(self, op);
            Assert.AreEqual(expected, actual, "Uris did not match.");
        }

        [Test]
        public void TestBuildUriWithSigleDoubleDotSlashesAndNewUriEnding()
        {
            var self = "/users/10/problems/1/vehicles/1/events/1";
            var op = "../tasks/1";
            var expected = "/users/10/problems/1/vehicles/1/events/tasks/1";
            var actual = LinkUtil.BuildUri(self, op);
            Assert.AreEqual(expected, actual, "Uris did not match.");
        }

        [Test]
        public void TestBuildUriWithmultipleDoubleDotSlashesAndNewUriEnding()
        {
            var self = "/users/10/problems/1/vehicles/1/events/1";
            var op = "../../../../tasks/1";
            var expected = "/users/10/problems/1/tasks/1";
            var actual = LinkUtil.BuildUri(self, op);
            Assert.AreEqual(expected, actual, "Uris did not match.");
        }
    }
}
