// <copyright file="Graph.SpanningTree.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public enum SpanningTreeAlgorithm
    {
        Boruvka,
        Kruskal,
        Prim,
    }

    public class MinimumSpanningTreeException : Exception
    {
        public MinimumSpanningTreeException(string msg)
            : base(msg) { }
    }

    public partial class Graph
    {
        /// <summary>
        /// Generates a minimum spanning tree on the graph. The weight is defined using the.
        /// <code>EdgeWeight</code> component's. <code>Weight</code> value. The graph is assumed to be weighted. If any edge on the
        /// graph does not have a weight, a. <code>MinimumSpanningTreeException</code> will be thrown.
        /// The algorithm can be defined by the callee. Either <i>Kruskal</i> or <i>Prim</i> algorithms are available.
        /// The. <code>msutBeConnected</code> flag, if set to true, will cause this method to throw a. <code>MinimumSpanningTreeException</code>
        /// if the graph is not connected. By default this value is false.
        /// The default is Kruskal's algorithm.
        /// <example>
        /// Given a weighted graph, g.
        /// <code>
        /// var mst = g.GenerateMinimumSpanningTree(SpanningTreeAlgorithm.Prim);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="spanningTreeAlgorithm">Algorithm to use.</param>
        /// <param name="mustBeConnected">Flag. If true the graph must be connected.</param>
        /// <returns>List of edgese, representing the minimum spanning tree.</returns>
        public List<Edge> GenerateMinimumSpanningTree(
            SpanningTreeAlgorithm spanningTreeAlgorithm = SpanningTreeAlgorithm.Kruskal,
            bool mustBeConnected = false
        )
        {
            if (mustBeConnected && !this.IsConnected())
            {
                throw new MinimumSpanningTreeException("Graph is not connected.");
            }

            foreach (var edge in this.edges)
            {
                if (this.GetComponent<EdgeWeight>(edge) == null)
                {
                    throw new MinimumSpanningTreeException(
                        string.Format(
                            "Edge {0} does not have an attached EdgeWeight component.",
                            edge
                        )
                    );
                }
            }

            switch (spanningTreeAlgorithm)
            {
                case SpanningTreeAlgorithm.Boruvka:
                    return this.GenerateMinimumSpanningTreeBoruvka();
                case SpanningTreeAlgorithm.Kruskal:
                    return this.GenerateMinimumSpanningTreeKruskal();
                case SpanningTreeAlgorithm.Prim:
                    return this.GenerateMinimumSpanningTreePrim();
                default:
                    throw new MinimumSpanningTreeException(
                        "Unknown minimum spanning tree algorithm."
                    );
            }
        }

        /// <summary>
        /// Generates a spanning tree on the graph. THe graph need not be weighted or connected.
        /// The resulting spanning tree, need not be minimum in any sense, and may not be unique.
        /// <example>
        /// Given a graph, g.
        /// <code>
        /// var mst = g.GenerateSpanningTree();
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>Spanning tree, as a list of edges.</returns>
        public List<Edge> GenerateSpanningTree()
        {
            var edges = this.GetEdges();
            _ = new List<Edge>(edges);
            var stEdges = new List<Edge>();
            var ds = new Dictionary<Node, DisjointSet>();
            var nodeList = this.nodes.ToList();

            foreach (var edge in edges)
            {
                var (x, b1) = this.Find(ds, edge.From());
                var (y, b2) = this.Find(ds, edge.To());
                if (!b1)
                {
                    ds[x] = new DisjointSet(x, 0);
                }

                if (!b2)
                {
                    ds[y] = new DisjointSet(y, 0);
                }

                if (x != y)
                {
                    stEdges.Add(edge);
                    this.Union(ds, x, y);
                }
            }

            return stEdges;
        }

        internal (Node, bool) Find(Dictionary<Node, DisjointSet> subsets, Node node)
        {
            if (!subsets.ContainsKey(node))
            {
                return (node, false);
            }

            if (subsets[node].Parent != node)
            {
                var a = this.Find(subsets, subsets[node].Parent);
                if (a.Item2)
                {
                    subsets[node].Parent = a.Item1;
                }
            }

            return (subsets[node].Parent, true);
        }

        internal void Union(Dictionary<Node, DisjointSet> subsets, Node a, Node b)
        {
            var rootA = this.Find(subsets, a);
            var rootB = this.Find(subsets, b);

            if (subsets[rootA.Item1].Rank < subsets[rootB.Item1].Rank)
            {
                subsets[rootA.Item1].Parent = rootB.Item1;
            }
            else
            {
                subsets[rootB.Item1].Parent = rootA.Item1;
                if (subsets[rootB.Item1].Rank == subsets[rootA.Item1].Rank)
                {
                    subsets[rootA.Item1].Rank++;
                }
            }
        }

        /// <summary>
        /// Generates a minimum spanning tree on the graph. The weight is defined using the.
        /// <code>EdgeWeight</code> component's. <code>Weight</code> value.
        /// The algorithm uses <i>Boruvka's Algorithm</i>. The time complexity is
        /// ~O(E log N), where E is number of edges, N is number of nodes.
        /// </summary>
        /// <returns>Minimum spanning tree, as a list of edges.</returns>
        private List<Edge> GenerateMinimumSpanningTreeBoruvka()
        {
            var ds = new Dictionary<Node, DisjointSet>();
            var nodeList = this.nodes.ToList();
            var stEdges = new List<Edge>();
            var minimumEdges = new Dictionary<Node, float>();

            var nTrees = 0;

            while (nTrees < nodeList.Count - 1)
            {
                foreach (var edge in this.edges)
                {
                    var (x, b1) = this.Find(ds, edge.From());
                    var (y, b2) = this.Find(ds, edge.To());
                    if (!b1)
                    {
                        ds[x] = new DisjointSet(x, 0);
                    }

                    if (!b2)
                    {
                        ds[y] = new DisjointSet(y, 0);
                    }

                    if (x != y)
                    {
                        var edgeWeight = this.GetComponent<EdgeWeight>(edge).Weight;
                        if (
                            (minimumEdges.ContainsKey(x) && edgeWeight <= minimumEdges[x])
                            || !minimumEdges.ContainsKey(x)
                        )
                        {
                            minimumEdges[x] = edgeWeight;
                        }

                        if (
                            (minimumEdges.ContainsKey(y) && edgeWeight <= minimumEdges[y])
                            || !minimumEdges.ContainsKey(y)
                        )
                        {
                            minimumEdges[y] = edgeWeight;
                        }

                        stEdges.Add(edge);
                        nTrees++;
                        this.Union(ds, x, y);
                    }
                }
            }

            return stEdges;
        }

        /// <summary>
        /// Generates a minimum spanning tree on the graph. The weight is defined using the.
        /// <code>EdgeWeight</code> component's. <code>Weight</code> value.
        /// The algorithm uses <i>Kruskal's Algorithm</i>. The time complexity is
        /// ~O(E log N), where E is number of edges, N is number of nodes.
        /// </summary>
        /// <returns>Minimum spanning tree, as a list of edges.</returns>
        private List<Edge> GenerateMinimumSpanningTreeKruskal()
        {
            var edges = this.GetEdges();
            edges.Sort(
                (Edge x, Edge y) =>
                {
                    return this.GetComponent<EdgeWeight>(x)
                        .Weight.CompareTo(this.GetComponent<EdgeWeight>(y).Weight);
                }
            );
            var stEdges = new List<Edge>();
            var ds = new Dictionary<Node, DisjointSet>();

            foreach (var edge in edges)
            {
                var (x, b1) = this.Find(ds, edge.From());
                var (y, b2) = this.Find(ds, edge.To());
                if (!b1)
                {
                    ds[x] = new DisjointSet(x, 0);
                }

                if (!b2)
                {
                    ds[y] = new DisjointSet(y, 0);
                }

                if (x != y)
                {
                    stEdges.Add(edge);
                    this.Union(ds, x, y);
                }
            }

            return stEdges;
        }

        /// <summary>
        /// Generates a minimum spanning tree on the graph. The weight is defined using the.
        /// <code>EdgeWeight</code> component's. <code>Weight</code> value.
        /// The algorithm uses <i>Prim's Algorithm</i>. The time complexity is
        /// ~O(E log N), where E is number of edges, N is number of nodes.
        /// </summary>
        /// <returns>Minimum spanning tree, as a list of edges.</returns>
        private List<Edge> GenerateMinimumSpanningTreePrim()
        {
            var treeEdges = new List<Edge>();
            var r = new System.Random();
            var minDistMap = new Dictionary<Node, (float, Edge)>();
            this.nodes
                .ToList()
                .ForEach(n =>
                {
                    minDistMap[n] = (float.PositiveInfinity, this.GetIncidentEdges(n)[0]); // (A,B) is a dummy edge
                });

            var dPQ = new C5.IntervalHeap<Node>(new PrimNodeComparer(minDistMap));
            dPQ.AddAll(this.nodes);
            var next = r.Next(this.nodes.Count);
            var node = this.nodes.ToList()[next];

            var treeNodes = new HashSet<Node>();
            while (treeNodes.Count < this.nodes.Count)
            {
                var adjacentNodes = this.GetAdjacent(node);
                adjacentNodes.ForEach(n =>
                {
                    var hs = new HashSet<Node>();
                    hs.Add(node);
                    hs.Add(n);
                    var edge = this.GetEdge(hs).Value; // we can safely assume not null
                    var w = this.GetComponent<EdgeWeight>(edge).Weight;
                    if (treeNodes.Contains(n) == false && minDistMap[n].Item1 > w)
                    {
                        minDistMap[n] = (w, edge);
                    }
                });
                treeNodes.Add(node);

                dPQ = new C5.IntervalHeap<Node>(new PrimNodeComparer(minDistMap));
                dPQ.AddAll(this.nodes.Except(treeNodes));
                while (dPQ.Count > 0)
                {
                    node = dPQ.DeleteMin();

                    if (!treeNodes.Contains(node))
                    {
                        break; // if node is not in tree nodes we continue in outer loop
                    }
                }

                if (dPQ.Count == 0)
                {
                    break;
                }
            }

            return minDistMap.Values
                .ToList()
                .Where(i => i.Item1 < float.PositiveInfinity) // remove the initial node's edge.
                .Select(v => v.Item2)
                .ToHashSet()
                .ToList(); // listify the result edges, after dropping duplicates
        }
    }

    internal class PrimNodeComparer : IComparer<Node>
    {
        internal Dictionary<Node, (float, Edge)> NodeMap;

        public PrimNodeComparer(Dictionary<Node, (float, Edge)> map)
        {
            this.NodeMap = map;
        }

        public int Compare(Node o1, Node o2)
        {
            var o1f = this.NodeMap[o1].Item1;
            var o2f = this.NodeMap[o2].Item1;

            return o1f.CompareTo(o2f);
        }
    }
}
