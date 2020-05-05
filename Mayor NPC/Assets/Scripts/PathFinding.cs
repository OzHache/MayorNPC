using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathFinding {
    // To implement this on a pathfinding character get a path by calling PathFinding.Instance.FindPath( my position, target position)
    // this returns a Vector 3 list of waypoints

    //Basic Movement Cost

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    //Instance Reference
    public static PathFinding Instance { get; private set; }


    private Grid<PathNode> grid;                                    //Grid instance
    private List<PathNode> openList;                                //Valid locations
    private List<PathNode> closedList;                              //Invalid Locations

    #region Initializers and public getters
    //Initializer 
    public PathFinding(int width, int height, float cellSize, bool drawGrid)
    {
        // There should only ever be one Instance
        if(Instance != null)
        {
            Debug.LogWarning("Another Instance of Path Finding exist");
        }
        Instance = this;
        //Generate a new Instance of Grid
        grid = new Grid<PathNode>(width, height, cellSize, Vector3.zero, drawGrid, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }
    //Public Getter for Grid
    public Grid<PathNode> GetGrid()
    {
        return grid;
    }
    //Add objects that are not walkable to the grid
    public void AddObstruction(Vector3 atLocation)
    {
        int x = Mathf.FloorToInt(atLocation.x);
        int y = Mathf.FloorToInt(atLocation.y);
        PathNode node = GetNode(x, y);
        node.isWalkable = false;

    }

    #endregion

    //Public Method for finding a valid path given a start and end position in worldSpace as an overload to the Find Path with x and y coordinates
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


    /// <summary>
    /// Path Building Method
    /// H cost is the theoretical cost to go straight to the end point from this point
    /// G Cost is the cost to it takes to get here from the start position
    /// F Cost is the G cost plus the H cost. 
    /// </summary>
    /// <param name="startX">Starting Position's X value</param>
    /// <param name="startY">Starting Position's Y value</param>
    /// <param name="endX">Ending Position's X Value</param>
    /// <param name="endY">Ending Position's Y Value</param>
    /// <returns> A List of PathNodes that map a path from the start position to the end position</returns>
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {

        PathNode startNode = grid.GetGridObject(startX, startY);    //Starting location 
        PathNode endNode = grid.GetGridObject(endX, endY);          //End location
        openList = new List<PathNode> { startNode };                //Points we are still considering
        closedList = new List<PathNode>();                          //Points we are no longer considering

        /* 
         * Set all g to inf 
         * calculate f for all grid locations (g is cost from start)
         * cameFromNode = null 
         * This wipes the data for the grid location;
         */

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            //Reset all the G cost to max values
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }
        //reset the Start Node G Cost to 0, 
        startNode.gCost = 0;
        //Set the cost to get to the end point
        startNode.hCost = CalculateDistance(startNode, endNode);
        //Calculate the F cost
        startNode.CalculateFCost();

        //while we are still considering valid locations in the open list
        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);                             //node with the currently lowest Cost

            //If we have readed the end, Calcualte the path
            if(currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            //This node is being calculated so it is no longer goign to be considered after this.
            openList.Remove(currentNode);                                                   
            closedList.Add(currentNode);

            //Cycle through the Neighbours of this node
            foreach(PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                //Skip if the neighbour is on the closed list.
                if (closedList.Contains(neighbourNode)) continue;

                //Add to the closed list if this is not walkable
                if (!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                //Compare the Gcost of this Node with that of the neighbour we are currently considering
                int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);
                //is this a faster route, then add it to the open list
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
    
    /// <summary>
    /// Gets omni directional neighbours of a given path node
    /// </summary>
    /// <param name="currentNode">The Current node to checck for neighbours</param>
    /// <returns> A List of Nodes that neighbour the current node</returns>
    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        
        //Return list for all neighbours of the CN -> currentNode
        List<PathNode> neighbourList = new List<PathNode>();
        // [3,0,0]
        // [1,CN,0]
        // [2,0,0]
        if (currentNode.GetX() -1 >= 0) //not out on the left bounds (LEFT, LEFT DOWN, LEFT UP)
        {
            //1
            neighbourList.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY()));
            //2
            if (currentNode.GetY() - 1 >= 0) neighbourList.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY() - 1));
            //3
            if (currentNode.GetY() + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY() + 1));
        }
        // [0,0,3]
        // [0,CN,1]
        // [0,0,2]
        if (currentNode .GetX()+1 < grid.GetWidth()) // not out on the right bounds (RIGHT, RIGHT DOWN, RIGHT UP)
        {
            //1
            neighbourList.Add(GetNode(currentNode.GetX() + 1, currentNode.GetY()));
            //2
            if (currentNode.GetY() - 1 >= 0) neighbourList.Add(GetNode(currentNode.GetX() + 1, currentNode.GetY() - 1));
            //3
            if (currentNode.GetY() + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.GetX() +1, currentNode.GetY()+1));
        }
        //UP AND DOWN
        // [0,2,0]
        // [0,CN,0]
        // [0,1,0]
        //1
        if (currentNode.GetY() - 1 >= 0) neighbourList.Add(GetNode(currentNode.GetX(), currentNode.GetY() - 1));
        //2
        if (currentNode.GetY() + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.GetX(), currentNode.GetY() + 1));
        //remove any neighbours that are not walkable
        foreach(PathNode node in neighbourList)
        {
            if (!node.isWalkable)
            {
                closedList.Add(node);
            }
        }
        return neighbourList;
    }

    /// <summary>
    /// Gets the grid object at that location, used in th Get Nieghbour list method
    /// </summary>
    /// <param name="x"> X Coordinate of the Grid Object</param>
    /// <param name="y"> Y Coordinate of the Grid Object</param>
    /// <returns>Pathnode at the X Y position</returns>
    private PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    /// <summary>
    /// cycle from the end point through the camefrom nodes until you reach the begining and add them to the list
    /// </summary>
    /// <param name="endNode">Starting with this last node</param>
    /// <returns>An ordered list of nodes from the start to the end of the path</returns>
    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        //Start the list with the last node
        path.Add(endNode);
        //Cylce based on this current node
        PathNode currentNode = endNode;
        //While there is a node this one came from
        while (currentNode.cameFromNode != null)
        {
            //Addd the came from node and set current node to the camefrom node
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        //reverse the list so the first node 0 is the starting point
        path.Reverse();
        return path;
    }

    //theoretical straight line distance calculation
    private int CalculateDistance(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.GetX() - b.GetX());
        int yDistance = Mathf.Abs(a.GetY() - b.GetY());
        int remaining = Mathf.Abs(xDistance - yDistance);
        //Return the amount we can move diagonaly plus the amount we must move straight
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    /// <summary>
    /// iterates over all the nodes and returns the lowest FCost
    /// </summary>
    /// <param name="pathNodeList"></param>
    /// <returns>Lowest F Cost Node</returns>
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
