// <copyright file="AStarTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;

// <copyright file="AStarTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>
using System.Collections.Generic;
using Xunit;

namespace SharpGraph
{
    public class AStarTest
    {
        [Fact]
        public void AStarTest1()
        {
            var nodes = NodeGenerator.GenerateNodes(10);
            var graph = GraphGenerator.GenerateRandomGraph(nodes, 0.6f);

            GraphGenerator.GenerateRandomWeights(graph, 0, 10);
            var g = graph;
            var list = new List<Node>(nodes);
            var n1 = list[0];
            var n2 = list[1];

            var minPath = g.FindMinPath(n1, n2, new SimpleHeuristic());
            var minPathSet = new HashSet<Node>(minPath);

            //----assert there are no loops ---
            Assert.Equal(minPath.Count, minPathSet.Count);
        }

        [Fact]
        public void AStarTest2()
        {
            // A -> B -> F should be shortest path from A to F.
            var eAB = new Edge("A", "B");
            var eAC = new Edge("A", "C");
            var eCD = new Edge("C", "D");
            var eDE = new Edge("D", "E");
            var eAF = new Edge("A", "F");
            var eBF = new Edge("B", "F");
            var eDF = new Edge("D", "F");

            var list = new List<Edge>();
            list.Add(eAB);
            list.Add(eAC);
            list.Add(eCD);
            list.Add(eDE);
            list.Add(eAF);
            list.Add(eBF);
            list.Add(eDF);

            var g = new Graph(list);
            g.AddComponent<EdgeWeight>(eAB).Weight = 3;
            g.AddComponent<EdgeWeight>(eAC).Weight = 6;
            g.AddComponent<EdgeWeight>(eCD).Weight = 1;
            g.AddComponent<EdgeWeight>(eDE).Weight = 10;
            g.AddComponent<EdgeWeight>(eAF).Weight = 12;
            g.AddComponent<EdgeWeight>(eBF).Weight = 3.5f;
            g.AddComponent<EdgeWeight>(eDF).Weight = 6.5f;

            var path = g.FindMinPath(new Node("A"), new Node("F"), new TestHeuristic(5));

            Assert.True(
                path.Contains(new Node("A"))
                    && path.Contains(new Node("B"))
                    && path.Contains(new Node("F"))
                    && path.Count == 3
            );
        }

        [Fact]
        public void AStarTest3()
        {
            // A -> F -> D -> H should be shortest path from A to H
            var b1 = new Node("A");
            var b2 = new Node("B");
            var b3 = new Node("C");
            var b4 = new Node("D");
            var b5 = new Node("E");
            var b6 = new Node("F");
            var b7 = new Node("G");
            var b8 = new Node("H");

            var eAB = new Edge(b1, b2);
            var eAG = new Edge(b1, b7);
            var eCD = new Edge(b3, b4);
            var eDH = new Edge(b4, b8);
            var eAF = new Edge(b1, b6);
            var eBF = new Edge(b2, b6);
            var eDF = new Edge(b4, b6);
            var eCG = new Edge(b3, b7);
            var eBC = new Edge(b2, b3);
            var eCE = new Edge(b3, b5);

            var list = new List<Edge>();
            list.Add(eAB);
            list.Add(eAG);
            list.Add(eCD);
            list.Add(eDH);
            list.Add(eAF);
            list.Add(eBF);
            list.Add(eDF);
            list.Add(eCG);
            list.Add(eBC);
            list.Add(eCE);

            var g = new Graph(list);

            g.AddComponent<EdgeWeight>(eAB).Weight = 10;
            g.AddComponent<EdgeWeight>(eAG).Weight = 14;
            g.AddComponent<EdgeWeight>(eCD).Weight = 5;
            g.AddComponent<EdgeWeight>(eDH).Weight = 5.5f;
            g.AddComponent<EdgeWeight>(eAF).Weight = 12;
            g.AddComponent<EdgeWeight>(eBF).Weight = 3.5f;
            g.AddComponent<EdgeWeight>(eDF).Weight = 1.5f;
            g.AddComponent<EdgeWeight>(eCG).Weight = 11.5f;
            g.AddComponent<EdgeWeight>(eBC).Weight = 6.5f;
            g.AddComponent<EdgeWeight>(eCE).Weight = 6.5f;
            var path = g.FindMinPath(b1, b8, new TestHeuristic(5));

            Assert.True(
                path.Contains(b1)
                    && path.Contains(b4)
                    && path.Contains(b6)
                    && path.Contains(b8)
                    && path.Count == 4
            );
        }

        [Fact]
        public void AStarTest4()
        {
            // A ->B should be shortest path from A to B
            var b1 = new Node("A");
            var b2 = new Node("B");

            var eAB = new Edge(b1, b2);

            var list = new List<Edge>();
            list.Add(eAB);

            var g = new Graph(list);
            g.AddComponent<EdgeWeight>(eAB).Weight = 10;
            var path = g.FindMinPath(b1, b2, new TestHeuristic(5));

            Assert.True(path.Contains(b1) && path.Contains(b2) && path.Count == 2);
        }

        [Fact]
        public void AStarTest5()
        {
            // A ->B path does not exist
            var b1 = new Node("A");
            var b2 = new Node("B");
            var nodeSet = new HashSet<Node>();
            nodeSet.Add(b1);
            nodeSet.Add(b2);

            var list = new List<Edge>();

            var g = new Graph(list, nodeSet);
            var path = g.FindMinPath(b1, b2, new TestHeuristic(5));

            Assert.True(path.Count == 1);
        }

