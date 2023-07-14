using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace SharpGraph {
    public class LineGraphTest {

        [Test]
        public void TestLineGraphOfComplete () {
            var nodes = NodeGenerator.GenerateNodes (7);
            var K7 = GraphGenerator.CreateComplete (nodes);

            var lineGraph = LineGraph.GenerateLineGraph (K7, new LineGraphBuilder ());

            Assert.AreEqual (21, K7.GetEdges ().Count);
            Assert.AreEqual (21 * 10 / 2, lineGraph.GetEdges ().Count);

        }

         [Test]
        public void TestLineGraphOfCycle1 () {
            var C10 = GraphGenerator.GenerateCycle(10);

            var lineGraph = LineGraph.GenerateLineGraph (C10, new LineGraphBuilder ());

            Assert.AreEqual (10, C10.GetEdges ().Count);
            Assert.AreEqual (10, lineGraph.GetEdges ().Count);
            Assert.AreEqual (10, C10.GetNodes ().Count);
            Assert.AreEqual (10, lineGraph.GetNodes ().Count);

        }

        class LineGraphBuilder : ILineGraphBuilder {
            public Edge createEdge (Node nodeFrom, Node nodeTo) {
                return new Edge (nodeFrom, nodeTo);
            }

            public Node createNode (Edge edge) {
                var bn = new Node ("(" + edge.From ().GetLabel () + ", " + edge.To ().GetLabel () + ")");
                return bn;
            }
        }
    }
}