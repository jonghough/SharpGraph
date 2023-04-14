 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{

    public partial class Graph
    {


        /// <summary>
        /// Finds biconnected components of the graph. The graph is assumed to be undirected.
        /// </summary>
        /// <returns></returns>
        public HashSet<HashSet<Edge>> FindBiconnectedComponents()
        {

            var nodeDataMap = new Dictionary<Node, BCNodeData>();
            foreach (Node node in _nodes)
            {
                var m = new BCNodeData();
                nodeDataMap[node] = m;
                m.Visited = false;
            }
            var edgeStack = new Stack<Edge>();
            var connectedSets = new HashSet<HashSet<Edge>>();
            var counter = new int[] { 0 };
            foreach (Node node in _nodes)
            {
                if (!nodeDataMap[node].Visited)
                {
                    VisitNode_BC(node, counter, edgeStack, connectedSets, nodeDataMap);
                }
            }

            return connectedSets;
        }

        private void VisitNode_BC(Node node, int[] counter, Stack<Edge> edgeStack, HashSet<HashSet<Edge>> connectedSets, Dictionary<Node, BCNodeData> nodeDataMap)
        {
            var m = nodeDataMap[node];
            m.Visited = true;
            counter[0]++;
            m.Depth = counter[0];
            m.Low = counter[0];
            foreach (var nextEdge in GetIncidentEdges(node))
            {
                var nextNode = nextEdge.From() == node ? nextEdge.To() : nextEdge.From();
                if (!nodeDataMap[nextNode].Visited)
                {
                    edgeStack.Push(nextEdge);
                    nodeDataMap[nextNode].Parent = node;
                    VisitNode_BC(nextNode, counter, edgeStack, connectedSets, nodeDataMap);
                    if (nodeDataMap[nextNode].Low >= m.Depth)
                    {
                        connectedSets.Add(GatherBCNodes(nextEdge, edgeStack));
                    }
                    m.Low = Math.Min(
                        nodeDataMap[nextNode].Low,
                        m.Low);
                }
                else if (m.Parent != nextNode && m.Depth > nodeDataMap[nextNode].Depth)
                {

                    edgeStack.Push(nextEdge);
                    m.Low = Math.Min(nodeDataMap[nextNode].Depth,
                        m.Low);
                }
            }
        }

        private HashSet<Edge> GatherBCNodes(Edge lastEdge, Stack<Edge> edgeStack)
        {
            HashSet<Edge> nl = new HashSet<Edge>();

            while (true)
            {
                Edge edge = edgeStack.Pop();
                nl.Add(edge);
                if (lastEdge == edge)
                    break;

            }
            return nl;
        }
    }

    internal class BCNodeData
    {
        public bool Visited = false;
        public Node? Parent = null;
        public int Depth = 0;
        public int Low = 0;
        public BCNodeData() { }


    }
}