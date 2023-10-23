// <copyright file="Graph.Reciprocity.cs" company="Jonathan Hough">
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
        /// Returns the <i>reciprocity</i> of each node of the graph, where reciprocity is defined as the
        /// ratio of the bidirectional edges to the total edges to and from the node.
        /// </summary>
        /// <returns>reciprocity of each node in the graph, as a dictionary of node, float pairs.</returns>
        public Dictionary<Node, float> Reciprocity()
        {
            var nodeDict = this.nodes.ToDictionary(x => x, x => (0, 1));

            foreach (var edge in this.edges)
            {
                var direction = this.GetComponent<EdgeDirection>(edge);
                var fromPair = nodeDict[edge.From()];
                var toPair = nodeDict[edge.To()];

                if (direction.Direction == Direction.Both)
                {
                    fromPair = (fromPair.Item1 + 1, fromPair.Item2 + 1);
                    toPair = (toPair.Item1 + 1, toPair.Item2 + 1);
                }
                else
                {
                    fromPair = (fromPair.Item1, fromPair.Item2 + 1);
                    toPair = (toPair.Item1, toPair.Item2 + 1);
                }

                nodeDict[edge.From()] = fromPair;
                nodeDict[edge.To()] = toPair;
            }

            return nodeDict
                .Select(kvp =>
                {
                    var key = kvp.Key;
                    var v = kvp.Value;
                    var val = v.Item1 / v.Item2;
                    return new KeyValuePair<Node, float>(key, val);
                })
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
