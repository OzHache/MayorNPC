using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Debug with grid")]
    [SerializeField]
    private bool willDrawGrid;
    //Grid size Variables
    int rows = 100;
    int cols = 100;
    //References
    private GridSetup gridSetup;
    private ObstacleManager obstacleManager;


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
        //Create the GridSetup
        gridSetup = new GridSetup();

        //Set up the Grid
        gridSetup.BuildGrid(rows: 100, cols: 100, cellSize: 1f, willDrawGrid: willDrawGrid);
        //Get the Obstacles from the Obstacle manager and add them to the Grid
        gridSetup.AddObjects(obstacleManager.GetSceneObstacles(gridSetup.GetSize));
        //Tell the Game Manager that the Game is ready
        GameManager.instance.GridComplete();
        GameManager.instance.gridSize = new Vector2(rows, cols);

    }
}
