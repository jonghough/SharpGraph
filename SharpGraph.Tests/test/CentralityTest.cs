using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace SharpGraph.Tests.test {
    [TestFixture]
    public class CentralityTest {

        [Test]
        public void TestCentralityOfCompleteGraph () {
            var k10 = GraphGenerator.CreateComplete (10);
            var degCen = k10.GetDegreeCentrality ();
            foreach (var kvp in degCen) {
                // centrality is 0.9 for all nodes
                Assert.AreEqual (0.9f, kvp.Value);
            }
        }

        [Test]
        public void TestCentrality2 () {
            var g = new Graph ();
            g.AddEdge ("A", "B");
            g.AddEdge ("A", "C");
            g.AddEdge ("A", "D");
            g.AddEdge ("A", "E");
            g.AddEdge ("B", "C");
            var da = g.GetDegreeCentrality (new Node ("A"));
            Assert.AreEqual (0.8f, da);

            var dd = g.GetDegreeCentrality (new Node ("D"));
            Assert.AreEqual (0.2f, dd);

        }
    }
}