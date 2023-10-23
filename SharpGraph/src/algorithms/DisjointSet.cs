// <copyright file="DisjointSet.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

namespace SharpGraph
{
    /// <summary>
    /// Disjoint set.
    /// </summary>
    internal class DisjointSet
    {
        internal int Parent;
        internal int Rank;

        internal DisjointSet(int parent, int rank)
        {
            this.Parent = parent;
            this.Rank = rank;
        }
    }
}
