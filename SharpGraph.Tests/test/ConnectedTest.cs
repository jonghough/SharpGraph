using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace SharpGraph {
    [TestFixture]
    public class ConnectedTest {
        [Test]
        public void ConnectedTest1 () {
            var g = GraphGenerator.CreateComplete (NodeGenerator.GenerateNodes (2));
            Assert.True (g.IsConnected ());
        }

        [Test]
        public void ConnectedTest2 () {
            var g = GraphGenerator.CreateComplete (NodeGenerator.GenerateNodes (3));
            Assert.True (g.IsConnected ());
        }

        [Test]
        public void ConnectedTest3 () {
            var g = GraphGenerator.CreateComplete (NodeGenerator.GenerateNodes (5));
            Assert.True (g.IsConnected ());
        }

        [Test]
        public void ConnectedTest4 () {
            var g = GraphGenerator.CreateComplete (NodeGenerator.GenerateNodes (10));
            Assert.True (g.IsConnected ());
        }

        [Test]
        public void ConnectedTest5 () {

            /* nodes 0,1,2,3 are connected and nodes 4,5,6,7 are connected */
            HashSet<Node> nodes = NodeGenerator.GenerateNodes (8);
            List<Node> nodeList = new List<Node> (nodes);
            Edge wedge1 = new Edge (nodeList[0], nodeList[1]);
            Edge wedge2 = new Edge (nodeList[0], nodeList[2]);
            Edge wedge3 = new Edge (nodeList[0], nodeList[3]);
            Edge wedge4 = new Edge (nodeList[1], nodeList[2]);
            Edge wedge5 = new Edge (nodeList[1], nodeList[3]);

            Edge wedge6 = new Edge (nodeList[4], nodeList[5]);
            Edge wedge7 = new Edge (nodeList[4], nodeList[6]);
            Edge wedge8 = new Edge (nodeList[5], nodeList[6]);
            Edge wedge9 = new Edge (nodeList[5], nodeList[7]);
            Edge wedge10 = new Edge (nodeList[6], nodeList[7]);
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
            var g = new Graph (edges);
            //graph is not connected.
            Assert.False (g.IsConnected ());
        }

        [Test]
        public void StronglyConnectedCOmponentTest1 () {

            HashSet<Node> nodes = NodeGenerator.GenerateNodes (5);
            List<Node> nodeList = new List<Node> (nodes);
            Edge wedge1 = new Edge (nodeList[0], nodeList[1]);
            Edge wedge2 = new Edge (nodeList[1], nodeList[2]);
            Edge wedge3 = new Edge (nodeList[2], nodeList[3]);
            Edge wedge4 = new Edge (nodeList[3], nodeList[0]);
            Edge wedge5 = new Edge (nodeList[3], nodeList[4]);

            var edges = new List<Edge> ();
            edges.Add (wedge1);
            edges.Add (wedge2);
            edges.Add (wedge3);
            edges.Add (wedge4);
            edges.Add (wedge5);

            var g = new Graph (edges);

            var ed1 = g.AddComponent<EdgeDirection> (wedge1);
            ed1.Direction = Direction.Forwards;

            var ed2 = g.AddComponent<EdgeDirection> (wedge2);
            ed2.Direction = Direction.Forwards;

            var ed3 = g.AddComponent<EdgeDirection> (wedge3);
            ed3.Direction = Direction.Forwards;

            var ed4 = g.AddComponent<EdgeDirection> (wedge4);
            ed4.Direction = Direction.Forwards;

            var ed5 = g.AddComponent<EdgeDirection> (wedge5);
            ed5.Direction = Direction.Forwards;
            //graph is not connected.
            Assert.True (g.IsConnected ());

            List<Graph> sccl = g.FindStronglyConnectedComponents ();
            Assert.True (sccl.Count == 2);
        }

        [Test]
        public void StronglyConnectedCOmponentTest2 () {

            HashSet<Node> nodes = NodeGenerator.GenerateNodes (5);
            List<Node> nodeList = new List<Node> (nodes);
            Edge wedge1 = new Edge (nodeList[0], nodeList[1]);
            Edge wedge2 = new Edge (nodeList[1], nodeList[2]);
            Edge wedge3 = new Edge (nodeList[2], nodeList[3]);
            Edge wedge4 = new Edge (nodeList[3], nodeList[0]);
            Edge wedge5 = new Edge (nodeList[3], nodeList[4]);

            var edges = new List<Edge> ();
            edges.Add (wedge1);
            edges.Add (wedge2);
            edges.Add (wedge3);
            edges.Add (wedge4);
            edges.Add (wedge5);

            var g = new Graph (edges);

            var ed1 = g.AddComponent<EdgeDirection> (wedge1);
            ed1.Direction = Direction.Forwards;

            // backwards edge
            var ed2 = g.AddComponent<EdgeDirection> (wedge2);
            ed2.Direction = Direction.Backwards;

            var ed3 = g.AddComponent<EdgeDirection> (wedge3);
            ed3.Direction = Direction.Forwards;

            var ed4 = g.AddComponent<EdgeDirection> (wedge4);
            ed4.Direction = Direction.Forwards;

            var ed5 = g.AddComponent<EdgeDirection> (wedge5);
            ed5.Direction = Direction.Forwards;

            List<Graph> sccl = g.FindStronglyConnectedComponents ();
            Assert.True (sccl.Count == 5);
        }

        [Test]
        public void StronglyConnectedCOmponentTest3 () {

            HashSet<Node> nodes = NodeGenerator.GenerateNodes (6);
            List<Node> nodeList = new List<Node> (nodes);
            Edge wedge1 = new Edge (nodeList[0], nodeList[1]);
            Edge wedge2 = new Edge (nodeList[1], nodeList[2]);
            Edge wedge3 = new Edge (nodeList[2], nodeList[0]);

            Edge wedge4 = new Edge (nodeList[3], nodeList[4]);
            Edge wedge5 = new Edge (nodeList[4], nodeList[5]);
            Edge wedge6 = new Edge (nodeList[5], nodeList[3]);

            Edge wedge7 = new Edge (nodeList[2], nodeList[3]);

            var edges = new List<Edge> ();
            edges.Add (wedge1);
            edges.Add (wedge2);
            edges.Add (wedge3);
            edges.Add (wedge4);
            edges.Add (wedge5);
            edges.Add (wedge6);
            edges.Add (wedge7);

            var g = new Graph (edges);

            var ed1 = g.AddComponent<EdgeDirection> (wedge1);
            ed1.Direction = Direction.Forwards;

            var ed2 = g.AddComponent<EdgeDirection> (wedge2);
            ed2.Direction = Direction.Forwards;

            var ed3 = g.AddComponent<EdgeDirection> (wedge3);
            ed3.Direction = Direction.Forwards;

            // these three edges are backwars, but it should make no difference.
            var ed4 = g.AddComponent<EdgeDirection> (wedge4);
            ed4.Direction = Direction.Backwards;

            var ed5 = g.AddComponent<EdgeDirection> (wedge5);
            ed5.Direction = Direction.Backwards;

            var ed6 = g.AddComponent<EdgeDirection> (wedge6);
            ed6.Direction = Direction.Backwards;

            var ed7 = g.AddComponent<EdgeDirection> (wedge7);
            ed7.Direction = Direction.Forwards;

            List<Graph> sccl = g.FindStronglyConnectedComponents ();
            Assert.True (sccl.Count == 2);
        }

        [Test]
        public void BridgeTest1 () {

            var left = new HashSet<Node> ();
            left.Add (new Node ("A"));
            left.Add (new Node ("B"));
            left.Add (new Node ("C"));

            var right = new HashSet<Node> ();
            right.Add (new Node ("X"));
            right.Add (new Node ("Y"));
            right.Add (new Node ("Z"));

            var g = GraphGenerator.GenerateBarbell (left, right);
            var bridges = g.FindBridges ();
            Assert.AreEqual (1, bridges.Count);
        }

        [Test]
        public void BridgeTest2 () {

            var g = GraphGenerator.CreateComplete ("A", "B", "C", "D", "E");

            var bridges = g.FindBridges ();
            Assert.AreEqual (0, bridges.Count);
        }
    }

}