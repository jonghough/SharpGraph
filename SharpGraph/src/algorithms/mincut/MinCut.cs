// <copyright file="MinCut.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public partial class Graph
    {
        /// <summary>
        /// Finds the minimum cut of the graph. Graph is assumed ot be unweighted, and undirected.
        /// If the graph is not connected a. <code>NotConnectedException</code> will be thrown.
        /// This algorithm is based on<i>Kagr's algorthm</i>
        /// <see href="http://www.columbia.edu/~cs2035/courses/ieor6614.S09/Contraction.pdf">Kager Algorithm</see>.
        /// <code>
        /// HashSet&lt;Node&gt; nodes = NodeGenerator.GenerateNodes(8);
        /// var g = GraphGenerator.CreateComplete(nodes);
        /// int cutEdges = g.FindMinCut();
        /// </code>
        /// In the above example, <i>cutEdges</i> should equal 7, sd s mimum of 7 edge removals
        /// would be needed to make K8 disconnected.
        /// </summary>
        /// <returns>The minimum number of edge removals to make a disconnected graph.</returns>
        public int FindMinCut()
        {
            if (!this.IsConnected())
            {
                throw new NotConnectedException("Graph is not connected.");
            }

            var edgeList = this.GetEdges();

            var modGraph = new Graph(edgeList, this.GetNodes());

            edgeList.ForEach(e => modGraph.AddComponent<MultiplicityComponent>(e));
            var r = new Random();
            var nCount = modGraph.GetNodes().Count;
            while (nCount > 2)
            {
                var next = r.Next(0, modGraph.GetEdges().Count);
                var toRemove = modGraph.GetEdges()[next];
                modGraph = modGraph.ContractEdge(toRemove);
                nCount = modGraph.GetNodes().Count;
            }

            var sum = modGraph.edges
                .Select(e => modGraph.GetComponent<MultiplicityComponent>(e).Multiplicity)
                .Sum();
            return sum;
        }

        private Graph ContractEdge(Edge edge)
        {
            if (this.GetEdges().Contains(edge) == false)
            {
                throw new Exception("Edge does not exist on this graph.");
            }

            // arbitrary, choose the form node to make the new node.
            var newNode = edge.From();
            var incident = this.GetIncidentEdges(edge.To());
            var toRemove = new HashSet<Edge>();
            var newEdges = new HashSet<Edge>();
            var dict = new Dictionary<Edge, int>();
            foreach (var ed in this.edges)
            {
                dict[ed] = this.GetComponent<MultiplicityComponent>(ed).Multiplicity;
            }

            dict[edge] = 0;
            foreach (var e in incident)
            {
                if (e == edge)
                {
                    continue;
                }

                if (toRemove.Contains(e))
                {
                    continue;
                }
                else if (e.From() == edge.To())
                {
                    var mult = this.GetComponent<MultiplicityComponent>(e).Multiplicity;
                    toRemove.Add(e);
                    var rex = new Edge(newNode, e.To());

                    newEdges.Add(rex);
                    if (!dict.ContainsKey(rex))
                    {
                        dict[rex] = mult;
                    }
                    else
                    {
                        dict[rex] += mult;
                    }

                    var rexi = new Edge(e.To(), newNode);

                    if (this.edges.Contains(rexi))
                    {
                        if (!toRemove.Contains(rexi))
                        {
                            toRemove.Add(rexi);
                            if (dict.ContainsKey(rexi))
                            {
                                dict[rex] += dict[rexi];
                            }
                        }
                    }
                }
                else if (e.To() == edge.To())
                {
                    var mult = this.GetComponent<MultiplicityComponent>(e).Multiplicity;
                    toRemove.Add(e);
                    var rex = new Edge(e.From(), newNode);
                    newEdges.Add(rex);
                    if (!dict.ContainsKey(rex))
                    {
                        dict[rex] = mult;
                    }
                    else
                    {
                        dict[rex] += mult;
                    }

                    var rexi = new Edge(newNode, e.From());

                    if (this.edges.Contains(rexi))
                    {
                        if (!toRemove.Contains(rexi))
                        {
                            toRemove.Add(rexi);
                            if (dict.ContainsKey(rexi))
                            {
                                dict[rex] += dict[rexi];
                            }
                        }
                    }
                }
            }

            var nodes = this.GetNodes();
            var edges = this.GetEdges();
            nodes.Remove(edge.To());
            edges.Remove(edge);
            foreach (var ex in edges)
            {
                newEdges.Add(ex);
            }

            newEdges.Remove(edge);
            foreach (var deadEdge in toRemove)
            {
                newEdges.Remove(deadEdge);
            }

            var g = new Graph(new List<Edge>(newEdges), nodes);

            g.edges.ForEach(e =>
            {
                var comp = g.AddComponent<MultiplicityComponent>(e);

                if (dict.ContainsKey(e))
                {
                    comp.Multiplicity = dict[e];
                }
            });
            return g;
        }
    }

    internal class MultiplicityComponent : EdgeComponent
    {
        public int Multiplicity = 1;

        public override void Copy(EdgeComponent edgeComponent)
        {
            if (edgeComponent == null)
            {
                throw new Exception("Null edge component");
            }

            if (edgeComponent is MultiplicityComponent)
            {
                (edgeComponent as MultiplicityComponent).Multiplicity = this.Multiplicity;
            }
            else
            {
                throw new Exception("Type is not correct");
            }
        }
    }
}
