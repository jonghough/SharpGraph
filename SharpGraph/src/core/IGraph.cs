// <copyright file="IGraph.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SharpGraph
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface IGraph<N, E>
    {
        List<E> GetEdges();

        HashSet<N> GetNodes();
    }
}
