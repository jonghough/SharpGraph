// <copyright file="Graph.DepthFirstSearch.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System.Collections.Generic;

namespace SharpGraph
{
    public partial class Graph
    {
        public void DFS(OnNextNode nextNodeFunction, Node start)
        {
            // find the initial node, and set all nodes to unvisited.
            var currentNode = start;
            var previousNode = start;
            var nodeSearchMemDict = new Dictionary<Node, NodeSearchMemory>();

            foreach (var node in this.nodes)
            {
                nodeSearchMemDict[node] = new NodeSearchMemory();
            }

            // DFS
            var nodeStack = new Stack<Node>();
            nodeStack.Push(currentNode);
            while (nodeStack.Count > 0)
            {
                var current = nodeStack.Pop();

                if (!nodeSearchMemDict[current].Visited)
                {
                    // Call delegate fucntion here.
                    nextNodeFunction(this, previousNode, current);

                    nodeSearchMemDict[current].Visited = true;
                    var adjNodes = this.GetAdjacentUnvisited(current, nodeSearchMemDict);
                    foreach (var m in adjNodes)
                    {
                        nodeStack.Push(m);
                    }
                }

                previousNode = current;
            }
        }

        public void DFS(OnNextNode nextNodeFunction)
        {
            // find the initial node, and set all nodes to unvisited.
            var currentNode = default(Node);
            var previousNode = default(Node);
            var currentSelected = false;

            var nodeSearchMemDict = new Dictionary<Node, NodeSearchMemory>();

            // find an initial node, and set all nodes to not visited.
            foreach (var node in this.nodes)
            {
                if (!currentSelected)
                {
                    currentSelected = true;
                    currentNode = node;
                    previousNode = node;
                }

                nodeSearchMemDict[node] = new NodeSearchMemory();
            }

            if (!currentSelected)
            {
                return;
            }

            // DFS
            var nodeStack = new Stack<Node>();
            nodeStack.Push(currentNode);
            while (nodeStack.Count > 0)
            {
                var current = nodeStack.Pop();

                if (!nodeSearchMemDict[current].Visited)
                {
                    // Call delegate fucntion here.
                    nextNodeFunction(this, previousNode, current);

                    nodeSearchMemDict[current].Visited = true;
                    var adjNodes = this.GetAdjacentUnvisited(current, nodeSearchMemDict);
                    foreach (var m in adjNodes)
                    {
                        nodeStack.Push(m);
                    }
                }

                previousNode = current;
            }
        }

        /// <summary>
        /// Performs a preorder depth-first search of the graph edges, beginning at an arbitray node.
        ///
        /// </summary>
        /// <param name="nextEdgeFunction">The function called on each edge.</param>
        public void DFSEdge(OnNextEdge nextEdgeFunction)
        {
            // find the initial node, and set all nodes to unvisited.
            var currentEdge = default(Edge);
            var previousEdge = default(Edge);
            var currentSelected = false;
            var edgeSearchMemDict = new Dictionary<Edge, EdgeSearchMemory>();

            // find an initial node, and set all nodes to not visited.
            foreach (var edge in this.GetEdges())
            {
                if (!currentSelected)
                {
                    currentSelected = true;
                    currentEdge = edge;
                    previousEdge = edge;
                }

                edgeSearchMemDict[edge] = new EdgeSearchMemory();
            }

            // DFS
            var edgeStack = new Stack<Edge>();
            edgeStack.Push(currentEdge);
            while (edgeStack.Count > 0)
            {
                var current = edgeStack.Pop();

                if (!edgeSearchMemDict[current].Visited)
                {
                    // Call delegate function here.
                    nextEdgeFunction(this, previousEdge, current);

                    edgeSearchMemDict[current].Visited = true;
                    var incidentEdges = this.GetIncidentEdgesUnvisited(current, edgeSearchMemDict);
                    foreach (var e in incidentEdges)
                    {
                        edgeStack.Push(e);
                    }
                }

                previousEdge = current;
            }
        }
    }
}
