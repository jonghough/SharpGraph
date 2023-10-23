// <copyright file="Graph.SteinerTree.cs" company="Jonathan Hough">
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
        /// Generates an approximate minimal steiner tree for the given node set.
        /// That is, this method will find the minimum spanning tree
        /// containing a subset of nodes of the graph that must contain all the nodes in the given subset.
        /// The algorithm runs in O(N^2) time, and O(N-2) space, so for large graphs caution should be taken.
        ///
        /// ref: https://core.ac.uk/download/pdf/82609861.pdf.
        /// </summary>
        /// <param name="subset">A subset of the nodes of the set.</param>
        /// <returns>A tree Graph represneitng the minimum Steiner tree.</returns>
        public Graph GenerateMinimalSpanningSteinerTree(HashSet<Node> subset)
        {
            if (!this.IsConnected())
            {
                throw new NotConnectedException(
                    "Minimal Steiner Tree algorithm can only run on a connected graph."
                );
            }

            var distanceDictionary = this.GetDistances();
            var subsetList = new List<Node>(subset);
            int k = 0;
            var current = subsetList[k];

            var allSTEdges = new HashSet<Edge>();
            var tree = new Graph(new Node[] { current });
            var shortestPaths = this.GetDistancesWithPaths();
            while (k < subset.Count - 1)
            {
                var next = subsetList[k + 1];

                var distances = shortestPaths.Item1;
                var nodePaths = shortestPaths.Item2;
                var distToTree = distances
                    .Where(
                        d =>
                            d.Key.Item1 == next
                            && tree.GetNodes().Contains(d.Key.Item2)
                            && d.Key.Item2 != next
                    )
                    .MinBy(kvp => kvp.Value);

                this.ExtractEdgeList(shortestPaths.Item2, next, distToTree.Key.Item2)
                    .ForEach(e =>
                    {
                        allSTEdges.Add(e);
                        tree.AddEdge(e.From(), e.To());
                    });

                current = next;
                k++;
            }

            return tree;
        }

        private List<Edge> ExtractEdgeList(
            Dictionary<(Node, Node), Node> nodeMatrix,
            Node start,
            Node end
        )
        {
            var edgeList = new List<Edge>();
            var prev = start;
            var current = nodeMatrix[(start, end)];

            var nodeSet = new HashSet<Node>();

            while (current != end)
            {
                nodeSet.Clear();
                nodeSet.Add(prev);
                nodeSet.Add(current);
                edgeList.Add(this.GetEdge(nodeSet).Value);
                var next = nodeMatrix[(current, end)];

                prev = current;
                current = next;
            }

            nodeSet.Clear();
            nodeSet.Add(prev);
            nodeSet.Add(end);
            edgeList.Add(this.GetEdge(nodeSet).Value);
            return edgeList;
        }
    }
}
