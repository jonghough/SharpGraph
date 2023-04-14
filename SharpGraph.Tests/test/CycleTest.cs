using System;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace SharpGraph
{

    [TestFixture]
    public class CompleteGraphTest
    {
        [Test]
        public void CompleteGenerationTest()
        {
            //complete graph on 10 nodes ~ 10 * 9 * 0.5 = 45 edges.
            HashSet<Node> nodes = NodeGenerator.GenerateNodes(10);
            var g = GraphGenerator.CreateComplete(nodes);
            Assert.AreEqual(45, g.GetEdges().Count);
        }
    }

    [TestFixture]
    public class OtherGraphTest
    {
        [Test]
        public void CompleteGenerationTest()
        {
            const int nodeNum = 15;
            const int width = 3;
            HashSet<Node> nodes = NodeGenerator.GenerateNodes(nodeNum);
            var g = GraphGenerator.GenerateGrid(nodes, width);
            Assert.AreEqual((width - 1) * (nodeNum / width) + width * (nodeNum / width - 1), g.GetEdges().Count);
        }
    }


    [TestFixture]
    public class CycleTesting
    {
        [Test]
        public void CycleTestComplete4()
        {

            // ------- There are 7 possible cycles in the complete graph K4. --------//
            HashSet<Node> nodes4 = NodeGenerator.GenerateNodes(4);
            var graph4 = GraphGenerator.CreateComplete(nodes4);
            var c = graph4.FindSimpleCycles();
            Assert.AreEqual(7, graph4.FindSimpleCycles().Count);
        }

        [Test]
        public void CycleTestComplete5()
        {
            // ------- There are 37 possible cycles in the complete graph K5. --------//
            HashSet<Node> nodes5 = NodeGenerator.GenerateNodes(5);
            var graph5 = GraphGenerator.CreateComplete(nodes5);
            Assert.AreEqual(37, graph5.FindSimpleCycles().Count);

        }

        [Test]
        public void CycleTestComplete6()
        {
            // ------- There are 197 possible cycles in the complete graph K6. --------//
            HashSet<Node> nodes6 = NodeGenerator.GenerateNodes(6);
            var graph6 = GraphGenerator.CreateComplete(nodes6);
            Assert.AreEqual(197, graph6.FindSimpleCycles().Count);


        }

        [Test]
        public void CycleTestComplete7()
        {
            // ------- There are 1172 possible cycles in the complete graph K7. --------//
            HashSet<Node> nodes7 = NodeGenerator.GenerateNodes(7);
            var graph7 = GraphGenerator.CreateComplete(nodes7);
            Assert.AreEqual(1172, graph7.FindSimpleCycles().Count);


        }

        [Test]
        public void CycleTestCyclic()
        {
            // Test there is only one cycle in a ... cycle graph //
            HashSet<Node> nodes5 = NodeGenerator.GenerateNodes(5);
            var cycleGraph = GraphGenerator.GenerateCycle(nodes5);
            Assert.AreEqual(1, cycleGraph.FindSimpleCycles().Count);
        }

        [Test]
        public void ChordalTest1()
        { 
            HashSet<Node> nodes5 = NodeGenerator.GenerateNodes(5);
            var cycleGraph = GraphGenerator.GenerateCycle(nodes5);
            var chords = cycleGraph.IsChordal();
            
            Assert.IsFalse(chords);
        }
    } 
}

