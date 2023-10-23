// <copyright file="IGraph.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System.Collections.Generic;

namespace SharpGraph
{
    public interface IGraph<N, E>
    {
        List<E> GetEdges();

        HashSet<N> GetNodes();
    }
}
