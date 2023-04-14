using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SharpGraph {
    [TestFixture]
    public class MISTest {

        [Test]
        public void Test1 () {
            var g = GraphGenerator.CreateComplete (10);
            var sets = g.GetMaximallyIndependentSets ();
            Assert.IsTrue (sets.Count == 10);
        }

        [Test]
        public void Test2 () {
            var nodes = new List<Node> { new Node ("A"), new Node ("B"), new Node ("C"), new Node ("C") };
            var g = new Graph (new List<Edge> (), nodes.ToHashSet ());
            var sets = g.GetMaximallyIndependentSets ();
            Assert.IsTrue (sets.Count == 1);
        }
    }
}