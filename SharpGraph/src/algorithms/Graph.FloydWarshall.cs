// <copyright file="Graph.FloydWarshall.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace SharpGraph
{
    public partial class Graph
    {
        /// <summary>
        /// Calculates pairwise distances between all nodes of the graph, using the
        /// <i>Floyd-Warshall</i> algorithm. The algorithm has a runtime complexity of.
        /// <i>O(N^3). The method returns a dictionary, where keys are pairs of nodes
        /// and values are the distances between those nodes.
        /// </summary>
        /// <returns>Dictionary of node pairs and distances</returns>
        public Dictionary<(Node, Node), float> GetDistances()
        {
            var (dist, paths) = this.CalculateDistances();
            return dist;
        }

        /// <summary>
        /// Calculates the pairwise distances between all nodes of the graph, and returns dictionary
        /// of node pairs mapped to nodes, which gives the path of each node to each other node. This method
        /// is useful if not only the pairwise distances are wanted, but also the paths (as a sequence of nodes)
        /// from source to sink are also wanted.
        /// </summary>
        /// <returns>A tuple containing a dictionary of distances, and a dictionary mapping node pairs to next nodes in
        /// the shortest path sequence.</returns>
        private (
            Dictionary<(Node, Node), float>,
            Dictionary<(Node, Node), Node>
        ) GetDistancesWithPaths()
        {
            return this.CalculateDistances();
        }

        private (
            Dictionary<(Node, Node), float>,
            Dictionary<(Node, Node), Node>
        ) CalculateDistances()
        {
            var nodeList = new List<Node>(this.nodes);
            var distances = new Dictionary<(Node, Node), float>();
            Dictionary<(Node, Node), Node> path = new Dictionary<(Node, Node), Node>();
            var n = nodeList.Count;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        if (this.IsAdjacent(nodeList[i], nodeList[j]))
                        {
                            var edge = this.GetEdge(
                                new HashSet<Node>() { nodeList[i], nodeList[j] }
                            ).Value;
                            var dist = this.GetComponent<EdgeWeight>(edge).Weight;
                            distances[(nodeList[i], nodeList[j])] = dist;
                            path[(nodeList[i], nodeList[j])] = nodeList[j];
                        }
                        else
                        {
                            var dist = float.PositiveInfinity;
                            distances[(nodeList[i], nodeList[j])] = dist;
                        }
                    }
                }
            }

            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    if (i == k)
                    {
                        continue;
                    }

                    for (int j = 0; j < n; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        if (k == j)
                        {
                            continue;
                        }

                        var dij = distances[(nodeList[i], nodeList[j])];
                        var dik = distances[(nodeList[i], nodeList[k])];
                        var dkj = distances[(nodeList[k], nodeList[j])];
                        if (dik + dkj < dij)
                        {
                            dij = dik + dkj;
                            path[(nodeList[i], nodeList[j])] = path[(nodeList[i], nodeList[k])];
                        }

                        distances[(nodeList[i], nodeList[j])] = dij;
                    }
                }
            }

            return (distances, path);
        }
    }
}
