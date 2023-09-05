// <copyright file="Graph.FordFulkerson.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SharpGraph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Graph
    {
        /// <summary>
        /// Finds the Max Flow from the <i>startNode</i> to the <i>finishNode</i>.
        /// For each node in the Flow Network, the <i>CapacityEdge</i> has a value set to give the
        /// capacity per node. If the method succeeds <b>true</b> is returned, otherwise <b>false</b>
        /// is returned.
        ///
        /// <example>
        /// <code>
        /// var didComplete = g.FindMaxFlow(startNode, finishNode);
        ///
        /// if(didComplete){
        ///
        ///     var flow = g.GetComponent&lt;EdgeCapacity&gt;(middleNode).Flow;
        ///
        /// }
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="finishNode"></param>
        /// <returns>true if succeeds, false otherwise.</returns>
        public bool FindMaxFlow(Node startNode, Node finishNode)
        {
            // find some initial path.
            var flowMemoryMap = new Dictionary<Node, FlowMemory>();
            foreach (Node n in this.GetNodes())
            {
                // NodeData nodeData = AddComponent<NodeData> (n);
                var fm = new FlowMemory();
                fm.Visited = n == startNode ? true : false;
                flowMemoryMap[n] = fm;
            }

            foreach (Edge e in this.GetEdges())
            {
                var capacity = this.AddComponent<EdgeCapacity>(e);
                capacity.FlowDirection = Direction.Forwards;
            }

            List<Edge> allEdges = new List<Edge>(this.GetEdges());
            List<Edge> initialPath = this.FindPath(allEdges, startNode, finishNode, flowMemoryMap);

            while (initialPath != null)
            {
                Node last = startNode;
                Func<Edge, float> selector = e =>
                {
                    return this.GetComponent<EdgeCapacity>(e).GetResidualFlow();
                };
                var minFlow = initialPath.Select(selector).Min();
                List<Edge> minNode = initialPath
                    .FindAll(e => this.GetComponent<EdgeCapacity>(e).GetResidualFlow() == minFlow)
                    .ToList();

                foreach (Edge e in initialPath)
                {
                    if (this.GetComponent<EdgeCapacity>(e).FlowDirection == Direction.Forwards)
                    {
                        this.GetComponent<EdgeCapacity>(e).Flow += minFlow;
                    }
                    else
                    {
                        this.GetComponent<EdgeCapacity>(e).Flow -= minFlow;
                    }
                }

                foreach (Node n in this.GetNodes())
                {
                    flowMemoryMap[n].Visited = false;
                }

                foreach (Edge e in this.GetEdges())
                {
                    this.GetComponent<EdgeCapacity>(e).FlowDirection = Direction.Forwards;
                }

                flowMemoryMap[startNode].Visited = true;

                // GetComponent<NodeData> (startNode).Visited = true;
                // augment the path.
                initialPath.Clear();
                allEdges.Clear();
                allEdges = new List<Edge>(this.GetEdges());
                allEdges.Remove(minNode[0]);
                initialPath = this.FindPath(allEdges, startNode, finishNode, flowMemoryMap);
            }

            return false;
        }

        private List<Edge> FindPath(
            List<Edge> edges,
            Node start,
            Node finish,
            Dictionary<Node, FlowMemory> flowMemoryMap
        )
        {
            // filter only the edges coming from the start node
            Predicate<Edge> match = e => !edges.Contains(e);
            List<Edge> edgesList = this.GetIncidentEdges(start, match)
                .Where(e =>
                {
                    if (e.From() == start && !flowMemoryMap[e.To()].Visited)
                    {
                        return true;
                    }
                    else if (e.To() == start && !flowMemoryMap[e.From()].Visited)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                })
                .ToList();

            // For each outgoing edge, do a depth first search for any path terminating at the
            // finish node. If no such path can be found, then there is no route from start to finish.
            foreach (Edge selectedEdge in edgesList)
            {
                if (selectedEdge != null)
                {
                    // only want edges with positive residual flow.
                    if (this.GetComponent<EdgeCapacity>(selectedEdge).GetResidualFlow() <= 0)
                    {
                        continue;
                    }

                    flowMemoryMap[selectedEdge.To()].Visited = true;

                    // GetComponent<NodeData> (selectedEdge.To ()).Visited = true;

                    // Flow direction is backwards if going from from() to to()
                    // nodes. This changes the residual capacity calculation.
                    if (selectedEdge.To() == start)
                    {
                        this.GetComponent<EdgeCapacity>(selectedEdge).FlowDirection =
                            Direction.Backwards;
                    }

                    if (selectedEdge.To() == finish)
                    {
                        List<Edge> path = new List<Edge>();

                        path.Add(selectedEdge);

                        return path;
                    }
                    else
                    {
                        List<Edge> edgesCopy = new List<Edge>(edges);
                        edgesCopy.Remove(selectedEdge);
                        Node next = selectedEdge.To();
                        if (next == start)
                        {
                            next = selectedEdge.From();
                        }

                        List<Edge> subPath = this.FindPath(edgesCopy, next, finish, flowMemoryMap);

                        if (subPath != null && subPath.Count() != 0)
                        {
                            List<Edge> path = new List<Edge>();

                            path.AddRange(subPath);
                            path.Insert(0, selectedEdge);

                            return path;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    continue;
                }
            }

            // Return null, only if all possible paths are exhausted
            // and no path to the finish node has been found.
            return null;
        }
    }
}
