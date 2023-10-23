// <copyright file="Graph.Boundary.cs" company="Jonathan Hough">
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
        /// Returns the node boundary of the given node set. The node boundary is defined
        /// as the set of nodes whose incident edges have exactly one node in the set.
        /// i.e. the nodes that sit on the boundary of the set.
        /// </summary>
        /// <param name="nodes">Collection of nodes to find the boundary of.</param>
        /// <returns>A HashSet of Nodes, giving the boundary node set of the input parameter node set.</returns>
        public HashSet<Node> NodeBoundary(HashSet<Node> nodes)
        {
            var except = new HashSet<Node>(this.nodes.Except(nodes));
            if (except.Count == 0)
            {
                return except;
            }

            var boundaryNodes = this.edges
                .Select(i =>
                {
                    if (nodes.Contains(i.From()) && !nodes.Contains(i.To()))
                    {
                        var res = (Node?)i.To();
                        return res;
                    }

                    if (!nodes.Contains(i.From()) && nodes.Contains(i.To()))
                    {
                        var res = (Node?)i.From();
                        return res;
                    }

                    return (Node?)null;
                })
                .Where(x => x != null)
                .Select(i => i.Value)
                .ToList();
            return new HashSet<Node>(boundaryNodes);
        }

        public HashSet<Node> DirectedNodeBoundary(HashSet<Node> nodes)
        {
            var except = new HashSet<Node>(this.nodes.Except(nodes));
            if (except.Count == 0)
            {
                return except;
            }

            var boundaryNodes = this.edges
                .Select(i =>
                {
                    var direction = this.GetComponent<EdgeDirection>(i);
                    if (direction == null)
                    {
                        throw new Exception("Edge has no direction");
                    }

                    if (
                        nodes.Contains(i.From())
                        && !nodes.Contains(i.To())
                        && direction.Direction != Direction.Backwards
                    )
                    {
                        var res = (Node?)i.To();
                        return res;
                    }

                    if (
                        !nodes.Contains(i.From())
                        && nodes.Contains(i.To())
                        && direction.Direction != Direction.Forwards
                    )
                    {
                        var res = (Node?)i.From();
                        return res;
                    }

                    return (Node?)null;
                })
                .Where(x => x != null)
                .Select(i => i.Value)
                .ToList();
            return new HashSet<Node>(boundaryNodes);
        }

        /// <summary>
        /// Returns the edge boundary of the given node set. The noedgede boundary is defined
        /// as the set of edges whose incident nodes have exactly one node in the set.
        /// i.e. the edges that sit on the boundary of the set.
        /// </summary>
        /// <param name="nodes">Collection of nodes to find the boundary of.</param>
        /// <returns>A HashSet of edges, giving the boundary edge set of the input parameter node set.</returns>
        public HashSet<Edge> EdgeBoundary(HashSet<Node> nodes)
        {
            var boundaryEdges = this.edges
                .Select(i =>
                {
                    if (nodes.Contains(i.From()) && !nodes.Contains(i.To()))
                    {
                        var res = (Edge?)i;
                        return res;
                    }

                    if (!nodes.Contains(i.From()) && nodes.Contains(i.To()))
                    {
                        var res = (Edge?)i;
                        return res;
                    }

                    return (Edge?)null;
                })
                .Where(x => x != null)
                .Select(i => i.Value)
                .ToList();
            return new HashSet<Edge>(boundaryEdges);
        }
    }
}
