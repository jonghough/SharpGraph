// <copyright file="IAStarHeuristic.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

namespace SharpGraph
{
    public interface IAStarHeuristic
    {
        float GetHeuristic(Node t, Node goal);
    }
}
