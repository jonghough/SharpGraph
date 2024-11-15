// <copyright file="Graph.Contraction.cs" company="Jonathan Hough">
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
        /// Contracts the edge from the graph. This essentially removes the edge form the graph.
        /// Incident edges will be reassigned to connect to the "keep"  node, which is one of the
        /// two nodes of the to-be-deleted edge.
        /// Edges that previously connected to the to-be-deleted node, will be deleted, and new edges to
        /// the to-be-kept node will be created.
        /// For edges not incident with the edge that is to be deleted, the edges in the returned graph will
        /// be copies, including any components.
        /// </summary>
        /// <param name="edge">Edge to be deleted.</param>
        /// <param name="keep">Node of deleted edge to keep in output graph.</param>
        /// <returns>Graph with edge removed.</returns>
        public Graph ContractEdge(Edge edge, Node keep)
        {
            if (this.GetEdges().Contains(edge) == false)
            {
                throw new Exception("Edge does not exist on this graph.");
            }

            if (!edge.Nodes().Contains(keep))
            {
                throw new Exception(
                    "Node to keep does not exist in the edge ot remove. Cannot remove edge."
                );
            }

            var removeNode = keep == edge.To() ? edge.From() : edge.To();
            var incident = this.GetIncidentEdges(removeNode);
            var toRemove = new HashSet<Edge>();
            var newEdges = new HashSet<Edge>();
            toRemove.Add(edge);
            foreach (var e in incident)
            {
                if (toRemove.Contains(e))
                {
                    continue;
                }

                if (!e.Nodes().Contains(keep))
                {
                    if (removeNode == e.To())
                    {
                        toRemove.Add(e);
                        var rex = new Edge(keep, e.From());
                        newEdges.Add(rex);
                    }
                    else if (removeNode == e.From())
                    {
                        toRemove.Add(e);
                        var rex = new Edge(keep, e.To());
                        newEdges.Add(rex);
                    }
                }
            }

            var g = this.Copy();
            foreach (var deadEdge in toRemove)
            {
                g.RemoveEdge(deadEdge);
            }

            g.RemoveNode(removeNode);
            foreach (var newEdge in newEdges)
            {
                g.AddEdge(newEdge);
            }

            return g;
        }
    }
}
