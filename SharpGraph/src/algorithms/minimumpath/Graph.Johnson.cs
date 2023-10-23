// <copyright file="Graph.Johnson.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public partial class Graph
    {
        /// <summary>
        /// Finds the shortest paths between all pairs of nodes, using <i>Johnson's algorithm</i>.
        /// The graph is assumed to be both weighted and directed. If any edge has no. <code>EdgeWeight</code>
        /// component, or. <code>EdgeDirection</code> component then an exception will be thrown. Negative edge weights are allowed.
        /// The method will return a list of tuples. Each tuple represents the shortest path length from a node to another node.
        /// Tuples are of the type. <code>Node,Node,float</code>.
        /// <see href="https://en.wikipedia.org/wiki/Johnson%27s_algorithm">Johnson's algorithm.</see>.
        /// </summary>
        /// <returns>List of tuples, where esch tuple represents shortest path length between two nodes.</returns>
        public List<Tuple<Node, Node, float>> FindShortestPaths()
        {
            var weightlessEdges = this.edges
                .Where(e => this.GetComponent<EdgeWeight>(e) == null)
                .Count();
            var directionlessEdges = this.edges
                .Where(e => this.GetComponent<EdgeDirection>(e) == null)
                .Count();

            if (weightlessEdges > 0)
            {
                throw new Exception(
                    string.Format(
                        "All edges must contain an EdgeWeight component. {0} edges found without this component.",
                        weightlessEdges
                    )
                );
            }

            if (directionlessEdges > 0)
            {
                throw new Exception(
                    string.Format(
                        "All edges must contain an EdgeDirection component. {0} edges found without this component.",
                        directionlessEdges
                    )
                );
            }

            var g = this.Copy();
            var q = new Node(Guid.NewGuid().ToString());
            g.nodes
                .ToList()
                .ForEach(n =>
                {
                    var edge = new Edge(q, n);
                    g.AddEdge(edge);
                    var ew = g.AddComponent<EdgeWeight>(edge);
                    ew.Weight = 0;
                    var dir = g.AddComponent<EdgeDirection>(edge);
                    dir.Direction = Direction.Forwards;
                });

            var minPathDict = new Dictionary<Node, List<Node>>();
            var minPathDist = new Dictionary<Node, float>();
            g.nodes
                .ToList()
                .ForEach(n =>
                {
                    minPathDict[n] = g.FindMinPathBF(q, n);
                });

            foreach (var kvp in minPathDict)
            {
                var nodes = kvp.Value;
                float dist = 0;
                for (var i = 0; i < nodes.Count - 1; i++)
                {
                    if (nodes[i + 1] == q)
                    {
                        continue;
                    }

                    var edge = g.GetEdge(nodes[i], nodes[i + 1]).Value;
                    dist += g.GetComponent<EdgeWeight>(edge).Weight;
                }

                minPathDist[kvp.Key] = dist;
            }

            foreach (var edge in g.edges)
            {
                var dir = g.GetComponent<EdgeDirection>(edge);
                var weight = g.GetComponent<EdgeWeight>(edge);

                var diff = minPathDist[edge.From()] - minPathDist[edge.To()];
                diff = dir.Direction == Direction.Backwards ? -diff : diff;

                weight.Weight += diff;
            }

            g.RemoveNode(q);

            // dijkstras on all pairs
            var result = new List<Tuple<Node, Node, float>>();
            g.nodes.Remove(q);
            var nl = g.nodes.ToList();

            for (var i = 0; i < nl.Count - 1; i++)
            {
                var minPath = g.FindMinPathWithDistances(nl[i], nl[i + 1]);

                foreach (var kvp in minPath.Item2)
                {
                    var nodes = kvp.Value;
                    var path = new Tuple<Node, Node, float>(nl[i], kvp.Key, kvp.Value.Distance);
                    result.Add(path);
                }
            }

            return result;
        }
    }
}
