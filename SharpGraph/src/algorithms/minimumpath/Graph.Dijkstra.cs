// <copyright file="Graph.Dijkstra.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SharpGraph
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class Graph
    {
        /// <summary>
        /// Finds the minimum path between <i>start</i> and <i>finsh</i> nodes in the graph,
        /// using <i>Dijkstra's Algorithm</i>. Node weights are expected ot be non-negative.
        /// </summary>
        /// <param name="start">start node.</param>
        /// <param name="finish">finish node.</param>
        /// <returns>>A list of nodes representing the shortest path from the start node to the finish node.</returns>
        public List<Node> FindMinPath(Node start, Node finish, bool isDirected = false)
        {
            return this.FindMinPathWithRouteDistances(start, finish).Item1;
        }

        public Tuple<List<Node>, Dictionary<Node, RouteMemory>> FindMinPathWithDistances(
            Node start,
            Node finish,
            bool isDirected = false
        )
        {
            return this.FindMinPathWithRouteDistances(start, finish);
        }

        private Tuple<List<Node>, Dictionary<Node, RouteMemory>> FindMinPathWithRouteDistances(
            Node start,
            Node finish,
            bool isDirected = false
        )
        {
            List<Node> minPath = new List<Node>();

            List<Node> nodes = new List<Node>(this.GetNodes());

            // set the initial conditions. The start node (start) has a temp
            // distance of 0
            // and all other nodes have a temp distance of max possible.
            Dictionary<Node, RouteMemory> routeMemoryMap = new Dictionary<Node, RouteMemory>();
            foreach (Node node in nodes)
            {
                var rm = new RouteMemory();
                routeMemoryMap[node] = rm;
                rm.Distance = node.Equals(start) ? 0 : float.PositiveInfinity;
                rm.Visited = false;
            }

            // loop until complete.
            while (true)
            {
                Node min = default(Node);
                bool foundNext = false;

                // try to find an unvisited node.
                foreach (Node t in nodes)
                {
                    var mem = routeMemoryMap[t];
                    if (mem.Visited == false)
                    {
                        min = t;
                        foundNext = true;
                        break;
                    }
                }

                if (!foundNext)
                {
                    break;
                }

                // find the minimum node of all unvisited nodes.
                foreach (Node t in nodes)
                {
                    var rm = routeMemoryMap[t];
                    if (rm.Visited == false && rm.Distance <= routeMemoryMap[min].Distance)
                    {
                        min = t;
                    }
                }

                routeMemoryMap[min].Visited = true;

                // update the distance of adjacent nodes if necessary.
                Dictionary<Node, float> adjacent = this.GetAdjacentNodesWithWeights(
                    min,
                    isDirected
                );
                foreach (KeyValuePair<Node, float> kvp in adjacent)
                {
                    var mem = routeMemoryMap[kvp.Key];
                    if (mem.Visited)
                    {
                        continue;
                    }

                    var minDist = routeMemoryMap[min].Distance;
                    float poss = kvp.Value + minDist; // .Distance;

                    if (poss < mem.Distance)
                    {
                        mem.Distance = poss;
                        mem.Previous = min;
                        if (kvp.Key.Equals(finish))
                        {
                            break;
                        }
                    }
                }
            }

            // build the path list.
            Node? pp = routeMemoryMap[finish].Previous;
            if (pp.HasValue)
            {
                Node p = pp.Value;
                Node c = finish; // current
                minPath.Add(c);
                while (true)
                {
                    minPath.Add(p);
                    c = p;
                    Node? pn = routeMemoryMap[p].Previous;
                    if (pn == null)
                    {
                        break;
                    }
                    else
                    {
                        if (pn.HasValue)
                        {
                            p = pn.Value;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                minPath = null;
            }

            return new(minPath, routeMemoryMap);
        }
    }
}
