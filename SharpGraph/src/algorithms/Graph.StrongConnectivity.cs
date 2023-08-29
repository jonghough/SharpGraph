
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{

    public partial class Graph
    {


        /// <summary>
        /// Finds all strongly connected components of the graph. Components of a <i>Directed Graph</i> are strongly connected if each
        /// node of the component can be reached from every other node.
        /// The important requirement is the the graph object is a directed graph. i.e. Each <i>Edge</i> has a <i>EdgeDirection</i>
        /// component. If any edge lacks such a component an Exception is thrown.
        /// </summary>
        /// <returns>List of graphs, where each graph is equivalent to a connected component.</returns>
        public List<Graph> FindStronglyConnectedComponents()
        {
            // Kosaraju’s algorithm
            if(!IsDirected()){
                throw new Exception("Cannot run algorithm. Graph is not directed.");
            }
             
            var linkedList = new LinkedList<Node>();
            var visited = new HashSet<Node>();

            foreach (Node node in _nodes)
            {
                if (!visited.Contains(node))
                {
                    VisitNode(node, visited, linkedList);
                }
            }
            visited.Clear();
            List<HashSet<Node>> comps = new List<HashSet<Node>>();
            while (linkedList.Count > 0)
            {
                var first = linkedList.First.Value;
                linkedList.RemoveFirst();
                if (!visited.Contains(first))
                {

                    comps.Add(Assign(first, visited));
                }
            }
            List<Graph> gl = new List<Graph>();
            gl.AddRange(comps.Select(hs => new Graph(this, hs)).ToList());
            return gl;
        }

        private void VisitNode(Node node, HashSet<Node> visited, LinkedList<Node> linkedList)
        {

            visited.Add(node);
            GetAdjacent(node).ForEach(i =>
            {
                if (!visited.Contains(i))
                {
                    VisitNode(i, visited, linkedList);
                }
            });
            linkedList.AddFirst(node);
        }

        private bool IsDirected(){  
            foreach (var e in this.GetEdges())
            {
                if (!HasComponent<EdgeDirection>(e))
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
       /// Finds all weakly connected components of the graph. Components of a <i>Directed Graph</i> are weakly connected if each
        /// node of the component can be reached from every other node, regardless of edge direction.
        /// The important requirement is the the graph object is a directed graph. i.e. Each <i>Edge</i> has a <i>EdgeDirection</i>
        /// component. If any edge lacks such a component an Exception is thrown.
        /// </summary>
        /// <returns>List of graphs, where each graph is equivalent to a weakly connected component.</returns>
        public List<Graph>  FindWeaklyConnectedComponents(){
            if(!IsDirected()){
                throw new Exception("Cannot run algorithm. Graph is not directed.");
            }

            var connectedComponents =  GetConnectedComponents();
             
            var graphs = connectedComponents.Select(hs => new Graph(this, hs.ToHashSet())).ToList();
            return graphs;
        }
    }
}