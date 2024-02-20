// <copyright file="SpanningTreeTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System.Collections.Generic;
using Xunit;

namespace SharpGraph
{
    public class SpanningTreeTest
    {
        [Fact]
        public void CompleteGraphSpanningTreeTest()
        {
            var nodes = NodeGenerator.GenerateNodes(10);
            var g = GraphGenerator.CreateComplete(nodes);
            Assert.Equal(9, g.GenerateSpanningTree().Count);
        }

        [Fact]
        public void MinimumSpanningTreeTest1()
        {
            var nodes = NodeGenerator.GenerateNodes(8);
            var nodeList = new List<Node>(nodes);
            var wedge1 = new Edge(nodeList[0], nodeList[1]);
            var wedge2 = new Edge(nodeList[0], nodeList[2]);
            var wedge3 = new Edge(nodeList[0], nodeList[7]);
            var wedge4 = new Edge(nodeList[1], nodeList[2]);
            var wedge5 = new Edge(nodeList[1], nodeList[3]);
            var wedge6 = new Edge(nodeList[1], nodeList[5]);
            var wedge7 = new Edge(nodeList[2], nodeList[4]);
            var wedge8 = new Edge(nodeList[2], nodeList[5]);
            var wedge9 = new Edge(nodeList[2], nodeList[6]);
            var wedge10 = new Edge(nodeList[3], nodeList[5]);
            var wedge11 = new Edge(nodeList[4], nodeList[6]);
            var wedge12 = new Edge(nodeList[5], nodeList[6]);
            var wedge13 = new Edge(nodeList[6], nodeList[7]);
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
            edges.Add(wedge11);
            edges.Add(wedge12);
            edges.Add(wedge13);
            var g = new Graph(edges);

            g.AddComponent<EdgeWeight>(wedge1).Weight = 2.4f;
            g.AddComponent<EdgeWeight>(wedge2).Weight = 23.4f;
            g.AddComponent<EdgeWeight>(wedge3).Weight = 13.5f;
            g.AddComponent<EdgeWeight>(wedge4).Weight = 66.4f;
            g.AddComponent<EdgeWeight>(wedge5).Weight = 19.14f;
            g.AddComponent<EdgeWeight>(wedge6).Weight = 7.905f;
            g.AddComponent<EdgeWeight>(wedge7).Weight = 17.05f;
            g.AddComponent<EdgeWeight>(wedge8).Weight = 100.8f;
            g.AddComponent<EdgeWeight>(wedge9).Weight = 88.8f;
            g.AddComponent<EdgeWeight>(wedge10).Weight = 10.8f;
            g.AddComponent<EdgeWeight>(wedge11).Weight = 20.8f;
            g.AddComponent<EdgeWeight>(wedge12).Weight = 14.2f;
            g.AddComponent<EdgeWeight>(wedge13).Weight = 10.55f;

            var span = g.GenerateMinimumSpanningTree();

            Assert.Equal(7, span.Count);
        }

        [Fact]
        public void MinimumSpanningTreeTest2()
        {
            var b1 = new Node("1");
            var b2 = new Node("2");
            var b3 = new Node("3");
            var wedge1 = new Edge(b1, b2);
            var wedge2 = new Edge(b1, b3);
            var wedge3 = new Edge(b2, b3);
            var edges = new List<Edge>();
            edges.Add(wedge1);
            edges.Add(wedge2);
            edges.Add(wedge3);

            var g = new Graph(edges);

            g.AddComponent<EdgeWeight>(wedge1).Weight = -5.1f;
            g.AddComponent<EdgeWeight>(wedge2).Weight = 0f;
            g.AddComponent<EdgeWeight>(wedge3).Weight = 3.5f;
            var span = g.GenerateMinimumSpanningTree();

            // minimum spanning tree should contain only edges 1 and 2, not edge 3.
            Assert.True(span.Contains(wedge1) && span.Contains(wedge2) && !span.Contains(wedge3));
        }

