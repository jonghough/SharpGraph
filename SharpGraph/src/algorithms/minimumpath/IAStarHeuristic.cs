using System;

namespace SharpGraph
{
	public interface IAStarHeuristic
	{

		float GetHeuristic (Node t, Node goal);
	}
}

