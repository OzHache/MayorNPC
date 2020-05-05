using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSetup : MonoBehaviour
{
    //Reference to the pathfinding script
    private PathFinding pathFinding;

    //Grid properties
    public int rows = 10;
    public int cols = 10;
    public float CellSize;
    //Inspector variable to trigger using a visual script
    [Header("Debug with visual Grid?")]
    public bool DrawGrid;

    private void Start()
    {
        // Create a new instance of Pathfinding. This will also generate the new grid. 
        pathFinding = new PathFinding(10, 10, CellSize, DrawGrid);
        AddObjects();
        GameManager.Instance.GridComplete(true);
    }
    //Called from the Obstacle Manager, Pathfinding makes this space unwalkable
    public void AddObjects()
    {
        List<Obstacle> obstacles = ObstacleManager.GetObstacleManager.GetSceneObstacles();
        foreach (Obstacle obs in obstacles)
        {
            //make it unwalkable
            pathFinding.AddObstruction(obs.position);
        }

    }

    /// <summary>
    /// The remainder of these functions are for debugging
    /// </summary>

    private void Update()
    {
        //If debugging, draw a path from the 0,0 position to the x,y, mouse position of the mouse.
        if (Input.GetMouseButtonDown(0) && DrawGrid)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition2D();
            //get the xy value of from the grid
            pathFinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            List<PathNode> path = pathFinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                for(int i = 0; i < path.Count -1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].GetX(), path[i].GetY()) * CellSize + Vector3.one * CellSize, new Vector3(path[i + 1].GetX(), path[i + 1].GetY()) * CellSize + (Vector3.one * CellSize), Color.green);
                }
            }
        }
    }

    //Get Mouse Position in World with Z = 0f
    public static Vector3 GetMouseWorldPosition2D()
    {
        Vector3 screenPosition = Input.mousePosition;
        Camera worldCamera = Camera.main;
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0;
        return worldPosition;
    }
   
}