        [Fact]
        public void MinimumSpanningTreeTest3()
        {
            var b1 = new Node("1");
            var b2 = new Node("2");
            var b3 = new Node("3");
            var b4 = new Node("4");
            var wedge1 = new Edge(b1, b2);
            var wedge2 = new Edge(b1, b3);
            var wedge3 = new Edge(b2, b3);
            var wedge4 = new Edge(b2, b4);
            var wedge5 = new Edge(b3, b4);

            var edges = new List<Edge>();
            edges.Add(wedge1);
            edges.Add(wedge2);
            edges.Add(wedge3);
            edges.Add(wedge4);
            edges.Add(wedge5);

            var g = new Graph(edges);

            g.AddComponent<EdgeWeight>(wedge1).Weight = 0.1f;
            g.AddComponent<EdgeWeight>(wedge2).Weight = 0.14f;
            g.AddComponent<EdgeWeight>(wedge3).Weight = 0.15f;
            g.AddComponent<EdgeWeight>(wedge4).Weight = 0.15f;
            g.AddComponent<EdgeWeight>(wedge5).Weight = 0.15f;

            var span = g.GenerateMinimumSpanningTree();
            Assert.Equal(3, span.Count);

            // minimum spanning tree should contain only edges 1 and 2
            Assert.True(span.Contains(wedge1) && span.Contains(wedge2));

            // only contains one of edge 3,4,5
            Assert.True(span.Contains(wedge3) ^ span.Contains(wedge4) ^ span.Contains(wedge5));
        }

        [Fact]
        public void MinimumSpanningTreeTestForDisconnected()
        {
            var g = new Graph();
            g.AddEdge("A", "B");
            g.AddEdge("C", "D");
            g.AddComponent<EdgeWeight>(g.GetEdge("A", "B").Value).Weight = 2;
            g.AddComponent<EdgeWeight>(g.GetEdge("C", "D").Value).Weight = 2;

            var span = g.GenerateMinimumSpanningTree();
            Assert.Equal(2, span.Count);
            Assert.Throws<MinimumSpanningTreeException>(
                () => g.GenerateMinimumSpanningTree(mustBeConnected: true)
            );
        }

        [Fact]
        public void MinimumSpanningTreeTestForUnweighted1()
        {
            var g = GraphGenerator.GenerateCycle(5);

            Assert.Throws<MinimumSpanningTreeException>(
                () => g.GenerateMinimumSpanningTree(mustBeConnected: false)
            );
        }

        [Fact]
        public void MinimumSpanningTreePrimTest3()
        {
            var b1 = new Node("1");
            var b2 = new Node("2");
            var b3 = new Node("3");
            var b4 = new Node("4");
            var wedge1 = new Edge(b1, b2);
            var wedge2 = new Edge(b1, b3);
            var wedge3 = new Edge(b2, b3);
            var wedge4 = new Edge(b2, b4);
            var wedge5 = new Edge(b3, b4);

            var edges = new List<Edge>();
            edges.Add(wedge1);
            edges.Add(wedge2);
            edges.Add(wedge3);
            edges.Add(wedge4);
            edges.Add(wedge5);

            var g = new Graph(edges);

            g.AddComponent<EdgeWeight>(wedge1).Weight = 0.1f;
            g.AddComponent<EdgeWeight>(wedge2).Weight = 0.14f;
            g.AddComponent<EdgeWeight>(wedge3).Weight = 0.15f;
            g.AddComponent<EdgeWeight>(wedge4).Weight = 0.15f;
            g.AddComponent<EdgeWeight>(wedge5).Weight = 0.15f;

            var span = g.GenerateMinimumSpanningTree(SpanningTreeAlgorithm.Prim);

            Assert.Equal(3, span.Count);

            // minimum spanning tree should contain only edges 1 and 2
            Assert.True(span.Contains(wedge1) && span.Contains(wedge2));

            // only contains one of edge 3,4,5
            Assert.True(span.Contains(wedge3) ^ span.Contains(wedge4) ^ span.Contains(wedge5));
        }

