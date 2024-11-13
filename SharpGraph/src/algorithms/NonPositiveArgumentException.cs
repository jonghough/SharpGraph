// <copyright file="NonPositiveArgumentException.cs" company="Jonathan Hough">
// Copyright (C) 2023 Jonathan Hough.
// Copyright Licensed under the MIT license.
// See LICENSE file in the samples root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph
{
    public class NonPositiveArgumentException : Exception
    {
        public NonPositiveArgumentException(string message)
            : base(message) { }
    }
}
