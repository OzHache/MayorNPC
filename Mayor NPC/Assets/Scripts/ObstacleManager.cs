using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The Obstacle Manager takes all the obstacles in the scene and adds them to the list of obstacles on the grid 
/// </summary>

public class ObstacleManager : MonoBehaviour
{
    //Instance reference
    public static ObstacleManager GetObstacleManager;
    //List of POI obstacles
    public List<Obstacle> InterestPoints = new List<Obstacle>();
    //Reference to the Grid Setup Script
    public GridSetup gridSetup;


    private List<Obstacle> SceneObstacles = new List<Obstacle>();
     

    // Assign Instance references before the first frame. 
    void Awake()
    {
        //Assign the static property for the Obstacle components in the game to add themselves to. 
        GetObstacleManager = this;
    }

    //Start
    private void Start()
    {
        //confirm reference to Grid Setup
        if(gridSetup == null)
        {
            //Send a warning and disable this script if there is no GridSetup property assigned in the inspector
            Debug.LogWarning(gameObject.name + " requires a refernece to the GridSetup Object assigned in the inspector", this.gameObject);
            //disable this script
            this.enabled = false;
        }
        //start the coroutine to add obstacles to the Grid System
        //StartCoroutine(AddObstaclesToGrid());
    }
   
    //A public getter for returning all of the scene Objects

    public List<Obstacle> GetSceneObstacles()
    {
        //reset the scene list of obstacles
        SceneObstacles.Clear();
        //populate with current obstacles
        SceneObstacles.AddRange(FindObjectsOfType<Obstacle>());

        //check all the obstacles to see if they are POIs
        foreach (Obstacle obs in SceneObstacles)
            if (obs.isPOI)
                InterestPoints.Add(obs);

        return SceneObstacles;
    }
}
