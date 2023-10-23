// <copyright file="Graph.Matrix.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public class MatrixException : Exception
    {
        public MatrixException(string msg)
            : base(msg) { }
    }

    public partial class Graph
    {
        public static Graph FromAdjacencyMatrix(Matrix<float> adjacencyMatrix)
        {
            var mat = adjacencyMatrix;
            if (mat.RowCount != mat.ColumnCount)
            {
                throw new MatrixException(
                    "Adjacency matrix must have same number of columns and rows."
                );
            }

            var nodeCount = mat.RowCount;
            var nodes = NodeGenerator.GenerateNodes(nodeCount).ToList();
            var edges = new List<Edge>();
            for (var i = 0; i < nodeCount; i++)
            {
                for (var j = 0; j < nodeCount; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (mat[i, j] > 0 && i > j)
                    {
                        edges.Add(new Edge(nodes[i], nodes[j]));
                    }
                }
            }

            return new Graph(edges, nodes.ToHashSet());
        }

        /// <summary>
        /// Creates an adjacency matrix from the graph. The matrix will have <i>N</i>
        /// rows and <i>N</i> columns where <i>N</i> is euqal to the number of nodes in the graph.
        /// The elements of the matrix will be 1.0 if the [i,j]th edge (the edge between the ith node and jth node)
        /// exists, and 0.0 otherwise.
        /// For this construction the order of the nodes will be the lexicographic order of the node labels.
        /// </summary>
        /// <returns>Matrix of floats.</returns>
        public Matrix<float> GetAdjacencyMatrix()
        {
            var nl = this.nodes.ToList().OrderBy(x => x.GetLabel()).ToList();
            var nc = nl.Count();
            var l = new float[nc, nc];
            for (var i = 0; i < nc; i++)
            {
                for (var j = 0; j < nc; j++)
                {
                    if (i == j)
                    {
                        l[i, j] = 0.0f;
                    }
                    else
                    {
                        l[i, j] = this.IsAdjacent(nl[i], nl[j]) ? 1.0f : 0.0f;
                    }
                }
            }

            Matrix<float> mat = DenseMatrix.OfArray(l);
            return mat;
        }

        public Matrix<float> CreateEdgeWeightMatrix(bool isDirected = false)
        {
            var nl = this.nodes.ToList().OrderBy(x => x.GetLabel()).ToList();
            var nc = nl.Count();
            var l = new float[nc, nc];

            for (var i = 0; i < nc; i++)
            {
                for (var j = 0; j < nc; j++)
                {
                    if (i == j)
                    {
                        l[i, j] = 0.0f;
                        continue;
                    }

                    var hs = new HashSet<Node>();
                    hs.Add(nl[i]);
                    hs.Add(nl[j]);
                    var e = this.GetEdge(hs);
                    float w = 0;
                    if (e.HasValue)
                    {
                        if (isDirected)
                        {
                            var edgeDir = this.GetComponent<EdgeDirection>(e.Value);

                            // if ith node is "from" and the edge direction is not going from "to" to "from".
                            if (edgeDir.Direction != Direction.Backwards && e.Value.From() == nl[i])
                            {
                                w = this.GetComponent<EdgeWeight>(e.Value).Weight;
                            }
                        }
                        else
                        {
                            w = this.GetComponent<EdgeWeight>(e.Value).Weight;
                        }
                    }

                    l[i, j] = w;
                }
            }

            Matrix<float> mat = DenseMatrix.OfArray(l);
            return mat;
        }
    }
}
