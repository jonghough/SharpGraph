// <copyright file="DisjointSet.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SharpGraph
{
    using System;
    using System.Collections.Generic;

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
