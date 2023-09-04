using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace SharpGraph
{
    public class ConnectedTest
    {
        [Fact]
        public void ConnectedTest1()
        {
            var g = GraphGenerator.CreateComplete(NodeGenerator.GenerateNodes(2));
            Assert.True(g.IsConnected());
        }

        [Fact]
        public void ConnectedTest2()
        {
            var g = GraphGenerator.CreateComplete(NodeGenerator.GenerateNodes(3));
            Assert.True(g.IsConnected());
        }

        [Fact]
        public void ConnectedTest3()
        {
            var g = GraphGenerator.CreateComplete(NodeGenerator.GenerateNodes(5));
            Assert.True(g.IsConnected());
        }

        [Fact]
        public void ConnectedTest4()
        {
            var g = GraphGenerator.CreateComplete(NodeGenerator.GenerateNodes(10));
            Assert.True(g.IsConnected());
        }

        [Fact]
        public void ConnectedTest5()
        {

            /* nodes 0,1,2,3 are connected and nodes 4,5,6,7 are connected */
            HashSet<Node> nodes = NodeGenerator.GenerateNodes(8);
            List<Node> nodeList = new List<Node>(nodes);
            Edge wedge1 = new Edge(nodeList[0], nodeList[1]);
            Edge wedge2 = new Edge(nodeList[0], nodeList[2]);
            Edge wedge3 = new Edge(nodeList[0], nodeList[3]);
            Edge wedge4 = new Edge(nodeList[1], nodeList[2]);
            Edge wedge5 = new Edge(nodeList[1], nodeList[3]);

            Edge wedge6 = new Edge(nodeList[4], nodeList[5]);
            Edge wedge7 = new Edge(nodeList[4], nodeList[6]);
            Edge wedge8 = new Edge(nodeList[5], nodeList[6]);
            Edge wedge9 = new Edge(nodeList[5], nodeList[7]);
            Edge wedge10 = new Edge(nodeList[6], nodeList[7]);
            var edges = new List<Edge>();
            edges.Add(wedge1);
            edges.Add(wedge2);
            edges.Add(wedge3);
            edges.Add(wedge4);
            edges.Add(wedge5);
            edges.Add(wedge6);
            edges.Add(wedge7);
            edges.Add(wedge8);
            edges.Add(wedge9);
            edges.Add(wedge10);
            var g = new Graph(edges);
            //graph is not connected.
            Assert.False(g.IsConnected());
        }

        [Fact]
        public void ConnectedTest6()
        {
            // test edges with two obvious connected components is not connected.
            // then add a bridge edge and test that the result is connected.
            var edges = new List<Edge>{
                new Edge("a","b"),
                new Edge("b","c"),
                new Edge("a","c"),
                new Edge("d","e"),
                new Edge("e","f")
            };

            var g = new Graph(edges);
            Assert.False(g.IsConnected());
            // add "bridge" edge betwene 'c' and 'd'.
            g.AddEdge("c", "d");
            Assert.True(g.IsConnected());
        }

        [Fact]
        public void StronglyConnectedComponentTest1()
        {

            HashSet<Node> nodes = NodeGenerator.GenerateNodes(5);
            List<Node> nodeList = new List<Node>(nodes);
            Edge wedge1 = new Edge(nodeList[0], nodeList[1]);
            Edge wedge2 = new Edge(nodeList[1], nodeList[2]);
            Edge wedge3 = new Edge(nodeList[2], nodeList[3]);
            Edge wedge4 = new Edge(nodeList[3], nodeList[0]);
            Edge wedge5 = new Edge(nodeList[3], nodeList[4]);

            var edges = new List<Edge>();
            edges.Add(wedge1);
            edges.Add(wedge2);
            edges.Add(wedge3);
            edges.Add(wedge4);
            edges.Add(wedge5);

            var g = new Graph(edges);

            var ed1 = g.AddComponent<EdgeDirection>(wedge1);
            ed1.Direction = Direction.Forwards;

            var ed2 = g.AddComponent<EdgeDirection>(wedge2);
            ed2.Direction = Direction.Forwards;

            var ed3 = g.AddComponent<EdgeDirection>(wedge3);
            ed3.Direction = Direction.Forwards;

            var ed4 = g.AddComponent<EdgeDirection>(wedge4);
            ed4.Direction = Direction.Forwards;

            var ed5 = g.AddComponent<EdgeDirection>(wedge5);
            ed5.Direction = Direction.Forwards;
            //graph is not connected.
            Assert.True(g.IsConnected());

            List<Graph> sccl = g.FindStronglyConnectedComponents();
            Assert.True(sccl.Count == 2);
        }

        [Fact]
        public void StronglyConnectedComponentTest2()
        {

            HashSet<Node> nodes = NodeGenerator.GenerateNodes(5);
            List<Node> nodeList = new List<Node>(nodes);
            Edge wedge1 = new Edge(nodeList[0], nodeList[1]);
            Edge wedge2 = new Edge(nodeList[1], nodeList[2]);
            Edge wedge3 = new Edge(nodeList[2], nodeList[3]);
            Edge wedge4 = new Edge(nodeList[3], nodeList[0]);
            Edge wedge5 = new Edge(nodeList[3], nodeList[4]);

            var edges = new List<Edge>();
            edges.Add(wedge1);
            edges.Add(wedge2);
            edges.Add(wedge3);
            edges.Add(wedge4);
            edges.Add(wedge5);

            var g = new Graph(edges);

            var ed1 = g.AddComponent<EdgeDirection>(wedge1);
            ed1.Direction = Direction.Forwards;

            // backwards edge
            var ed2 = g.AddComponent<EdgeDirection>(wedge2);
            ed2.Direction = Direction.Backwards;

            var ed3 = g.AddComponent<EdgeDirection>(wedge3);
            ed3.Direction = Direction.Forwards;

            var ed4 = g.AddComponent<EdgeDirection>(wedge4);
            ed4.Direction = Direction.Forwards;

            var ed5 = g.AddComponent<EdgeDirection>(wedge5);
            ed5.Direction = Direction.Forwards;

            List<Graph> sccl = g.FindStronglyConnectedComponents();
            Assert.True(sccl.Count == 2);
        }

        [Fact]
        public void StronglyConnectedComponentTest3()
        {

            HashSet<Node> nodes = NodeGenerator.GenerateNodes(6);
            List<Node> nodeList = new List<Node>(nodes);
            Edge wedge1 = new Edge(nodeList[0], nodeList[1]);
            Edge wedge2 = new Edge(nodeList[1], nodeList[2]);
            Edge wedge3 = new Edge(nodeList[2], nodeList[0]);

            Edge wedge4 = new Edge(nodeList[3], nodeList[4]);
            Edge wedge5 = new Edge(nodeList[4], nodeList[5]);
            Edge wedge6 = new Edge(nodeList[5], nodeList[3]);

            Edge wedge7 = new Edge(nodeList[2], nodeList[3]);

            var edges = new List<Edge>();
            edges.Add(wedge1);
            edges.Add(wedge2);
            edges.Add(wedge3);
            edges.Add(wedge4);
            edges.Add(wedge5);
            edges.Add(wedge6);
            edges.Add(wedge7);

            var g = new Graph(edges);

            var ed1 = g.AddComponent<EdgeDirection>(wedge1);
            ed1.Direction = Direction.Forwards;

            var ed2 = g.AddComponent<EdgeDirection>(wedge2);
            ed2.Direction = Direction.Forwards;

            var ed3 = g.AddComponent<EdgeDirection>(wedge3);
            ed3.Direction = Direction.Forwards;

            // these three edges are backwars, but it should make no difference.
            var ed4 = g.AddComponent<EdgeDirection>(wedge4);
            ed4.Direction = Direction.Backwards;

            var ed5 = g.AddComponent<EdgeDirection>(wedge5);
            ed5.Direction = Direction.Backwards;

            var ed6 = g.AddComponent<EdgeDirection>(wedge6);
            ed6.Direction = Direction.Backwards;

            var ed7 = g.AddComponent<EdgeDirection>(wedge7);
            ed7.Direction = Direction.Forwards;

            List<Graph> sccl = g.FindStronglyConnectedComponents();
            Assert.True(sccl.Count == 2);
        }

        [Fact]
        public void WeaklyConnectedComponentTest1()
        {

            HashSet<Node> nodes = NodeGenerator.GenerateNodes(5);
            List<Node> nodeList = new List<Node>(nodes);
            Edge wedge1 = new Edge(nodeList[0], nodeList[1]);
            Edge wedge2 = new Edge(nodeList[1], nodeList[2]);
            Edge wedge3 = new Edge(nodeList[2], nodeList[3]);
            Edge wedge4 = new Edge(nodeList[3], nodeList[0]);
            Edge wedge5 = new Edge(nodeList[3], nodeList[4]);

            var edges = new List<Edge>();
            edges.Add(wedge1);
            edges.Add(wedge2);
            edges.Add(wedge3);
            edges.Add(wedge4);
            edges.Add(wedge5);

            var g = new Graph(edges);

            var ed1 = g.AddComponent<EdgeDirection>(wedge1);
            ed1.Direction = Direction.Forwards;

            var ed2 = g.AddComponent<EdgeDirection>(wedge2);
            ed2.Direction = Direction.Forwards;

            var ed3 = g.AddComponent<EdgeDirection>(wedge3);
            ed3.Direction = Direction.Forwards;

            var ed4 = g.AddComponent<EdgeDirection>(wedge4);
            ed4.Direction = Direction.Forwards;

            var ed5 = g.AddComponent<EdgeDirection>(wedge5);
            ed5.Direction = Direction.Forwards;
            //graph is not connected.
            Assert.True(g.IsConnected());

            List<Graph> wccl = g.FindWeaklyConnectedComponents();
            Assert.True(wccl.Count == 1); // ignoring direction only one component.
        }

        [Fact]
        public void WeaklyConnectedComponentTest2()
        {
            var g = new Graph();
            g.AddEdge("1", "2");
            g.AddEdge("1", "3");
            g.AddEdge("1", "4");
            g.AddEdge("2", "3");
            g.AddEdge("2", "4");
            // another component  
            g.AddEdge("5", "6");
            g.AddEdge("6", "7");

            g.GetEdges().ForEach(e => g.AddComponent<EdgeDirection>(e));
            List<Graph> wccl = g.FindWeaklyConnectedComponents();

            Assert.True(wccl.Count == 2);
        }

        [Fact]
        public void BridgeTest1()
        {

            var left = new HashSet<Node>();
            left.Add(new Node("A"));
            left.Add(new Node("B"));
            left.Add(new Node("C"));

            var right = new HashSet<Node>();
            right.Add(new Node("X"));
            right.Add(new Node("Y"));
            right.Add(new Node("Z"));

            var g = GraphGenerator.GenerateBarbell(left, right);
            var bridges = g.FindBridges();
            Assert.Single(bridges);
        }

        [Fact]
        public void BridgeTest2()
        {

            var g = GraphGenerator.CreateComplete("A", "B", "C", "D", "E");

            var bridges = g.FindBridges();
            Assert.Empty(bridges);
        }
    }

}