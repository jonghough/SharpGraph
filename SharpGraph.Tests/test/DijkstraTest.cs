// <copyright file="DijkstraTest.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System.Collections.Generic;
using Xunit;

namespace SharpGraph
{
    public class DijkstraTest
    {
        [Fact]
        public void DijkstraTest1()
        {
            var nodes = NodeGenerator.GenerateNodes(10);
            var graph = GraphGenerator.GenerateRandomGraph(nodes, 0.6f);

            GraphGenerator.GenerateRandomWeights(graph, 0, 10);
            var g = graph;
            var list = new List<Node>(nodes);
            var n1 = list[0];
            var n2 = list[1];

            var minPath = g.FindMinPath(n1, n2);
            var minPathSet = new HashSet<Node>(minPath);

            //----assert there are no loops ---
            Assert.Equal(minPath.Count, minPathSet.Count);
        }

        [Fact]
        public void DijkstraTest2()
        {
            // A -> B -> F should be shortest path from A to F.
            var b1 = new Node("A");
            var b2 = new Node("B");
            var b3 = new Node("C");
            var b4 = new Node("D");
            var b5 = new Node("E");
            var b6 = new Node("F");

            var eAB = new Edge(b1, b2);
            var eAC = new Edge(b1, b3);
            var eCD = new Edge(b3, b4);
            var eDE = new Edge(b4, b5);
            var eAF = new Edge(b1, b6);
            var eBF = new Edge(b2, b6);
            var eDF = new Edge(b4, b6);

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
            var path = g.FindMinPath(b1, b6);

            Assert.True(
                path.Contains(b1) && path.Contains(b2) && path.Contains(b6) && path.Count == 3
            );
        }

        [Fact]
        public void DijkstraTest3()
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
            var path = g.FindMinPath(b1, b8);

            Assert.True(
                path.Contains(b1)
                    && path.Contains(b4)
                    && path.Contains(b6)
                    && path.Contains(b8)
                    && path.Count == 4
            );
        }

        [Fact]
        public void DijkstraTest4()
        {
            // A ->B should be shortest path from A to B
            var b1 = new Node("A");
            var b2 = new Node("B");

            var eAB = new Edge(b1, b2);

            var list = new List<Edge>();
            list.Add(eAB);

            var g = new Graph(list);
            g.AddComponent<EdgeWeight>(eAB).Weight = 10;
            var path = g.FindMinPath(b1, b2);

            Assert.True(path.Contains(b1) && path.Contains(b2) && path.Count == 2);
        }

        [Fact]
        public void DijkstraTest5()
        {
            // A ->B path does not exist
            var b1 = new Node("A");
            var b2 = new Node("B");
            var nodeSet = new HashSet<Node>();
            nodeSet.Add(b1);
            nodeSet.Add(b2);

            var eAB = new Edge(b1, b2);

            var list = new List<Edge>();
            list.Add(eAB);

            var g = new Graph(list, nodeSet);
            g.AddComponent<EdgeWeight>(eAB).Weight = 10;
            var path = g.FindMinPath(b1, b2);

            Assert.True(path.Count == 2);
        }
    }
}
