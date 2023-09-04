using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpGraph
{
    public static class LineGraph
    {

        internal class EdgeComparer : IEqualityComparer<Edge>
        {
            public bool Equals(Edge x, Edge y)
            {

                return x.Equals(y);
            }

            public int GetHashCode(Edge obj)
            {
                return obj.GetHashCode();
            }
        }
        /// <summary>
        /// Returns the <i>Line Graph</i> generated form the given graph, where Line Graph is the
        /// graph whose vertices are the edges of the original graph and edges of the Line Graph correspond 
        /// to incident edges in the original graph.
        /// </summary>
        /// <param name="graph">graph from which to generate the line graph</param>
        /// <param name="builder">a callback to produce the nodes and edges of the line graph.</param>
        /// <returns>A graph, representing the line graph generated from the graph argument</returns>
        public static Graph GenerateLineGraph(Graph graph, ILineGraphBuilder builder)
        {
            var edgeNodeDict = new Dictionary<Edge, Node>();
            var edgeDict = new Dictionary<int, Edge>();

            graph.BFSEdge((g, c, e) =>
          {
              Node newNode = builder.createNode(e);
              edgeNodeDict[e] = newNode;
              List<Edge> incident = graph.GetIncidentEdges(e);
              foreach (Edge inc in incident)
              {
                  if (edgeNodeDict.ContainsKey(inc) && edgeNodeDict[inc] != newNode)
                  {
                      var edge = builder.createEdge(edgeNodeDict[inc], newNode);
                      var ch = e.GetHashCode();
                      var ih = inc.GetHashCode();
                      int hash = ((ch * ih) * 1337) - 13 * (((ch + ih) << 2) ^ (ch + ih));

                      if (edgeDict.ContainsKey(hash))
                      {

                      }
                      else
                      {
                          edgeDict[hash] = edge;
                      }
                  }
              }
          });

            return new Graph(new List<Edge>(edgeDict.Values), new HashSet<Node>(edgeNodeDict.Values));
        }
    }

    public interface ILineGraphBuilder
    {
        Node createNode(Edge edge);

        Edge createEdge(Node nodeFrom, Node nodeTo);
    }
}
