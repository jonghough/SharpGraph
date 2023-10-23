// <copyright file="Graph.Cliques.cs" company="Jonathan Hough">
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
        /// Finds all <i>maximal cliques</i> in the graph. The method returns
        /// a list of <i>hashsets</i>, where each hashset represents the nodes in the
        /// clique containing those nodes. This is an implementation of <i>Bron-Kerbosch</i>
        /// algorithm.
        /// <code>
        /// var cliques = g.FindMaximalCliques();
        /// </code>
        /// </summary>
        /// <returns>List of sets, where each set represents an optimal clique.</returns>
        public List<HashSet<Node>> FindMaximalCliques()
        {
            var r = new HashSet<Node>();
            var x = new HashSet<Node>();
            var p = this.GetNodes();

            var cliques = this.FindCliques(r, x, p);
            return cliques;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="r"></param>
        /// <param name="x"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private List<HashSet<Node>> FindCliques(HashSet<Node> r, HashSet<Node> x, HashSet<Node> p)
        {
            var nodeSetList = new List<HashSet<Node>>();
            if (p.Count == 0 && x.Count == 0)
            {
                nodeSetList.Add(r);
                return nodeSetList;
            }
            else
            {
                var used = new HashSet<Node>();
                foreach (var n in p)
                {
                    if (used.Contains(n))
                    {
                        continue;
                    }

                    var r_ = new HashSet<Node>(r);
                    var x_ = new HashSet<Node>(x);
                    var p_ = new HashSet<Node>(this.GetAdjacent(n));
                    p_.RemoveWhere(i => used.Contains(i));
                    p_.IntersectWith(p);
                    r_.Add(n);
                    x_.UnionWith(used);
                    x_.IntersectWith(this.GetAdjacent(n));
                    var fc = this.FindCliques(r_, x_, p_);
                    nodeSetList.AddRange(fc);
                    used.Add(n);
                }
            }

            return nodeSetList;
        }
    }
}
