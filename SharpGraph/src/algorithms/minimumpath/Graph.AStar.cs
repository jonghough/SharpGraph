// <copyright file="Graph.AStar.cs" company="Jonathan Hough">
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
        /// Generates a list of nodes, beginning at >i>nodeS.</i> and finishing at <i>nodeF.</i>,
        /// giving the shortest path in the graph from <i>nodeS.</i> to <i>nodeF.</i>, using the <b>A-star.</b> algorithm.
        /// The method requires the caller provides a heuristic that is used by the algorithm to
        /// give estimates of the distance between nodes.
        ///
        /// <returns></returns></summary>
        /// <param name="nodeS">start node</param>
        /// <param name="nodeF">finish node</param>
        /// <param name="heuristic">a heuristic to estimate distances between pairs of nodes.</param>
        /// <returns>A list of nodes indicating the shortest path from start to finish.</returns>
        public List<Node> FindMinPath(Node nodeS, Node nodeF, IAStarHeuristic heuristic)
        {
            return this.FindMinPath(new List<Edge>(), nodeS, nodeF, heuristic);
        }

        public List<Node> FindMinPath(
            List<Edge> forbiddenEdges,
            Node nodeS,
            Node nodeF,
            IAStarHeuristic heuristic
        )
        {
            var openSet = new HashSet<Node>();
            var closedSet = new HashSet<Node>();
            var openPQ = new C5.IntervalHeap<Node>(new NodeComparer(heuristic, nodeF));
            openSet.Add(nodeS);
            openPQ.Add(nodeS);
            var allowedEdges = new List<Edge>(this.GetEdges());
            Predicate<Edge> match = e => forbiddenEdges.Contains(e);
            allowedEdges.RemoveAll(match);

            var shortestPath = new List<Node>();

            var gScoreMap = new Dictionary<Node, float>();
            var fScoreMap = new Dictionary<Node, float>();
            var routeMemoryMap = new Dictionary<Node, RouteMemory>();
            foreach (var t in this.nodes)
            {
                routeMemoryMap[t] = new RouteMemory();

                gScoreMap.Add(t, float.PositiveInfinity);
                fScoreMap.Add(t, float.PositiveInfinity);
                routeMemoryMap[t].Previous = t;
            }

            gScoreMap[nodeS] = 0;
            routeMemoryMap[nodeS].Previous = nodeS;
            fScoreMap[nodeS] = 0f + heuristic.GetHeuristic(nodeS, nodeF); // (f = g + heuristic)
            while (openPQ.Count > 0)
            {
                var current = openPQ.DeleteMin();
                if (current.Equals(nodeF))
                {
                    shortestPath.Add(current);
                    break;
                }

                openSet.Remove(current);
                closedSet.Add(current);
                var incident = this.GetIncidentEdges(current, match);

                foreach (var u in incident)
                {
                    foreach (var t in u.Nodes())
                    {
                        if (t.Equals(current))
                        {
                            continue;
                        }
                        else
                        {
                            var tmp = gScoreMap[current] + this.GetComponent<EdgeWeight>(u).Weight;
                            var currentScore = gScoreMap[t];
                            if (currentScore > tmp)
                            {
                                routeMemoryMap[t].Previous = current;

                                gScoreMap[t] = tmp;
                                fScoreMap[t] = gScoreMap[t] + heuristic.GetHeuristic(t, nodeF);
                                routeMemoryMap[t].Distance = tmp;
                                if (openSet.Contains(t) == false)
                                {
                                    openSet.Add(t);
                                    openPQ.Add(t);
                                }
                            }
                        }
                    }
                }
            }

            routeMemoryMap[nodeF].Distance = gScoreMap[nodeF];
            var p = routeMemoryMap[nodeF].Previous.GetValueOrDefault();
            while (true)
            {
                shortestPath.Add(p);
                routeMemoryMap[p].Distance = gScoreMap[p];
                if (p == nodeF || p == nodeS)
                {
                    break;
                }

                p = routeMemoryMap[p].Previous.GetValueOrDefault();
            }

            shortestPath.Reverse();
            return shortestPath;
        }
    }

    internal class NodeComparer : IComparer<Node>
    {
        public IAStarHeuristic Func;
        private readonly Node src;

        public NodeComparer(IAStarHeuristic h, Node s)
        {
            this.Func = h;
            this.src = s;
        }

        public int Compare(Node o1, Node o2)
        {
            if (this.Func.GetHeuristic(this.src, o1) < this.Func.GetHeuristic(this.src, o2))
            {
                return 1;
            }
            else if (this.Func.GetHeuristic(this.src, o1) > this.Func.GetHeuristic(this.src, o2))
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
