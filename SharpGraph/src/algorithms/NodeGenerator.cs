using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpGraph
{
    public static class NodeGenerator
    {
        /// <summary>
        /// Returns a set of nodes, whose names are indexed by the positive integers less than
        /// or equal to the input number. 
        /// <example>
        /// <code>
        ///     var nodes = NodeGenerator.GenerateNodes(5);
        ///     /* nodes will contain 5 elements. */
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="number">number of nodes to generate.</param>
        /// <returns>set of nodes</returns>
        public static HashSet<Node> GenerateNodes(int number)
        {
            if (number < 1) throw new Exception(string.Format("argument 'number' must be a positive integer. Value given {0}", number));
            HashSet<Node> nodes = new HashSet<Node>();
            int i = number;
            while (i-- > 0)
            {
                Node b = new Node("Node_" + i);
                nodes.Add(b);
            }
            return nodes;
        }
    }


}
