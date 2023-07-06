using System;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace SharpGraph
{
    public class MatrixTest
    {

        [Test]
        public void TestAdjacencyMatrixForK4()
        {
            var k4 = GraphGenerator.CreateComplete(4);
            var mat = k4.GetAdjacencyMatrix();
            Assert.AreEqual(4, mat.RowCount);
            Assert.AreEqual(4, mat.ColumnCount);
            Assert.AreEqual(0.0f, mat[0, 0]);
            Assert.AreEqual(1.0f, mat[0, 1]);
            Assert.AreEqual(1.0f, mat[2, 0]);
            Assert.AreEqual(1.0f, mat[3, 1]);

        }

        [Test]
        public void TestAdjacencyMatrixForK3_3()
        {
            var k4 = GraphGenerator.GenerateBipartiteComplete(3, 3);
            var mat = k4.GetAdjacencyMatrix();
            Assert.AreEqual(6, mat.RowCount);
            Assert.AreEqual(6, mat.ColumnCount);
            var rowSums = mat.RowSums();
            // in K3_3 each node is adjacent to exactly 3 other nodes.
            foreach (var rs in rowSums)
            {
                Assert.AreEqual(3, rs);
            }

        }

        [Test]
        public void TestAdjacencyMatrixEdgeless()
        {
            var g = new Graph(new List<Edge>(), NodeGenerator.GenerateNodes(25));
            var mat = g.GetAdjacencyMatrix();
            Assert.AreEqual(25, mat.RowCount);
            Assert.AreEqual(25, mat.ColumnCount);
            var rowSums = mat.RowSums();

            foreach (var rs in rowSums)
            {
                Assert.AreEqual(0, rs);
            } 
        }

        [Test]
        public void TestCompleteGraphFromAdjacencyMatrix()
        {
            var k6 = GraphGenerator.CreateComplete(6);
            var mat = k6.GetAdjacencyMatrix();  
            var g = Graph.FromAdjacencyMatrix(mat);
            Assert.AreEqual(15, g.GetEdges().Count);
            Assert.AreEqual(expected: 6, g.GetNodes().Count);

            
        }


    }
}