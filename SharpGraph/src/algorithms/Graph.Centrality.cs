using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public partial class Graph
    {

        /// <summary>
        /// Gets the degree centrality of the given node, where degree centrality is the
        /// ratio of the number of adjacent nodes to the total number of nodes in the graph.
        /// <code>
        /// float nodeCentrality = g.GetDegreeCentrality(new Node("A"));
        /// </code>
        /// <see href="https://en.wikipedia.org/wiki/Centrality">Centrality.</see> 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public float GetDegreeCentrality(Node node)
        {

            if (this._nodes.Count == 0) return 0;
            var adjNodes = this.GetAdjacent(node);
            return adjNodes.Count * 1.0f / this._nodes.Count;
        }

        /// <summary>
        /// Gets the degree centrality of all nodes in the graph. Centrality gives a measure of
        /// "importance" for a given node in the graph, depending on how many nodes are adjacent
        /// to the given node.
        /// <see href="https://en.wikipedia.org/wiki/Centrality">Centrality.</see> 
        /// </summary> 
        /// <returns>Dictionary of node, float, where each pair gives a node and its centrality value.</returns>
        public Dictionary<Node, float> GetDegreeCentrality()
        {
            List<float> l = new List<float>();
            if (this._nodes.Count == 0) return new Dictionary<Node, float>();
            return this._nodes.Select(node => new KeyValuePair<Node, float>(node, GetDegreeCentrality(node))).ToDictionary(pair => pair.Key, pair => pair.Value);

        }
    }
}