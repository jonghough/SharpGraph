using System;
using System.Collections.Generic;
using System.Linq;
namespace SharpGraph
{

    public enum NodeOrdering
    {
        Random,
        NodeOrderAscending,
        NodeOrderDescending

    }
    partial class Graph
    {


        private List<Node> GetOrderedNodes(NodeOrdering nodeOrdering)
        {
            List<Node> nodes = this._nodes.ToList();

            switch (nodeOrdering)
            {
                case NodeOrdering.Random:
                    break;
                case NodeOrdering.NodeOrderAscending:
                    {
                        Dictionary<Node, int> order = new Dictionary<Node, int>();
                        foreach (var m in nodes)
                        {
                            int o = this.GetAdjacent(m).Count();
                            order[m] = o;
                        }
                        nodes = nodes.OrderBy(item => order[item]).ToList();
                        break;
                    }
                case NodeOrdering.NodeOrderDescending:
                    {
                        Dictionary<Node, int> order = new Dictionary<Node, int>();
                        foreach (var m in nodes)
                        {
                            int o = this.GetAdjacent(m).Count();
                            order[m] = o;
                        }
                        nodes = nodes.OrderByDescending(item => order[item]).ToList();
                        break;
                    }
            }
            return nodes;
        }

        /// <summary>
        /// Colors the nodes of the graph by a greedy algorithm. There are no guarantees the coloring will
        /// be optimal.
        /// The method returns a dictionary of node-color pairs, where colors are represented by <i>uint</i>s.
        /// 
        /// </summary>
        /// <param name="nodeOrdering"></param>
        /// <returns>Dictionary of node, color pairs.</returns>
        public Dictionary<Node, uint> GreedyColorNodes(NodeOrdering nodeOrdering)
        {
            var processed = new HashSet<Node>();
            var dict = new Dictionary<Node, uint>();
            var nodes = GetOrderedNodes(nodeOrdering);
            foreach (var n in nodes)
            {
                var adjacent = this.GetAdjacent(n);
                adjacent = adjacent.Intersect(processed).ToList();

                var adjColors = adjacent.Select(a => dict[a]).ToHashSet();
                bool didProcess = false;
                uint i = 0;
                for (i = 0; ; i++)
                {
                    if (!adjColors.Contains(i))
                    {
                        dict[n] = i;
                        processed.Add(n);
                        didProcess = true;
                        break;
                    }
                }
                if (!didProcess)
                {
                    processed.Add(n);
                    dict[n] = ++i;
                }
            }
            return dict;
        }
 
    }
}