using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding {
    // to implement this on a pathfinding character get a path by calling PathFinding.Instance.FindPath( my position, target position)
    //this returns a Vector 3 list of waypoints

    //Basic Movement Cost

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    public static PathFinding Instance { get; private set; }


    private Grid<PathNode> grid;
    private List<PathNode> openList;                                //Valid locations
    private List<PathNode> closedList;                              //Invalid Locations

    public PathFinding(int width, int height, float cellSize)
    {
        Instance = this;
        grid = new Grid<PathNode>(width, height, cellSize, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }
    public Grid<PathNode> GetGrid()
    {
        return grid;
    }

    public List<Vector3> FindPath( Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);
        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if (path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach(PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.GetX(), pathNode.GetY()) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
            }
            return vectorPath;
        }

    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {

        PathNode startNode = grid.GetGridObject(startX, startY);    //Starting location 
        PathNode endNode = grid.GetGridObject(endX, endY);
        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        /* 
         * Set all g to inf 
         * calculate f for all grid locations (g is cost from start)
         * cameFromNode = null 
         * This wipes the data for the grid location;
         */

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);
        startNode.CalculateFCost();

        //while we are still considering valid locations in the open list
        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);                             //node with the currently lowest Cost
            if(currentNode == endNode)
            {
                //Reached final node
                return CalculatePath(endNode);
            }
            openList.Remove(currentNode);                                                   //This node has already been calculated
            closedList.Add(currentNode);

            //Cycle through the Neighbours of this node
            foreach(PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }
                int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);
                //is this a faster route
                if(tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistance(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();
                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        //Out of nodes on the open list
        //if we reach here,  there is no valid path
        Debug.Log("No valid path to the position selected");
        return null;
    }

    private PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        if(currentNode.GetX() -1 >= 0) //not out on the left bounds (LEFT, LEFT DOWN, LEFT UP)
        {
            neighbourList.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY()));
            if (currentNode.GetY() - 1 >= 0) neighbourList.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY() - 1));
            if (currentNode.GetY() + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY() + 1));
        }
        if(currentNode .GetX()+1 < grid.GetWidth()) // not out on the right bounds (RIGHT, RIGHT DOWN, RIGHT UP)
        {
            neighbourList.Add(GetNode(currentNode.GetX() + 1, currentNode.GetY()));
            if (currentNode.GetY() - 1 >= 0) neighbourList.Add(GetNode(currentNode.GetX() + 1, currentNode.GetY() - 1));
            if (currentNode.GetY() + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.GetX() +1, currentNode.GetY()+1));
        }
        //UP AND DOWN
        if (currentNode.GetY() - 1 >= 0) neighbourList.Add(GetNode(currentNode.GetX(), currentNode.GetY() - 1));
        if (currentNode.GetY() + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.GetX(), currentNode.GetY() + 1));

        return neighbourList;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        //cycle from the end point through the camefreom nodes until you reach the begining and add them to the list
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistance(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.GetX() - b.GetX());
        int yDistance = Mathf.Abs(a.GetY() - b.GetY());
        int remaining = Mathf.Abs(xDistance - yDistance);
        //Return the amount we can move diagonaly plus the amount we must move straight
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];

        for(int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}
