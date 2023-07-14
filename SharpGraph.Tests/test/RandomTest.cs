using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SharpGraph
{
    [TestFixture]
    public class RandomTest
    {
        [Test]
        public void RandomTest1()
        {
            var g = Graph.GenerateRandom(10, 5, false, 10);
            var ns = g.GetNodes().ToList();

            Assert.AreEqual(10, ns.Count);
            Assert.AreEqual(5, g.GetEdges().Count);
        }

         [Test]
        public void RandomTest2()
        {
            var g = Graph.GenerateRandom(140, 125, false, 10);
            var ns = g.GetNodes().ToList();

            Assert.AreEqual(140, ns.Count);
            Assert.AreEqual(125, g.GetEdges().Count);
            foreach (var edge in g.GetEdges())
            {
                var comp = g.GetComponent<EdgeDirection>(edge);
                Assert.IsNull(comp);
            }
        }

        [Test]
        public void RandomTest3()
        {
            var g = Graph.GenerateRandom(8, 15, true, 50);
            var ns = g.GetNodes().ToList();

            Assert.AreEqual(8, ns.Count);
            Assert.AreEqual(15, g.GetEdges().Count);
            foreach (var edge in g.GetEdges())
            {
                var comp = g.GetComponent<EdgeDirection>(edge);
                Assert.NotNull(comp);
            }
        }

        [Test]
        public void RandomTest4()
        {
            var g = Graph.GenerateRandom(20, 10, false, 50);
            var ns = g.GetNodes().ToList();

            Assert.AreEqual(20, ns.Count);
            Assert.AreEqual(10, g.GetEdges().Count);
            // not enough edges to remain connected.
            Assert.AreEqual(false, g.IsConnected());
        }


        [Test]
        public void RandomExceptionTest1()
        {
            Assert.Throws<RandomGraphException>(() => Graph.GenerateRandom(5, 11, false, 10));

        }

        [Test]
        public void RandomExceptionTest2()
        {
            Assert.Throws<RandomGraphException>(() => Graph.GenerateRandom(5, -1, false, 2));

        }

        [Test]
        public void RandomSpanningTreeTest1()
        {
            var g = Graph.GenerateRandomSpanningTree(10,   10);
            var ns = g.GetNodes().ToList();

            Assert.AreEqual(10, ns.Count);
            Assert.AreEqual(9, g.GetEdges().Count);
            // tree so there should be no cyclesw
            var simpleCycles = g.FindSimpleCycles();
            Assert.AreEqual(0, simpleCycles.Count);
        }

        [Test]
        public void RandomSpanningTreeExceptionTest1()
        {
            Assert.Throws<RandomGraphException>(() => Graph.GenerateRandomSpanningTree(2, -1 ));

        }
    }
}