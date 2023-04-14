using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpGraph
{
    public interface IGraph<N, E>
    {

        List<E> GetEdges();

        HashSet<N> GetNodes();

    }
}

