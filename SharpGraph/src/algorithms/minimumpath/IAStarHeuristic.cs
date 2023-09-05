// <copyright file="IAStarHeuristic.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SharpGraph
{
    using System;

    public interface IAStarHeuristic
    {
        float GetHeuristic(Node t, Node goal);
    }
}
