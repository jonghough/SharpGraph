using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SharpGraph {
    [TestFixture]
    public class PathsTest {
        [Test]
        public void PathsTest1 () {
            var g = GraphGenerator.CreateComplete (5);
            var ns = g.GetNodes ().ToList ();
            var paths = g.FindSimplePaths (ns[0], ns[1]);

            Assert.AreEqual (16, paths.Count);
        }

        [Test]
        public void PathsTest2 () {
            var g = GraphGenerator.CreateComplete (5);
            var ns = g.GetNodes ().ToList ();
            var paths = g.FindSimplePaths (ns[0], ns[1], 1);

            Assert.AreEqual (1, paths.Count);

            paths = g.FindSimplePaths (ns[0], ns[1], 2);
            Assert.AreEqual (4, paths.Count);
        }

        [Test]
        public void PathsTest3 () {
            var g = GraphGenerator.GenerateBipartiteComplete (2, 2);
            var ns = g.GetNodes ().ToList ();
            var paths = g.FindSimplePaths (ns[0], ns[1]);

            Assert.AreEqual (2, paths.Count);
        }
    }
}