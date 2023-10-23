// <copyright file="Graph.BreadthFirstSearch.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System.Collections.Generic;

namespace SharpGraph
{
    public partial class Graph
    {
        /// <summary>
        /// Delegate for Graph node searches (breadth first, and depth first). This delegate will be called
        /// when each node is visited. The graph object, the previous node, and the current node, which is the
        /// node being visited, are the arguments.
        /// </summary>
        /// <param name="graph">Graph object on which the search is being performed.</param>
        /// <param name="previous">Previously visited node.</param>
        /// <param name="current">Current visited node.</param>
        public delegate void OnNextNode(Graph graph, Node previous, Node current);

        /// <summary>
        /// Delegate for Graph edge searches (breadth first, and depth first). This delegate will be called
        /// when each node is visited. The graph object, the previous edge, and the current edge, which is the
        /// edge being visited, are the arguments.
        /// </summary>
        /// <param name="graph">Graph object on which the search is being performed.</param>
        /// <param name="previous">Previously visited edge.</param>
        /// <param name="current">Current visited edge.</param>
        public delegate void OnNextEdge(Graph graph, Edge previous, Edge current);

        /// <summary>
        /// Breadth First Search of the graph. The algorithm will assume the graph is connected. The start node is
        /// assigned randomly and on each node traversal the callback function <i>onNextNode</i> id called.
        /// </summary>
        /// <param name="nextNodeFunction">Callback to perofmr task when a node is visited.</param>
        public void BFS(OnNextNode nextNodeFunction)
        {
            var current = default(Node);
            var currentSelected = false;
            var nodeMemoryMap = new Dictionary<Node, NodeSearchMemory>();

            // find an initial node, and set all nodes to not visited.
            foreach (var node in this.GetNodes())
            {
                if (!currentSelected)
                {
                    currentSelected = true;
                    current = node;
                }

                nodeMemoryMap[node] = new NodeSearchMemory();
            }

            var nodeQueue = new Queue<Node>();
            nodeQueue.Enqueue(current);

            // apply next node function to the initial node.
            nextNodeFunction(this, current, current);
            nodeMemoryMap[current].Visited = true;

            while (nodeQueue.Count > 0)
            {
                var n = nodeQueue.Dequeue();

                // get all unvisited adjacent nodes.
                var adjNodes = this.GetAdjacentUnvisited(n, nodeMemoryMap);
                foreach (var m in adjNodes)
                {
                    if (nodeMemoryMap[m].Visited)
                    {
                        continue;
                    }
                    else
                    {
                        nextNodeFunction(this, n, m);
                        nodeMemoryMap[m].Visited = true;
                        nodeQueue.Enqueue(m);
                    }
                }
            }
        }

        public void BFSEdge(OnNextEdge nextEdgeFunction)
        {
            Edge? current = null;
            var currentSelected = false;

            var edgeMemoryMap = new Dictionary<Edge, EdgeSearchMemory>();

            // find an initial node, and set all nodes to not visited.
            foreach (var edge in this.GetEdges())
            {
                if (!currentSelected)
                {
                    currentSelected = true;
                    current = edge;
                }

                edgeMemoryMap[edge] = new EdgeSearchMemory();
            }

            var edgeQueue = new Queue<Edge>();
            edgeQueue.Enqueue(current.GetValueOrDefault());
            nextEdgeFunction(this, current.GetValueOrDefault(), current.GetValueOrDefault());
            edgeMemoryMap[current.GetValueOrDefault()].Visited = true;

            while (edgeQueue.Count > 0)
            {
                var e = edgeQueue.Dequeue();

                // get all unvisited adjacent nodes.
                var incEdges = this.GetIncidentEdges(e);

                foreach (var next in incEdges)
                {
                    if (edgeMemoryMap[next].Visited)
                    {
                        continue;
                    }
                    else
                    {
                        nextEdgeFunction(this, e, next);
                        edgeMemoryMap[e].Visited = true;
                        edgeQueue.Enqueue(next);
                    }
                }
            }
        }
    }

    internal class NodeSearchMemory
    {
        public NodeSearchMemory() { }

        public bool Visited { get; set; }

        public Node? Previous { get; set; }
    }

    internal class EdgeSearchMemory
    {
        public EdgeSearchMemory() { }

        public bool Visited { get; set; }

        public Node? Previous { get; set; }
    }
}
