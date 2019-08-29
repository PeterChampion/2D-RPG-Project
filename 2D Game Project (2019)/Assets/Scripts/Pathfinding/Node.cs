using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Defines what information that a node has, its position within the grid, whether it is obstructed or not and its A* values with g/h/f costs, along with a constructor for creating nodes
public class Node : IHeapItem<Node>
{
    public int gridX; // X position in node array
    public int gridY; // Y position in node array
    public int movementPenalty; 

    public bool walkable; // Informs us if this code is obstructed
    public Vector2 position; // The world position of the node

    public Node parent; // For the AStar algorithm, will store what node it previous came from so it can trace the shortest path

    public int gCost; // The cost of moving to the next node
    public int hCost; // The distance to the goal from this node
    public int fCost { get { return gCost + hCost; } } // The combined value of G & H

    private int heapIndex;
    public int HeapIndex { get { return heapIndex; } set { heapIndex = value; } }

    // Constructor
    public Node(bool _walkable, Vector2 _position, int _gridX, int _gridY, int _penalty)
    {
        walkable = _walkable;
        position = _position;
        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = _penalty;
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }
}
