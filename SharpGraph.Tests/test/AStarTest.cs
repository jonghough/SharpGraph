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
    }

    public class SimpleHeuristic : IAStarHeuristic
    {
        public float GetHeuristic(Node t, Node goal)
        {
            return 2;
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
}
