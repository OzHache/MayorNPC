using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private Grid<PathNode> grid;
    private int x;
    private int y;

    public int gCost;       // Path from Start
    public int hCost;       // Path to End
    public int fCost;       // g+h

    public bool isWalkable;
    public PathNode cameFromNode;
    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }
    public override string ToString()
    {
        return x + "," + y;
        
    }

    internal void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
    public int GetX()
    {
        return x;
    }
    public int GetY()
    {
        return y;
    }

}
