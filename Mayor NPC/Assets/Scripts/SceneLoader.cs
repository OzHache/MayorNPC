using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Debug with grid")]
    [SerializeField]
    private bool willDrawGrid;
    //References
    private GridSetup gridSetup;
    public ObstacleManager obstacleManager;

    //Called once enabled
    private void Start()
    {
        obstacleManager = ObstacleManager.GetObstacleManager;
        //Start the Build Scene Coroutine
        StartCoroutine(BuildScene());
    }
    IEnumerator BuildScene()
    {
        //Check for the Game Manager to be instantiated
        if(GameManager.instance == null)
        {
            Debug.LogError("GameManager Instance was not loaded before the Scene Manager");
            // Disable this script
            StopAllCoroutines();
            yield return null;
            this.enabled = false;
     
        }
        /// this will ultimately be rebuilt using Load parameters to tell a new Grid Setup what size it should be and where to start. 
        //Error logging for missing Gridsetup
        if (gridSetup == null)
        {
            gridSetup = new GridSetup();
        }
        //Set up the Grid
        gridSetup.BuildGrid(rows: 10, cols: 10, cellSize: 1f, willDrawGrid: willDrawGrid);
        //Get the Obstacles from the Obstacle manager and add them to the Grid
        gridSetup.AddObjects(obstacleManager.GetSceneObstacles());
        //Tell the Game Manager that the Game is ready
        GameManager.instance.GridComplete();

    }
}
