using System;
using System.Collections.Generic;

namespace SharpGraph
{
    /// <summary>
    /// Disjoint set
    /// </summary> 
    internal class DisjointSet
    {
        internal int parent;
        internal int rank;


        internal DisjointSet(int parent, int rank)
        {
            this.parent = parent;
            this.rank = rank;
        }
    }




}

