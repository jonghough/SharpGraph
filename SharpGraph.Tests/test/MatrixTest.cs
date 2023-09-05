using System;
using Xunit;
using System.Collections;
using System.Collections.Generic;

namespace SharpGraph
{
    public class MatrixTest
    {
        [Fact]
        public void TestAdjacencyMatrixForK4()
        {
            var k4 = GraphGenerator.CreateComplete(4);
            var mat = k4.GetAdjacencyMatrix();
            Assert.Equal(4, mat.RowCount);
            Assert.Equal(4, mat.ColumnCount);
            Assert.Equal(0.0f, mat[0, 0]);
            Assert.Equal(1.0f, mat[0, 1]);
            Assert.Equal(1.0f, mat[2, 0]);
            Assert.Equal(1.0f, mat[3, 1]);
        }

        [Fact]
        public void TestAdjacencyMatrixForK3_3()
        {
            var k4 = GraphGenerator.GenerateBipartiteComplete(3, 3);
            var mat = k4.GetAdjacencyMatrix();
            Assert.Equal(6, mat.RowCount);
            Assert.Equal(6, mat.ColumnCount);
            var rowSums = mat.RowSums();
            // in K3_3 each node is adjacent to exactly 3 other nodes.
            foreach (var rs in rowSums)
            {
                Assert.Equal(3, rs);
            }
        }

        [Fact]
        public void TestAdjacencyMatrixEdgeless()
        {
            var g = new Graph(new List<Edge>(), NodeGenerator.GenerateNodes(25));
            var mat = g.GetAdjacencyMatrix();
            Assert.Equal(25, mat.RowCount);
            Assert.Equal(25, mat.ColumnCount);
            var rowSums = mat.RowSums();

            foreach (var rs in rowSums)
            {
                Assert.Equal(0, rs);
            }
        }

        [Fact]
        public void TestCompleteGraphFromAdjacencyMatrix()
        {
            var k6 = GraphGenerator.CreateComplete(6);
            var mat = k6.GetAdjacencyMatrix();
            var g = Graph.FromAdjacencyMatrix(mat);
            Assert.Equal(15, g.GetEdges().Count);
            Assert.Equal(expected: 6, g.GetNodes().Count);
        }
    }
}
