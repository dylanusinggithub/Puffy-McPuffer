using System;

public class Node : IComparable<Node>
{
	public Node(int startX, int startY)
	{
		x = startX; 
		y = startY;
	}

	/// <summary>Logical x coordinate of the node. This is different from the real world space coordinate</summary>
	public int x;

	/// <summary>Logical y coordinate of the node. This is different from the real world space coordinate</summary>
	public int y;

	/// <summary>Used to keep track of which node is the parent of this one so a path is known once a solution is found</summary>
	public Node parent;

	/// <summary>Stores which nodes are connected to this one - its neigbours. This array has the same element layout as "neighbourCosts"</summary>
	public Node[] neighbours;

	/// <summary>Stores the cost to get to neighbour node from this node. This array has the same element layout as "neigbours"</summary>
	public float[] neighbourCosts;

	/// <summary>Final cost that is a summation of g+ h (used in A* and Dijkstra only)</summary>
	public float f;

	/// <summary>Goal cost that is the current cost to get to this node (used in A* and Dijkstra only)</summary>
	public float g;

	/// <summary>Heuristic cost that is the best guess to get to the goal bode from here (used in A* only)</summary>
	public float h;

	/// <summary>
	/// Keeps track of if this node is theorectically on the open list, therefore its been a neighbouring node during an iteration
	/// and therefore been added to the list for future consideration by the algorithm
	/// </summary>
	public bool onOpenList = false;

	/// <summary>
	/// Keeps track of if the this node has been considered by the algorithm so far. In AStar it is used to denote that
	/// the node is on the closed list to avoid an extra variable being used (which would be extra memory per node)
	/// </summary>
	public bool onClosedList = false;

	/// <summary>
	/// This interface function is used so that a node can be sorted in the way want so that a data structure such as a List
	/// can be used like a priority queue
	/// </summary>
	/// <param name="otherNode">The incoming other node to compare against</param>
	/// <returns>-1 to put this node first, 1 to put otherNode first, 0 indicates both nodes are at the same sort level</returns>
	public int CompareTo(Node otherNode)
	{
		if (f < otherNode.f)
		{
			return -1;
		}
		else if (f > otherNode.f)
		{
			return 1;
		}

		return 0;
	}
}
