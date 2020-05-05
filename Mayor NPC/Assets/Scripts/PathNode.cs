using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Root class for all nodes, contains a reference to the grid
/// </summary>
public class PathNode
{
    //Reference to the Grid
    private Grid<PathNode> grid;    
    
    //This node's position on the grid
    private int x;
    private int y;

    //Cost 
    public int gCost;               // Path from Start
    public int hCost;               // Path to End
    public int fCost;               // g+h

    //If it is walkable
    public bool isWalkable;
    //The node previous to this one in the path finding
    public PathNode cameFromNode;
   
    //Initializer
    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }
    //Descriptive string override for Debugging
    public override string ToString()
    {
        return x + "," + y;
        
    }

    //interal Fcost calculation
    internal void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    //public Getters for X and Y
    public int GetX()
    {
        return x;
    }
    public int GetY()
    {
        return y;
    }

}
