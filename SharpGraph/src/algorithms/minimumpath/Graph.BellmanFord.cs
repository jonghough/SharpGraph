// <copyright file="Graph.BellmanFord.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SharpGraph
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Exception class for Negative cycles found while performing Bellman-Ford algorithm.
    /// The existence of negative cycles means Bellman-Ford must fail, and throw this exception.
    /// </summary>
    public class NegativeCycleException : Exception
    {
        public NegativeCycleException(string message)
            : base(message) { }
    }

    public partial class Graph
    {
        /// <summary>
        /// Finds the minimum path between <i>nodeS</i> and <i>nodeF</i> in the graph, using the
        /// <b>Bellman-Ford Algorithm</b>. In this algorithm, node weights are allowed to take negative values.
        ///
        /// </summary>
        /// <param name="nodeS">start node.</param>
        /// <param name="nodeF">finish node.</param>
        /// <returns>A list of nodes representing the shortest path from the start node to the finish node.</returns>
        public List<Node> FindMinPathBF(Node nodeS, Node nodeF)
        {
            List<Node> path = new List<Node>();
            List<Node> nodes = new List<Node>(this.GetNodes());

            // set the initial conditions. The start node (start) has a temp
            // distance of 0
            // and all other nodes have a temp distance of max possible.
            Dictionary<Node, RouteMemory> routeMemoryMap = new Dictionary<Node, RouteMemory>();
            foreach (Node t in this.nodes)
            {
                var rm = new RouteMemory();
                routeMemoryMap[t] = rm;
                rm.Distance = t.Equals(nodeS) ? 0 : float.MaxValue;
                rm.Previous = t;
                rm.Visited = false;
            }

            for (int i = 1; i < nodes.Count; i++)
            {
                foreach (Edge edge in this.GetEdges())
                {
                    float tmp =
                        routeMemoryMap[edge.From()].Distance
                        + this.GetComponent<EdgeWeight>(edge).Weight;
                    if (tmp < routeMemoryMap[edge.To()].Distance)
                    {
                        routeMemoryMap[edge.To()].Distance = tmp;
                        routeMemoryMap[edge.To()].Previous = edge.From();
                    }
                }
            }

            // check there are no negative-weight cycles.
            foreach (Edge edge in this.GetEdges())
            {
                float tmp =
                    routeMemoryMap[edge.From()].Distance
                    + this.GetComponent<EdgeWeight>(edge).Weight;
                if (tmp < routeMemoryMap[edge.To()].Distance)
                {
                    throw new NegativeCycleException(
                        "Negative cycle found. Bellman-Ford algorithm Failure."
                    );
                }
            }

            Node p = routeMemoryMap[nodeF].Previous.GetValueOrDefault();
            path.Add(nodeF);
            while (true)
            {
                path.Add(p);
                if (p.Equals(nodeS))
                {
                    break;
                }

                p = routeMemoryMap[p].Previous.GetValueOrDefault();
            }

            path.Reverse();
            return path;
        }
    }
}
