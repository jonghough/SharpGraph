// <copyright file="Graph.Random.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public class RandomGraphException : Exception
    {
        public RandomGraphException(string msg)
            : base(msg) { }
    }

    public partial class Graph
    {
        /// <summary>
        /// Generates a random graph with. <code>nodeCount</code> nodes and. <code>edgeCount</code> edges.
        /// The edges will be randomly generated from pairs of nodes. If. <code>isDirected</code> is true then
        /// a random direction will be given to each edge.
        /// If the nodeCount is less than 1 or edgeCount is less than 0, a. <code>RandomGraphException</code> will be thrown.
        /// If the edgeCount is greater than the maximum possible number of edges assignable to a graph on nodeCount
        /// nodes, then a. <code>RandomGraphException</code> will be thrown.
        /// </summary>
        /// <param name="nodeCount">number of nodes in generated graph.</param>
        /// <param name="edgeCount">number of edges in generated graph.</param>
        /// <param name="isDirected">if true, EdgeDirection assigned ot each edge.</param>
        /// <param name="randSeed"></param>
        /// <returns>A random Graph.</returns>
        public static Graph GenerateRandom(
            int nodeCount,
            int edgeCount,
            bool isDirected,
            int? randSeed = null
        )
        {
            if (nodeCount < 1)
            {
                throw new RandomGraphException(
                    string.Format("Cannot create graph  {0} nodes. Not enough nodes.", nodeCount)
                );
            }
            else if (edgeCount < 0)
            {
                throw new RandomGraphException(
                    string.Format(
                        "Cannot create graph with {0} edges. Not enough edges.",
                        edgeCount
                    )
                );
            }
            else if (edgeCount > nodeCount * (nodeCount - 1) * 0.5f)
            {
                throw new RandomGraphException(
                    string.Format(
                        "Cannot create graph with {0} edges and {1} nodes. Too many edges.",
                        edgeCount,
                        nodeCount
                    )
                );
            }

            var r = new System.Random(
                randSeed == null ? (int)(DateTime.Now.Ticks & 0x0000FFFF) : randSeed.Value
            );
            var nodes = NodeGenerator.GenerateNodes(nodeCount).ToList();
            var edges = new HashSet<(Node, Node)>();
            while (edges.Count < edgeCount)
            {
                var n1 = r.Next(nodeCount);
                var n2 = r.Next(nodeCount);
                if (n1 == n2)
                {
                    continue;
                }

                if (
                    edges.Contains((nodes[n1], nodes[n2])) || edges.Contains((nodes[n2], nodes[n1]))
                )
                {
                    continue;
                }

                edges.Add((nodes[n1], nodes[n2]));
            }

            var edgeList = edges.Select(i => new Edge(i.Item1, i.Item2)).ToList();

            var g = new Graph(edgeList, nodes.ToHashSet());
            if (isDirected)
            {
                edgeList.ForEach(i =>
                {
                    var ed = g.AddComponent<EdgeDirection>(i);
                    ed.Direction = Direction.Forwards;
                });
            }

            return g;
        }

        /// <summary>
        /// Generates a random spanning tree on. <code>nodeCount</code> nodes. The node count must be at least 3, or a. <code>RandomGraphException</code>
        /// will be thrown. (Note that if node count is less than 2, a random spanning tree is just the complete graph
        /// on 2 nodes).
        ///
        /// </summary>
        /// <param name="nodeCount">number of nodes in the graph.</param>
        /// <param name="randSeed"></param>
        /// <returns>Random spanning tree.</returns>
        public static Graph GenerateRandomSpanningTree(int nodeCount, int? randSeed = null)
        {
            if (nodeCount < 3)
            {
                throw new RandomGraphException(
                    string.Format(
                        "Cannot create random spanning tree with {0} nodes. Not enough nodes.",
                        nodeCount
                    )
                );
            }

            var r = new System.Random(
                randSeed == null ? (int)(DateTime.Now.Ticks & 0x0000FFFF) : randSeed.Value
            );
            var nodes = NodeGenerator.GenerateNodes(nodeCount).ToList();
            var edges = new List<Edge>();
            for (var i = 1; i < nodeCount; i++)
            {
                var m = r.Next(i);
                edges.Add(new Edge(nodes[m], nodes[i]));
            }

            return new Graph(edges, nodes.ToHashSet());
        }
    }
}
