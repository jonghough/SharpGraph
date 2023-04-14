using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace SharpGraph {
    [TestFixture]
    public class SpanningTreeTest {
        [Test]
        public void CompleteGraphSpanningTreeTest () {
            HashSet<Node> nodes = NodeGenerator.GenerateNodes (10);
            var g = GraphGenerator.CreateComplete (nodes);
            Assert.AreEqual (9, g.GenerateSpanningTree ().Count);
        }

        [Test]
        public void MinimumSpanningTreeTest1 () {

            HashSet<Node> nodes = NodeGenerator.GenerateNodes (8);
            List<Node> nodeList = new List<Node> (nodes);
            Edge wedge1 = new Edge (nodeList[0], nodeList[1]);
            Edge wedge2 = new Edge (nodeList[0], nodeList[2]);
            Edge wedge3 = new Edge (nodeList[0], nodeList[7]);
            Edge wedge4 = new Edge (nodeList[1], nodeList[2]);
            Edge wedge5 = new Edge (nodeList[1], nodeList[3]);
            Edge wedge6 = new Edge (nodeList[1], nodeList[5]);
            Edge wedge7 = new Edge (nodeList[2], nodeList[4]);
            Edge wedge8 = new Edge (nodeList[2], nodeList[5]);
            Edge wedge9 = new Edge (nodeList[2], nodeList[6]);
            Edge wedge10 = new Edge (nodeList[3], nodeList[5]);
            Edge wedge11 = new Edge (nodeList[4], nodeList[6]);
            Edge wedge12 = new Edge (nodeList[5], nodeList[6]);
            Edge wedge13 = new Edge (nodeList[6], nodeList[7]);
            var edges = new List<Edge> ();
            edges.Add (wedge1);
            edges.Add (wedge2);
            edges.Add (wedge3);
            edges.Add (wedge4);
            edges.Add (wedge5);
            edges.Add (wedge6);
            edges.Add (wedge7);
            edges.Add (wedge8);
            edges.Add (wedge9);
            edges.Add (wedge10);
            edges.Add (wedge11);
            edges.Add (wedge12);
            edges.Add (wedge13);
            var g = new Graph (edges);

            g.AddComponent<EdgeWeight> (wedge1).Weight = 2.4f;
            g.AddComponent<EdgeWeight> (wedge2).Weight = 23.4f;
            g.AddComponent<EdgeWeight> (wedge3).Weight = 13.5f;
            g.AddComponent<EdgeWeight> (wedge4).Weight = 66.4f;
            g.AddComponent<EdgeWeight> (wedge5).Weight = 19.14f;
            g.AddComponent<EdgeWeight> (wedge6).Weight = 7.905f;
            g.AddComponent<EdgeWeight> (wedge7).Weight = 17.05f;
            g.AddComponent<EdgeWeight> (wedge8).Weight = 100.8f;
            g.AddComponent<EdgeWeight> (wedge9).Weight = 88.8f;
            g.AddComponent<EdgeWeight> (wedge10).Weight = 10.8f;
            g.AddComponent<EdgeWeight> (wedge11).Weight = 20.8f;
            g.AddComponent<EdgeWeight> (wedge12).Weight = 14.2f;
            g.AddComponent<EdgeWeight> (wedge13).Weight = 10.55f;

            var span = g.GenerateMinimumSpanningTree ();

            Assert.AreEqual (7, span.Count);
        }

        [Test]
        public void MinimumSpanningTreeTest2 () {

            Node b1 = new Node  ("1");
            Node b2 = new Node   ("2");
            Node b3 = new Node ("3");
            Edge wedge1 = new Edge (b1, b2);
            Edge wedge2 = new Edge (b1, b3);
            Edge wedge3 = new Edge (b2, b3);
            var edges = new List<Edge> ();
            edges.Add (wedge1);
            edges.Add (wedge2);
            edges.Add (wedge3);

            var g = new Graph (edges);

            g.AddComponent<EdgeWeight> (wedge1).Weight = -5.1f;
            g.AddComponent<EdgeWeight> (wedge2).Weight = 0f;
            g.AddComponent<EdgeWeight> (wedge3).Weight = 3.5f;
            var span = g.GenerateMinimumSpanningTree ();

            //minimum spanning tree should contain only edges 1 and 2, not edge 3.
            Assert.True (span.Contains (wedge1) && span.Contains (wedge2) && !span.Contains (wedge3));
        }

        [Test]
        public void MinimumSpanningTreeTest3 () {

            Node b1 = new Node ("1");
            Node b2 = new Node ("2");
            Node b3 = new Node ("3");
            Node b4 = new Node  ("4");
            Edge wedge1 = new Edge (b1, b2);
            Edge wedge2 = new Edge (b1, b3);
            Edge wedge3 = new Edge (b2, b3);
            Edge wedge4 = new Edge (b2, b4);
            Edge wedge5 = new Edge (b3, b4);

            var edges = new List<Edge> ();
            edges.Add (wedge1);
            edges.Add (wedge2);
            edges.Add (wedge3);
            edges.Add (wedge4);
            edges.Add (wedge5);

            var g = new Graph (edges);

            g.AddComponent<EdgeWeight> (wedge1).Weight = 0.1f;
            g.AddComponent<EdgeWeight> (wedge2).Weight = 0.14f;
            g.AddComponent<EdgeWeight> (wedge3).Weight = 0.15f;
            g.AddComponent<EdgeWeight> (wedge4).Weight = 0.15f;
            g.AddComponent<EdgeWeight> (wedge5).Weight = 0.15f;

            var span = g.GenerateMinimumSpanningTree ();
            Assert.AreEqual (3, span.Count);
            //minimum spanning tree should contain only edges 1 and 2
            Assert.True (span.Contains (wedge1) && span.Contains (wedge2));
            //only contains one of edge 3,4,5
            Assert.True (span.Contains (wedge3) ^ span.Contains (wedge4) ^ span.Contains (wedge5));
        }
    }
}