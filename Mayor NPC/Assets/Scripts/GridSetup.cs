using System.Collections;
using System.Collections.Generic;

public class GridSetup
{
    //Reference to the pathfinding script
    private PathFinding pathFinding;

    //Grid properties
    private int rows = 10;
    private int cols = 10;
    private float cellSize;

    //Scene Loader can check this to see if the Grid has been built
    public bool complete {  get; private set; }

  public void BuildGrid(int rows, int cols, float cellSize, bool willDrawGrid)
    {
        //Set up properties
        this.rows = rows;
        this.cols = cols;
        this.cellSize = cellSize;
        //Set complete to false
        complete = false;
        // Create a new instance of Pathfinding. This will also generate the new grid. 
        pathFinding = new PathFinding(10, 10, this.cellSize, willDrawGrid);
        complete = true;
    }
    //Called from the Obstacle Manager, Pathfinding makes this space unwalkable
    public void AddObjects(List<Obstacle> obstacles)
    {
        foreach (Obstacle obs in obstacles)
        {
            //make it unwalkable
            pathFinding.AddObstruction(obs.position);
        }
    }
}
