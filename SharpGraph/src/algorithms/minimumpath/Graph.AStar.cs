// <copyright file="Graph.AStar.cs" company="PlaceholderCompany">
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
            HashSet<Node> openSet = new HashSet<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            var openPQ = new C5.IntervalHeap<Node>(new NodeComparer(heuristic, nodeS));
            openSet.Add(nodeS);
            openPQ.Add(nodeS);
            List<Edge> allowedEdges = new List<Edge>(this.GetEdges());
            Predicate<Edge> match = e => forbiddenEdges.Contains(e);
            allowedEdges.RemoveAll(match);

            List<Node> shortestPath = new List<Node>();

            Dictionary<Node, float> gScoreMap = new Dictionary<Node, float>();
            Dictionary<Node, float> fScoreMap = new Dictionary<Node, float>();
            Dictionary<Node, RouteMemory> routeMemoryMap = new Dictionary<Node, RouteMemory>();
            foreach (Node t in this.nodes)
            {
                routeMemoryMap[t] = new RouteMemory();
                if (!nodeS.Equals(t))
                {
                    gScoreMap.Add(t, float.MaxValue);
                    fScoreMap.Add(t, 0f);
                    routeMemoryMap[t].Previous = t;
                }
            }

            gScoreMap.Add(nodeS, 0f);
            routeMemoryMap[nodeS].Previous = nodeS;
            fScoreMap.Add(nodeS, 0f + heuristic.GetHeuristic(nodeS, nodeF)); // (f = g + heuristic)
            while (openPQ.Count > 0)
            {
                Node current = openPQ.DeleteMin();

                if (current.Equals(nodeF))
                {
                    shortestPath.Add(current);
                    break;
                }

                openSet.Remove(current);
                closedSet.Add(current);
                List<Edge> incident = this.GetIncidentEdges(current, match);

                foreach (Edge u in incident)
                {
                    foreach (Node t in u.Nodes())
                    {
                        if (t.Equals(current))
                        {
                            continue;
                        }
                        else if (closedSet.Contains(t))
                        {
                            continue;
                        }
                        else
                        {
                            float tmp =
                                gScoreMap[current] + this.GetComponent<EdgeWeight>(u).Weight;
                            if (openSet.Contains(t) == false)
                            {
                                routeMemoryMap[t].Previous = current;
                                openSet.Add(t);
                                openPQ.Add(t);
                                gScoreMap[t] = tmp;
                                fScoreMap[t] = gScoreMap[t] + heuristic.GetHeuristic(t, nodeF);
                                routeMemoryMap[t].Distance = tmp;
                            }
                            else
                            {
                                float currentScore = gScoreMap[t];
                                if (currentScore > tmp)
                                {
                                    routeMemoryMap[t].Previous = current;
                                    gScoreMap[t] = tmp;
                                    fScoreMap[t] = gScoreMap[t] + heuristic.GetHeuristic(t, nodeF);
                                    routeMemoryMap[t].Distance = tmp;
                                }
                            }
                        }
                    }
                }
            }

            routeMemoryMap[nodeF].Distance = gScoreMap[nodeF];
            Node p = routeMemoryMap[nodeF].Previous.GetValueOrDefault();
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
        private Node src;

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
