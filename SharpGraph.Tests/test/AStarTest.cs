using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace SharpGraph
{
    class TestHeuristic : IAStarHeuristic
    {
        private float _val;

        public TestHeuristic(float val)
        {
            _val = val;
        }

        public float GetHeuristic(Node t, Node goal)
        {
            return _val;
        }
    }

    public class AStarTest
    {
        [Fact]
        public void AStarTest1()
        {
            HashSet<Node> nodes = NodeGenerator.GenerateNodes(10);
            var graph = GraphGenerator.GenerateRandomGraph(nodes, 0.6f);

            GraphGenerator.GenerateRandomWeights(graph, 0, 10);
            Graph g = graph;
            List<Node> list = new List<Node>(nodes);
            Node n1 = list[0];
            Node n2 = list[1];

            List<Node> minPath = g.FindMinPath(n1, n2, new SimpleHeuristic());
            HashSet<Node> minPathSet = new HashSet<Node>(minPath);
            //----assert there are no loops ---
            Assert.Equal(minPath.Count, minPathSet.Count);
        }

        [Fact]
        public void AStarTest2()
        {
            // A -> B -> F should be shortest path from A to F.

            Edge eAB = new Edge("A", "B");
            Edge eAC = new Edge("A", "C");
            Edge eCD = new Edge("C", "D");
            Edge eDE = new Edge("D", "E");
            Edge eAF = new Edge("A", "F");
            Edge eBF = new Edge("B", "F");
            Edge eDF = new Edge("D", "F");

            List<Edge> list = new List<Edge>();
            list.Add(eAB);
            list.Add(eAC);
            list.Add(eCD);
            list.Add(eDE);
            list.Add(eAF);
            list.Add(eBF);
            list.Add(eDF);

            Graph g = new Graph(list);
            g.AddComponent<EdgeWeight>(eAB).Weight = 3;
            g.AddComponent<EdgeWeight>(eAC).Weight = 6;
            g.AddComponent<EdgeWeight>(eCD).Weight = 1;
            g.AddComponent<EdgeWeight>(eDE).Weight = 10;
            g.AddComponent<EdgeWeight>(eAF).Weight = 12;
            g.AddComponent<EdgeWeight>(eBF).Weight = 3.5f;
            g.AddComponent<EdgeWeight>(eDF).Weight = 6.5f;

            List<Node> path = g.FindMinPath(new Node("A"), new Node("F"), new TestHeuristic(5));

            Assert.True(
                path.Contains(new Node("A"))
                    && path.Contains(new Node("B"))
                    && path.Contains(new Node("F"))
                    && path.Count == 3
            );
        }

        [Fact]
        public void AStarTest3()
        {
            // A -> F -> D -> H should be shortest path from A to H
            Node b1 = new Node("A");
            Node b2 = new Node("B");
            Node b3 = new Node("C");
            Node b4 = new Node("D");
            Node b5 = new Node("E");
            Node b6 = new Node("F");
            Node b7 = new Node("G");
            Node b8 = new Node("H");

            Edge eAB = new Edge(b1, b2);
            Edge eAG = new Edge(b1, b7);
            Edge eCD = new Edge(b3, b4);
            Edge eDH = new Edge(b4, b8);
            Edge eAF = new Edge(b1, b6);
            Edge eBF = new Edge(b2, b6);
            Edge eDF = new Edge(b4, b6);
            Edge eCG = new Edge(b3, b7);
            Edge eBC = new Edge(b2, b3);
            Edge eCE = new Edge(b3, b5);

            List<Edge> list = new List<Edge>();
            list.Add(eAB);
            list.Add(eAG);
            list.Add(eCD);
            list.Add(eDH);
            list.Add(eAF);
            list.Add(eBF);
            list.Add(eDF);
            list.Add(eCG);
            list.Add(eBC);
            list.Add(eCE);

            Graph g = new Graph(list);

            g.AddComponent<EdgeWeight>(eAB).Weight = 10;
            g.AddComponent<EdgeWeight>(eAG).Weight = 14;
            g.AddComponent<EdgeWeight>(eCD).Weight = 5;
            g.AddComponent<EdgeWeight>(eDH).Weight = 5.5f;
            g.AddComponent<EdgeWeight>(eAF).Weight = 12;
            g.AddComponent<EdgeWeight>(eBF).Weight = 3.5f;
            g.AddComponent<EdgeWeight>(eDF).Weight = 1.5f;
            g.AddComponent<EdgeWeight>(eCG).Weight = 11.5f;
            g.AddComponent<EdgeWeight>(eBC).Weight = 6.5f;
            g.AddComponent<EdgeWeight>(eCE).Weight = 6.5f;
            List<Node> path = g.FindMinPath(b1, b8, new TestHeuristic(5));

            Assert.True(
                path.Contains(b1)
                    && path.Contains(b4)
                    && path.Contains(b6)
                    && path.Contains(b8)
                    && path.Count == 4
            );
        }

        [Fact]
        public void AStarTest4()
        {
            // A ->B should be shortest path from A to B
            Node b1 = new Node("A");
            Node b2 = new Node("B");

            Edge eAB = new Edge(b1, b2);

            List<Edge> list = new List<Edge>();
            list.Add(eAB);

            Graph g = new Graph(list);
            g.AddComponent<EdgeWeight>(eAB).Weight = 10;
            List<Node> path = g.FindMinPath(b1, b2, new TestHeuristic(5));

            Assert.True(path.Contains(b1) && path.Contains(b2) && path.Count == 2);
        }

        [Fact]
        public void AStarTest5()
        {
            // A ->B path does not exist
            Node b1 = new Node("A");
            Node b2 = new Node("B");
            HashSet<Node> nodeSet = new HashSet<Node>();
            nodeSet.Add(b1);
            nodeSet.Add(b2);

            List<Edge> list = new List<Edge>();

            Graph g = new Graph(list, nodeSet);
            List<Node> path = g.FindMinPath(b1, b2, new TestHeuristic(5));

            Assert.True(path.Count == 1);
        }
    }

    public class SimpleHeuristic : IAStarHeuristic
    {
        public float GetHeuristic(Node t, Node goal)
        {
            return 2;
        }
    }
}
