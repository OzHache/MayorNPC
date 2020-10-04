using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// The Obstacle Manager takes all the obstacles in the scene and adds them to the list of obstacles on the grid 
/// </summary>

public class ObstacleManager : MonoBehaviour
{
    //Instance reference
    public static ObstacleManager GetObstacleManager;

    //References to the Obstacles Tilemap
    public Tilemap obstacles;

    //List of POI obstacles
    public List<Obstacle> InterestPoints = new List<Obstacle>();


    public List<Obstacle> SceneObstacles = new List<Obstacle>();
     

    // Assign Instance references before the first frame. 
    void Awake()
    {
        //Assign the static property for the Obstacle components in the game to add themselves to. 
        GetObstacleManager = this;
    }
   
    //A public getter for returning all of the scene Objects

    public List<Obstacle> GetSceneObstacles(Vector2 gridSize)
    {
        //Search the Obstacles Tile map within the Grid
        var parentGameObject = Instantiate(new GameObject());
        parentGameObject.name = "TileMapObstacles";
        SceneObstacles.Clear();

        for (var x = 0; x< gridSize.x; x++)
        {
            for(var y = 0; y < gridSize.y; y++)
            {
                if(obstacles.GetTile(new Vector3Int(x,y,0))!= null)
                {
                    Obstacle obstacle = parentGameObject.AddComponent<Obstacle>();
                    obstacle.tileMapPosition.x = x;
                    obstacle.tileMapPosition.y = y;
                    SceneObstacles.Add(obstacle);
                }
            }
        }

        //populate with current obstacles
        SceneObstacles.AddRange(FindObjectsOfType<Obstacle>());

        //check all the obstacles to see if they are POIs
        foreach (Obstacle obs in SceneObstacles)
            if (obs.isPOI)
                InterestPoints.Add(obs);

        return SceneObstacles;
    }

    /// <summary>
    /// This method returns a list of POI's 
    /// </summary>
    /// <param name="position">The Position of the Requesting NPC</param>
    /// <returns>Returns a sorted list of interst POI within the interst Radius</returns>
    internal List<Obstacle> GetPOIs(Vector2 position, float interestRadius)
    {
        List<Obstacle> validInterestPoints = new List<Obstacle>();
        foreach(Obstacle poi in InterestPoints)
        {
            if (Vector2.Distance(position, poi.position) <= interestRadius)
                validInterestPoints.Add(poi);
        }
        return SortByDistance(validInterestPoints,position);
    }

    /// <summary>
    /// takes the POI list and sorts by closest
    /// </summary>
    /// <param name="POIs">List of POIs</param>
    /// <param name="position"> position to check on distance</param>
    /// <returns></returns>
    private List<Obstacle>SortByDistance(List<Obstacle> POIs, Vector2 position)
    {
        //Bubble sort
        //Temporary Obstacle
        Obstacle temp;
        //each pass
        for(int p = 0; p <= POIs.Count-2; p++)
        {
            //index
            for(int i = 0; i < POIs.Count -2; i++)
            {
                //compare distances
                if (Vector2.Distance(position, POIs[i].position) > Vector2.Distance(position, POIs[i+1].position))
                {
                    //temporaraly delocate i+1 (next)
                    temp = POIs[i + 1];
                    //set next to this(i)
                    POIs[i + 1] = POIs[i];
                    //relocate temp to i
                    POIs[i] = temp;
                }
            }
        }
        //reverse the order so it is closes first
        POIs.Reverse();
        return POIs;
    }
}
