using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
namespace SharpGraph
{
    public class FordFulkersonTest
    {
        [Fact]
        public void TestFF1()
        {
            /*
             * This test is a copy of this problem:
             * https://en.wikipedia.org/wiki/Edmonds%E2%80%93Karp_algorithm
             */
            var nodes = new List<Node>(NodeGenerator.GenerateNodes(7));


            Edge e1 = new Edge(nodes[0], nodes[1]);
            Edge e2 = new Edge(nodes[0], nodes[3]);

            Edge e3 = new Edge(nodes[1], nodes[2]);
            Edge e3x = new Edge(nodes[1], nodes[4]);
            Edge e4 = new Edge(nodes[2], nodes[0]);
            Edge e5 = new Edge(nodes[2], nodes[3]);
            Edge e6 = new Edge(nodes[2], nodes[4]);
            Edge e7 = new Edge(nodes[3], nodes[5]);
            Edge e8 = new Edge(nodes[3], nodes[4]);
            Edge e9 = new Edge(nodes[4], nodes[6]);
            Edge e10 = new Edge(nodes[5], nodes[6]);



            List<Edge> edges = new List<Edge>();
            edges.Add(e1);
            edges.Add(e2);
            edges.Add(e3);
            edges.Add(e4);
            edges.Add(e3x);
            edges.Add(e5);
            edges.Add(e6);
            edges.Add(e7);
            edges.Add(e8);
            edges.Add(e9);
            edges.Add(e10);

            var g = new Graph(edges);
            g.AddComponent<EdgeCapacity>(e1).Capacity = 3;
            g.AddComponent<EdgeCapacity>(e2).Capacity = 3;
            g.AddComponent<EdgeCapacity>(e3).Capacity = 4;
            g.AddComponent<EdgeCapacity>(e3x).Capacity = 1;
            g.AddComponent<EdgeCapacity>(e4).Capacity = 3;
            g.AddComponent<EdgeCapacity>(e5).Capacity = 1;
            g.AddComponent<EdgeCapacity>(e6).Capacity = 2;
            g.AddComponent<EdgeCapacity>(e7).Capacity = 6;
            g.AddComponent<EdgeCapacity>(e8).Capacity = 2;
            g.AddComponent<EdgeCapacity>(e9).Capacity = 1;
            g.AddComponent<EdgeCapacity>(e10).Capacity = 9;


            g.FindMaxFlow(nodes[0], nodes[6]);

            Assert.Equal(2, g.GetComponent<EdgeCapacity>(e1).Flow, 0.001);
            Assert.Equal(3, g.GetComponent<EdgeCapacity>(e2).Flow, 0.001);
            Assert.Equal(2, g.GetComponent<EdgeCapacity>(e3).Flow, 0.001);
            Assert.Equal(0, g.GetComponent<EdgeCapacity>(e3x).Flow, 0.001);
            Assert.Equal(0, g.GetComponent<EdgeCapacity>(e4).Flow, 0.001);
            Assert.Equal(1, g.GetComponent<EdgeCapacity>(e5).Flow, 0.001);
            Assert.Equal(1, g.GetComponent<EdgeCapacity>(e6).Flow, 0.001);
            Assert.Equal(4, g.GetComponent<EdgeCapacity>(e7).Flow, 0.001);
            Assert.Equal(0, g.GetComponent<EdgeCapacity>(e8).Flow, 0.001);
            Assert.Equal(1, g.GetComponent<EdgeCapacity>(e9).Flow, 0.001);
            Assert.Equal(4, g.GetComponent<EdgeCapacity>(e10).Flow, 0.001);
        }

        [Fact]
        public void TestFF2()
        {
            Node n1 = new Node("A");
            Node n2 = new Node("B");

            Edge e1 = new Edge(n1, n2);


            List<Edge> edges = new List<Edge>();
            edges.Add(e1);

            var g = new Graph(edges);
            g.AddComponent<EdgeCapacity>(e1).Capacity = 3;


            g.FindMaxFlow(n1, n2);

            Assert.Equal(3, g.GetComponent<EdgeCapacity>(e1).Flow, 0.001);

        }
    }
}
