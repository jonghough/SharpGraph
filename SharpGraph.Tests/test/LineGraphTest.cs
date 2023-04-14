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