        [Fact]
        public void MinimumSpanningTreePrimTest1()
        {
            var nodes = NodeGenerator.GenerateNodes(8);
            var nodeList = new List<Node>(nodes);
            var wedge1 = new Edge(nodeList[0], nodeList[1]);
            var wedge2 = new Edge(nodeList[0], nodeList[2]);
            var wedge3 = new Edge(nodeList[0], nodeList[7]);
            var wedge4 = new Edge(nodeList[1], nodeList[2]);
            var wedge5 = new Edge(nodeList[1], nodeList[3]);
            var wedge6 = new Edge(nodeList[1], nodeList[5]);
            var wedge7 = new Edge(nodeList[2], nodeList[4]);
            var wedge8 = new Edge(nodeList[2], nodeList[5]);
            var wedge9 = new Edge(nodeList[2], nodeList[6]);
            var wedge10 = new Edge(nodeList[3], nodeList[5]);
            var wedge11 = new Edge(nodeList[4], nodeList[6]);
            var wedge12 = new Edge(nodeList[5], nodeList[6]);
            var wedge13 = new Edge(nodeList[6], nodeList[7]);
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
            edges.Add(wedge11);
            edges.Add(wedge12);
            edges.Add(wedge13);
            var g = new Graph(edges);

            g.AddComponent<EdgeWeight>(wedge1).Weight = 2.4f;
            g.AddComponent<EdgeWeight>(wedge2).Weight = 23.4f;
            g.AddComponent<EdgeWeight>(wedge3).Weight = 13.5f;
            g.AddComponent<EdgeWeight>(wedge4).Weight = 66.4f;
            g.AddComponent<EdgeWeight>(wedge5).Weight = 19.14f;
            g.AddComponent<EdgeWeight>(wedge6).Weight = 7.905f;
            g.AddComponent<EdgeWeight>(wedge7).Weight = 17.05f;
            g.AddComponent<EdgeWeight>(wedge8).Weight = 100.8f;
            g.AddComponent<EdgeWeight>(wedge9).Weight = 88.8f;
            g.AddComponent<EdgeWeight>(wedge10).Weight = 10.8f;
            g.AddComponent<EdgeWeight>(wedge11).Weight = 20.8f;
            g.AddComponent<EdgeWeight>(wedge12).Weight = 14.2f;
            g.AddComponent<EdgeWeight>(wedge13).Weight = 10.55f;

            var span = g.GenerateMinimumSpanningTree(SpanningTreeAlgorithm.Prim);

            Assert.Equal(7, span.Count);
        }

        [Fact]
        public void MinimumSpanningTreePrimTest2()
        {
            var b1 = new Node("1");
            var b2 = new Node("2");
            var b3 = new Node("3");
            var wedge1 = new Edge(b1, b2);
            var wedge2 = new Edge(b1, b3);
            var wedge3 = new Edge(b2, b3);
            var edges = new List<Edge>();
            edges.Add(wedge1);
            edges.Add(wedge2);
            edges.Add(wedge3);

            var g = new Graph(edges);

            g.AddComponent<EdgeWeight>(wedge1).Weight = -5.1f;
            g.AddComponent<EdgeWeight>(wedge2).Weight = 0f;
            g.AddComponent<EdgeWeight>(wedge3).Weight = 3.5f;
            var span = g.GenerateMinimumSpanningTree(SpanningTreeAlgorithm.Prim);

            // minimum spanning tree should contain only edges 1 and 2, not edge 3.
            Assert.True(span.Contains(wedge1) && span.Contains(wedge2) && !span.Contains(wedge3));
        }

