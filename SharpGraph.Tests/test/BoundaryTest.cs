using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace SharpGraph {
    [TestFixture]
    public class BoundaryTest
    {
        
        [Test]
        public void NodeBoundaryTest1(){
            var g = GraphGenerator.CreateComplete(6);
            var nodes = g.GetNodes().ToList();
            var hs = new HashSet<Node>();
            hs.Add(nodes[0]);
            var bdry = g.NodeBoundary(hs);
            Assert.AreEqual(5, bdry.Count);
            Assert.IsFalse(bdry.Contains(nodes[0]));

            hs.Add(nodes[1]);
            hs.Add(nodes[2]);
            bdry = g.NodeBoundary(hs);
            Assert.AreEqual(3, bdry.Count);
            Assert.IsFalse(bdry.Contains(nodes[0]));
            Assert.IsFalse(bdry.Contains(nodes[1]));
            Assert.IsFalse(bdry.Contains(nodes[2]));

            
        }

        [Test]
        public void EdgeBoundaryTest1(){
            var g = GraphGenerator.CreateComplete(6);
            var nodes = g.GetNodes().ToList();
            var hs = new HashSet<Node>();
            hs.Add(nodes[0]);
            var bdry = g.EdgeBoundary(hs);
            Assert.AreEqual(5, bdry.Count); 

            hs.Add(nodes[1]);
            hs.Add(nodes[2]);
            hs.Add(nodes[3]);
            bdry = g.EdgeBoundary(hs); 
            // all edges to nodes 4,5. So 8 edges.
            Assert.AreEqual(8, bdry.Count); 

            
        }
    }
}