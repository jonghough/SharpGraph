using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpGraph {
    public partial class Graph {
 

        public void DFS (OnNextNode nextNodeFunction, Node start) {
            //find the initial node, and set all nodes to unvisited.
            Node currentNode = start;
            Node previousNode = start;  
            var nodeSearchMemDict = new Dictionary<Node, NodeSearchMemory>();
            
            foreach (Node node in _nodes) {
                
                nodeSearchMemDict[node] = new NodeSearchMemory();
            }
 
            //DFS
            Stack<Node> nodeStack = new Stack<Node> ();
            nodeStack.Push (currentNode);
            while (nodeStack.Count > 0) {
                Node current = nodeStack.Pop ();

                if (! nodeSearchMemDict[current].Visited) {
                    //Call delegate fucntion here. 
                    nextNodeFunction (this, previousNode, current);

                    nodeSearchMemDict[current].Visited = true;
                    List<Node> adjNodes = GetAdjacentUnvisited (current, nodeSearchMemDict);
                    foreach (Node m in adjNodes) {
                        nodeStack.Push (m);
                    }
                }
                previousNode = current;
            }
        }

        public void DFS (OnNextNode nextNodeFunction) {
            //find the initial node, and set all nodes to unvisited.
            Node currentNode = new Node ();
            Node previousNode = new Node ();
            bool currentSelected = false;

            var nodeSearchMemDict = new Dictionary<Node, NodeSearchMemory>();
            //find an initial node, and set all nodes to not visited.
            foreach (Node node in _nodes) {
                if (!currentSelected) {
                    currentSelected = true;
                    currentNode = node;
                    previousNode = node;
                }
                nodeSearchMemDict[node] = new NodeSearchMemory();
            }

            if (!currentSelected) {
                return;
            }
            //DFS
            Stack<Node> nodeStack = new Stack<Node> ();
            nodeStack.Push (currentNode);
            while (nodeStack.Count > 0) {
                Node current = nodeStack.Pop ();

                if (! nodeSearchMemDict[current].Visited) {
                    //Call delegate fucntion here. 
                    nextNodeFunction (this, previousNode, current);

                    nodeSearchMemDict[current].Visited = true;
                    List<Node> adjNodes = GetAdjacentUnvisited (current, nodeSearchMemDict);
                    foreach (Node m in adjNodes) {
                        nodeStack.Push (m);
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
        public void DFSEdge ( OnNextEdge nextEdgeFunction ) {
            //find the initial node, and set all nodes to unvisited.
            Edge currentEdge = default(Edge);
            Edge previousEdge = default(Edge);
            bool currentSelected = false;
            var edgeSearchMemDict = new Dictionary<Edge, EdgeSearchMemory>();
            //find an initial node, and set all nodes to not visited.
            foreach (Edge edge in  GetEdges ()) {
                if (!currentSelected) {
                    currentSelected = true;
                    currentEdge = edge;
                    previousEdge = edge;
                }
                edgeSearchMemDict[edge] = new EdgeSearchMemory();
            }

            //DFS
            Stack<Edge> edgeStack = new Stack<Edge> ();
            edgeStack.Push (currentEdge);
            while (edgeStack.Count > 0) {
                Edge current = edgeStack.Pop ();

                if (! edgeSearchMemDict[current].Visited) {
                    //Call delegate function here. 
                    nextEdgeFunction (this, previousEdge, current);

                     edgeSearchMemDict[current].Visited=true;
                    List<Edge> incidentEdges = GetIncidentEdgesUnvisited (current, edgeSearchMemDict);
                    foreach (Edge e in incidentEdges) {
                        edgeStack.Push (e);
                    }
                }
                previousEdge = current; 
            }
        }
    }
}