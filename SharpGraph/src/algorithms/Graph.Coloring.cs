// <copyright file="Graph.Coloring.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public enum NodeOrdering
    {
        Random,
        NodeOrderAscending,
        NodeOrderDescending,
    }

    public partial class Graph
    {
        /// <summary>
        /// Colors the nodes of the graph by a greedy algorithm. There are no guarantees the coloring will
        /// be optimal.
        /// The method returns a dictionary of node-color pairs, where colors are represented by <i>uint</i>s.
        ///
        /// </summary>
        /// <param name="nodeOrdering"></param>
        /// <returns>Dictionary of node, color pairs.</returns>
        public Dictionary<Node, uint> GreedyColorNodes(NodeOrdering nodeOrdering)
        {
            var processed = new HashSet<Node>();
            var dict = new Dictionary<Node, uint>();
            var nodes = this.GetOrderedNodes(nodeOrdering);
            foreach (var n in nodes)
            {
                var adjacent = this.GetAdjacent(n);
                adjacent = adjacent.Intersect(processed).ToList();

                var adjColors = adjacent.Select(a => dict[a]).ToHashSet();
                var didProcess = false;
                uint i = 0;
                for (i = 0; ; i++)
                {
                    if (!adjColors.Contains(i))
                    {
                        dict[n] = i;
                        processed.Add(n);
                        didProcess = true;
                        break;
                    }
                }

                if (!didProcess)
                {
                    processed.Add(n);
                    dict[n] = ++i;
                }
            }

            return dict;
        }

        private List<Node> GetOrderedNodes(NodeOrdering nodeOrdering)
        {
            var nodes = this.nodes.ToList();

            switch (nodeOrdering)
            {
                case NodeOrdering.Random:
                    break;
                case NodeOrdering.NodeOrderAscending:
                {
                    var order = new Dictionary<Node, int>();
                    foreach (var m in nodes)
                    {
                        var o = this.GetAdjacent(m).Count();
                        order[m] = o;
                    }

                    nodes = nodes.OrderBy(item => order[item]).ToList();
                    break;
                }

                case NodeOrdering.NodeOrderDescending:
                {
                    var order = new Dictionary<Node, int>();
                    foreach (var m in nodes)
                    {
                        var o = this.GetAdjacent(m).Count();
                        order[m] = o;
                    }

                    nodes = nodes.OrderByDescending(item => order[item]).ToList();
                    break;
                }
            }

            return nodes;
        }
    }
}
