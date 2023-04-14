using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SharpGraph
{
    [TestFixture]
    public class PlanarityTest
    {
        [Test]
        public void PlanarityTestSucceedsForComplete4()
        {
            var g = GraphGenerator.CreateComplete(4); 
            var isPlanar = g.IsPlanar(); 
            Assert.AreEqual(true, isPlanar); 
        }


        [Test]
        public void PlanarityTestFailsForComplete5()
        {
            var g = GraphGenerator.CreateComplete(5); 
            var isPlanar = g.IsPlanar(); 
            Assert.AreEqual(false, isPlanar);
            // removing an edge should make the graph planar
            bool b = g.RemoveEdge(new Edge("0","1"));
            isPlanar = g.IsPlanar(); 
            Assert.AreEqual(true, isPlanar);
        }

        [Test]
        public void PlanarityTestFailsForComplete33()
        {
            var g = GraphGenerator.GenerateBipartiteComplete(3, 3); 
            var isPlanar = g.IsPlanar(); 
            Assert.AreEqual(false, isPlanar); 
        }
        [Test]
        public void PlanarityTestFailsForSuperGraphOfComplete5()
        {
            var g = new Graph();
            g.AddEdge("1", "2");
            g.AddEdge("1", "3");
            g.AddEdge("1", "4");
            g.AddEdge("1", "5");
            g.AddEdge("2", "3");
            g.AddEdge("2", "4");
            g.AddEdge("2", "5");
            g.AddEdge("3", "4");
            g.AddEdge("3", "5");
            //between nodes 4 and 5 we have a new node 6. Graph is not isomorphic to K5, but
            // but is an expansion of K5
            g.AddEdge("4", "6");
            g.AddEdge("5", "6");

            var isPlanar = g.IsPlanar(); 
            Assert.AreEqual(false, isPlanar);


        }

        [Test]
        public void PlanarityTestSucceedsForBipartite2_10()
        {
            var g = GraphGenerator.GenerateBipartiteComplete(2,10); 
            var isPlanar = g.IsPlanar(); 
            Assert.AreEqual(true, isPlanar); 
        }

         [Test]
        public void PlanarityTestSucceedsForComplete4x2()
        {
            var g = GraphGenerator.CreateComplete(4);
            var h= GraphGenerator.CreateComplete(new string[]{ "A", "B", "C","D"});
            var merged = g.MergeWith(h);
            merged.AddEdge("1","A");
            //merged is now a "barbell of two copies of K4. It should be planar
            var isPlanar = merged.IsPlanar();
            Assert.AreEqual(true, isPlanar);
        }

        [Test]
        public void PlanarityTestFailsForComplete8()
        {
            var g = GraphGenerator.CreateComplete(8);
            var isPlanar = g.IsPlanar();
            Assert.AreEqual(false, isPlanar);

            //remove an edge 
            g.RemoveEdge(new Edge("1","2"));
            isPlanar = g.IsPlanar(); 
            Assert.AreEqual(false, isPlanar);
        }
    }
}