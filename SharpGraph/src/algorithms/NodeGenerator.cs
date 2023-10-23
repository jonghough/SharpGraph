// <copyright file="NodeGenerator.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
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
        /// <returns>set of nodes.</returns>
        public static HashSet<Node> GenerateNodes(int number)
        {
            if (number < 1)
            {
                throw new Exception(
                    string.Format(
                        "argument 'number' must be a positive integer. Value given {0}",
                        number
                    )
                );
            }

            var nodes = new HashSet<Node>();
            var i = number;
            while (i-- > 0)
            {
                var b = new Node("Node_" + i);
                nodes.Add(b);
            }

            return nodes;
        }
    }
}