        [Fact]
        public void MinimumSpanningTreeBoruvkaTest1()
        {
            var nodes = NodeGenerator.GenerateNodes(8);
            var nodeList = new List<Node>(nodes);
            var wedge1 = new Edge(nodeList[0], nodeList[1]);
            var wedge2 = new Edge(nodeList[0], nodeList[2]);
            var wedge3 = new Edge(nodeList[0], nodeList[7]);
            var wedge4 = new Edge(nodeList[1], nodeList[2]);
            var wedge5 = new Edge(nodeList[1], nodeList[3]);
            var wedge6 = new Edge(nodeList[1], nodeList[5]);
            var wedge7 = new Edge(nodeList[2], nodeList[4]);
            var wedge8 = new Edge(nodeList[2], nodeList[5]);
            var wedge9 = new Edge(nodeList[2], nodeList[6]);
            var wedge10 = new Edge(nodeList[3], nodeList[5]);
            var wedge11 = new Edge(nodeList[4], nodeList[6]);
            var wedge12 = new Edge(nodeList[5], nodeList[6]);
            var wedge13 = new Edge(nodeList[6], nodeList[7]);
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
            edges.Add(wedge11);
            edges.Add(wedge12);
            edges.Add(wedge13);
            var g = new Graph(edges);

            g.AddComponent<EdgeWeight>(wedge1).Weight = 2.4f;
            g.AddComponent<EdgeWeight>(wedge2).Weight = 23.4f;
            g.AddComponent<EdgeWeight>(wedge3).Weight = 13.5f;
            g.AddComponent<EdgeWeight>(wedge4).Weight = 66.4f;
            g.AddComponent<EdgeWeight>(wedge5).Weight = 19.14f;
            g.AddComponent<EdgeWeight>(wedge6).Weight = 7.905f;
            g.AddComponent<EdgeWeight>(wedge7).Weight = 17.05f;
            g.AddComponent<EdgeWeight>(wedge8).Weight = 100.8f;
            g.AddComponent<EdgeWeight>(wedge9).Weight = 88.8f;
            g.AddComponent<EdgeWeight>(wedge10).Weight = 10.8f;
            g.AddComponent<EdgeWeight>(wedge11).Weight = 20.8f;
            g.AddComponent<EdgeWeight>(wedge12).Weight = 14.2f;
            g.AddComponent<EdgeWeight>(wedge13).Weight = 10.55f;

            var span = g.GenerateMinimumSpanningTree(SpanningTreeAlgorithm.Boruvka);

            Assert.Equal(7, span.Count);
        }

        [Fact]
        public void MinimumSpanningTreeBoruvkaTest2()
        {
            var b1 = new Node("1");
            var b2 = new Node("2");
            var b3 = new Node("3");
            var wedge1 = new Edge(b1, b2);
            var wedge2 = new Edge(b1, b3);
            var wedge3 = new Edge(b2, b3);
            var edges = new List<Edge>();
            edges.Add(wedge1);
            edges.Add(wedge2);
            edges.Add(wedge3);

            var g = new Graph(edges);

            g.AddComponent<EdgeWeight>(wedge1).Weight = -5.1f;
            g.AddComponent<EdgeWeight>(wedge2).Weight = 0f;
            g.AddComponent<EdgeWeight>(wedge3).Weight = 3.5f;
            var span = g.GenerateMinimumSpanningTree(SpanningTreeAlgorithm.Boruvka);

            // minimum spanning tree should contain only edges 1 and 2, not edge 3.
            Assert.True(span.Contains(wedge1) && span.Contains(wedge2) && !span.Contains(wedge3));
        }

        [Fact]
        public void SpanningTreeTest1()
        {
            var b1 = new Node("1");
            var b2 = new Node("2");
            var b3 = new Node("3");
            var wedge1 = new Edge(b1, b2);
            var wedge2 = new Edge(b1, b3);
            var wedge3 = new Edge(b2, b3);
            var edges = new List<Edge>();
            edges.Add(wedge1);
            edges.Add(wedge2);
            edges.Add(wedge3);

            var g = new Graph(edges);
            var span = g.GenerateSpanningTree();
            Assert.Equal(2, span.Count);
        }

        [Fact]
        public void SpanningTreeTestCyclic()
        {
            var g = GraphGenerator.GenerateCycle(10);
            var span = g.GenerateSpanningTree();
            Assert.Equal(9, span.Count);
        }

        [Fact]
        public void SpanningTreeTestComplete()
        {
            var g = GraphGenerator.CreateComplete(15);
            var span = g.GenerateSpanningTree();
            Assert.Equal(14, span.Count);
        }

        [Fact]
        public void SpanningTreeTestDisconnected()
        {
            var g = new Graph();
            g.AddEdge("A", "B");
            g.AddEdge("B", "C");
            g.AddEdge("D", "E");
            var span = g.GenerateSpanningTree();
            Assert.Equal(3, span.Count);
        }
    }
}