        [Fact]
        public void AStarTestWithEuclideanHeuristic()
        {
            var g = new Graph("A->B,B->C,C->D,D->E,E->F,F->G,G->H,C->G, B->F");

            this.SetNodePosition(g, "A", 0, 0);
            this.SetNodePosition(g, "B", 10, 0);
            this.SetNodePosition(g, "C", 10, 10);
            this.SetNodePosition(g, "D", 20, 10);
            this.SetNodePosition(g, "E", 20, 20);
            this.SetNodePosition(g, "F", 7, 30);
            this.SetNodePosition(g, "G", 27, 40);
            this.SetNodePosition(g, "H", 5, 35);

            this.SetEdgeWeight(g, g.GetEdge("A", "B").Value, 12.0f);
            this.SetEdgeWeight(g, g.GetEdge("B", "C").Value, 15.0f);
            this.SetEdgeWeight(g, g.GetEdge("C", "D").Value, 4.0f);
            this.SetEdgeWeight(g, g.GetEdge("D", "E").Value, 25.0f);
            this.SetEdgeWeight(g, g.GetEdge("E", "F").Value, 20.0f);
            this.SetEdgeWeight(g, g.GetEdge("F", "G").Value, 1.0f);
            this.SetEdgeWeight(g, g.GetEdge("G", "H").Value, 22.0f);
            this.SetEdgeWeight(g, g.GetEdge("C", "G").Value, 6.0f);
            this.SetEdgeWeight(g, g.GetEdge("B", "F").Value, 500.0f);

            var path = g.FindMinPath(
                g.GetNode("A").Value,
                g.GetNode("H").Value,
                new EuclideanHeuristic(g)
            );

            // expect shortest route A,B,C,G,H
            Assert.True(path.Count == 5);
            Assert.True(path[0] == g.GetNode("A").Value);
            Assert.True(path[1] == g.GetNode("B").Value);
            Assert.True(path[2] == g.GetNode("C").Value);
            Assert.True(path[3] == g.GetNode("G").Value);
            Assert.True(path[4] == g.GetNode("H").Value);

            // we now make C -> G prohibitively expensive, and B -> F very cheap, so if we run A* again
            // a different route will be found.
            this.SetEdgeWeight(g, g.GetEdge("C", "G").Value, 6000.0f);
            this.SetEdgeWeight(g, g.GetEdge("B", "F").Value, 20.0f);

            var xpath = g.FindMinPath(
                g.GetNode("A").Value,
                g.GetNode("H").Value,
                new EuclideanHeuristic(g)
            );

            // expect shortest route A,B,F,G,H
            Assert.True(xpath.Count == 5);
            Assert.True(xpath[0] == g.GetNode("A").Value);
            Assert.True(xpath[1] == g.GetNode("B").Value);
            Assert.True(xpath[2] == g.GetNode("F").Value);
            Assert.True(xpath[3] == g.GetNode("G").Value);
            Assert.True(xpath[4] == g.GetNode("H").Value);
        }

        private void SetNodePosition(Graph g, string nodeLabel, float x, float y)
        {
            var pos = g.AddComponent<Position>(g.GetNode(nodeLabel).Value);
            pos.X = x;
            pos.Y = y;
        }

        private void SetEdgeWeight(Graph g, Edge e, float distanceAddition)
        {
            var w = g.AddComponent<EdgeWeight>(e);
            var f = g.GetComponent<Position>(e.From());
            var t = g.GetComponent<Position>(e.To());
            var dist = (float)Math.Sqrt(((f.X - t.X) * (f.X - t.X)) + ((f.Y - t.Y) * (f.Y - t.Y)));

            // add `distanceAddition` as we can imagine the "road" from the two
            // node ("cities") is not a straight line, i.e.
            // edge weight ("road distance") is strictly greater than euclidean distance.
            w.Weight = dist + distanceAddition;
        }
    }

    public class SimpleHeuristic : IAStarHeuristic
    {
        public float GetHeuristic(Node t, Node goal)
        {
            return 2;
        }
    }

    public class EuclideanHeuristic : IAStarHeuristic
    {
        private Graph graph;

        public EuclideanHeuristic(Graph graph)
        {
            this.graph = graph;
        }

        public float GetHeuristic(Node t, Node goal)
        {
            var posT = this.graph.GetComponent<Position>(t);
            var posGoal = this.graph.GetComponent<Position>(goal);

            return (float)
                Math.Sqrt(
                    ((posT.X - posGoal.X) * (posT.X - posGoal.X))
                        + ((posT.Y - posGoal.Y) * (posT.Y - posGoal.Y))
                );
        }
    }

    internal class TestHeuristic : IAStarHeuristic
    {
        private readonly float val;

        public TestHeuristic(float val)
        {
            this.val = val;
        }

        public float GetHeuristic(Node t, Node goal)
        {
            return this.val;
        }
    }

    public class Position : NodeComponent
    {
        public float X;
        public float Y;

        public override void Copy(NodeComponent nodeComponent)
        {
            var pos = nodeComponent as Position;
            pos.X = this.X;
            pos.Y = this.Y;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{1}, {2}", this.X, this.Y);
        }
    }
}
