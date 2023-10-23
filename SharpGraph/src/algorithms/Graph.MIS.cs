// <copyright file="Graph.MIS.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public partial class Graph
    {
        /// <summary>
        /// Returns the <i>Maximally Independent Sets of the graph</i>, which is a
        /// set such that given a vertex v in the graph, either v is in the set, or
        /// a neighbour of v is in the set.
        /// </summary>
        /// <returns>A list of node sets, where each set is a maximally independent set of the graph.</returns>
        public List<HashSet<Node>> GetMaximallyIndependentSets()
        {
            var remaining = this.nodes;
            var res = new List<HashSet<Node>>();
            while (remaining.Count > 0)
            {
                res.Add(this.GetMaximallyIndependentSet(remaining));
            }

            return res;
        }

        private HashSet<Node> GetMaximallyIndependentSet(HashSet<Node> remaining)
        {
            var nodes = this.nodes.ToHashSet();
            var mis = new HashSet<Node>();

            foreach (var n in remaining)
            {
                if (nodes.Contains(n))
                {
                    mis.Add(n);
                    nodes.Remove(n);
                    foreach (var m in this.GetAdjacent(n))
                    {
                        nodes.Remove(m);
                    }
                }
            }

            remaining.RemoveWhere(i => mis.Contains(i));
            return mis;
        }
    }
}
