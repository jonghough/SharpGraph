// <copyright file="Graph.Dijkstra.cs" company="Jonathan Hough">
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
            var minPath = new List<Node>();

            var nodes = new List<Node>(this.GetNodes());

            // set the initial conditions. The start node (start) has a temp
            // distance of 0
            // and all other nodes have a temp distance of max possible.
            var routeMemoryMap = new Dictionary<Node, RouteMemory>();
            foreach (var node in nodes)
            {
                var rm = new RouteMemory();
                routeMemoryMap[node] = rm;
                rm.Distance = node.Equals(start) ? 0 : float.PositiveInfinity;
                rm.Visited = false;
            }

            // loop until complete.
            while (true)
            {
                var min = default(Node);
                var foundNext = false;

                // try to find an unvisited node.
                foreach (var t in nodes)
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
                foreach (var t in nodes)
                {
                    var rm = routeMemoryMap[t];
                    if (rm.Visited == false && rm.Distance <= routeMemoryMap[min].Distance)
                    {
                        min = t;
                    }
                }

                routeMemoryMap[min].Visited = true;

                // update the distance of adjacent nodes if necessary.
                var adjacent = this.GetAdjacentNodesWithWeights(min, isDirected);
                foreach (var kvp in adjacent)
                {
                    var mem = routeMemoryMap[kvp.Key];
                    if (mem.Visited)
                    {
                        continue;
                    }

                    var minDist = routeMemoryMap[min].Distance;
                    var poss = kvp.Value + minDist; // .Distance;

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
            var pp = routeMemoryMap[finish].Previous;
            if (pp.HasValue)
            {
                var p = pp.Value;
                var c = finish; // current
                minPath.Add(c);
                while (true)
                {
                    minPath.Add(p);
                    c = p;
                    var pn = routeMemoryMap[p].Previous;
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
