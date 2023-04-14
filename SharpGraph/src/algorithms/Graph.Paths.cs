using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph {
    public partial class Graph {

        public List<List<Node>> FindSimplePaths (Node from, Node to, int limit = -1) {
            var result = new List<List<Node>> ();
            if (from == to) {
                return null;
            }

            var path = new List<Node> ();
            var hs = new HashSet<Node> ();

            Stack < (Node, (HashSet<Node>, List<Node>)) > nodeStack = new Stack < (Node, (HashSet<Node>, List<Node>)) > ();
            nodeStack.Push ((from, (new HashSet<Node> (hs), path.Select (x => x).ToList ())));
            while (nodeStack.Count > 0) {
                var currentPair = nodeStack.Pop ();
                var current = currentPair.Item1;

                if (!currentPair.Item2.Item1.Contains (current)) {
                    var nextHS = new HashSet<Node> (currentPair.Item2.Item1);
                    var cl = currentPair.Item2.Item2.Select (i => i).ToList ();
                    cl.Add (current);
                    nextHS.Add (current);

                    List<Node> adjNodes = GetAdjacent (current);

                    foreach (Node m in adjNodes) {

                        if (m == to) {
                            if (limit > -1 && cl.Count > limit) {
                                // do nothing, the path is too long
                            } else {
                                var pt = cl.Select (i => i).ToList ();
                                pt.Add (m);

                                result.Add (pt);
                            }
                        } else {
                            nodeStack.Push ((m, (new HashSet<Node> (nextHS), cl.Select (i => i).ToList ())));
                        }
                    }
                }
            }

            return result;
        }
    }
}