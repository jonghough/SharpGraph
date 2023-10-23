// <copyright file="Graph.Paths.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public partial class Graph
    {
        ///
        /// <returns></returns><summary>
        /// Finds all <i>simple paths</i> between the. <code>from</code> node and the. <code>to</code> node,
        /// where a simple path is a path that does nottraverse an edge more than once or traversea node more than once.
        /// The. <code>limit</code> provides a way to short-circuit the algorithm, if the number of simple paths in the
        /// graph is very large, by stopping after a certain number of paths has been found.
        /// If. <code>limit</code> is -1, then the limit will not be taken into account, and all
        /// paths will be found.
        ///
        /// The method will returns a list of node lists, wher eeach node list represents a single path.
        /// <example>
        /// Consider the complete graph K4. Given an arbitrary startNode and endNode, we can find the
        /// simple paths.
        /// <code>
        /// g = GraphGenerator.CreateComplete(4);
        /// List<List<Node>> paths = g.FindSimplePaths(startNode, endNode, 100);
        /// </code>
        /// The number of paths returned will be 5. (2 paths of length 4 nodes, 2 of length 3, and 1 of length 2).
        /// </example>
        /// </summary>
        /// <param name="from">Node to begin</param>
        /// <param name="to">Node to end</param>
        /// <param name="limit">Limit to the number of paths to return.</param>
        /// <returns>List of node lists</returns>
        public List<List<Node>> FindSimplePaths(Node from, Node to, int limit = -1)
        {
            var result = new List<List<Node>>();
            if (from == to)
            {
                return null;
            }

            var path = new List<Node>();
            var hs = new HashSet<Node>();

            var nodeStack = new Stack<(Node, (HashSet<Node>, List<Node>))>();
            nodeStack.Push((from, (new HashSet<Node>(hs), path.Select(x => x).ToList())));
            while (nodeStack.Count > 0)
            {
                var currentPair = nodeStack.Pop();
                var current = currentPair.Item1;

                if (!currentPair.Item2.Item1.Contains(current))
                {
                    var nextHS = new HashSet<Node>(currentPair.Item2.Item1);
                    var cl = currentPair.Item2.Item2.Select(i => i).ToList();
                    cl.Add(current);
                    nextHS.Add(current);

                    var adjNodes = this.GetAdjacent(current);

                    foreach (var m in adjNodes)
                    {
                        if (m == to)
                        {
                            if (limit > -1 && cl.Count > limit)
                            {
                                // do nothing, the path is too long
                            }
                            else
                            {
                                var pt = cl.Select(i => i).ToList();
                                pt.Add(m);

                                result.Add(pt);
                            }
                        }
                        else
                        {
                            nodeStack.Push(
                                (m, (new HashSet<Node>(nextHS), cl.Select(i => i).ToList()))
                            );
                        }
                    }
                }
            }

            return result;
        }
    }
}
