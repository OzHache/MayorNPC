using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSetup : MonoBehaviour
{
    private PathFinding pathFinding;

    public int rows = 10;
    public int cols = 10;
    public float CellSize;

    private void Start()
    {
        pathFinding = new PathFinding(10, 10, CellSize);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition2D();
            //get the xy value of from the grid
            pathFinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            List<PathNode> path = pathFinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                for(int i = 0; i < path.Count -1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].GetX(), path[i].GetY()) * 10f + Vector3.one * 5f, new Vector3(path[i + 1].GetX(), path[i + 1].GetY()) * 10f + (Vector3.one * 5f), Color.green);
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
