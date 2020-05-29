using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class WorkZone : MonoBehaviour
{
    //The worker assigned to this Task
    public Occupation Worker;
    public Occupation.Job job;
    //this will hold the crop game object;
    public GameObject dependant;
    //WorkSpace
    public int xSize;
    public int ySize;
    //Entrance
    [Tooltip("Cell just outside the work area that the character will use to enter, Distance from top left")]
    public Vector2 entrance;

    public WorkCell TypeOfWork;
   
    //List of WorkCells
    private List<WorkCell> workCells = new List<WorkCell>();
    //List of gameObjects that represent the WorkCells
    public List<GameObject> workCellGameObjects = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //If this object does not have a WorkCells Child, Send a warning and disable the Script
        if (!gameObject.transform.Find("WorkCells"))
        {
            Debug.LogError(gameObject.name + " Requires a \"WorkCells\" Child ", gameObject);
            this.enabled = false;
            return;
        }
        foreach(Transform child in transform.Find("WorkCells"))
        {
            workCellGameObjects.Add(child.gameObject);
        }
        //Align to grid
        AlignWithGrid();
        SortWorkCells();
        SetUpWorkCells();
    }

    private void SortWorkCells()
    {

        //Bubble sort
        for (var p = 0; p < workCellGameObjects.Count-1; p++)
        {
            for (var i = 0; i < workCellGameObjects.Count-1; i++)
            {
                bool swap = false;
                GameObject temp;
                //if this Gameobject X is less than the next, swap them
                if (workCellGameObjects[i].transform.position.x > workCellGameObjects[i + 1].transform.position.x)
                {
                    swap = true;
                }
                //If x is the same, and y is more swap them
                if(workCellGameObjects[i].transform.position.x == workCellGameObjects[i + 1].transform.position.x && workCellGameObjects[i].transform.position.y < workCellGameObjects[i + 1].transform.position.y)
                {
                    swap = true;
                }
                if (swap)
                {
                    // Temp the next item
                    temp = workCellGameObjects[i+1];
                    // Set next to this item
                    workCellGameObjects[i+1] = workCellGameObjects[i];
                    // Set this to temp
                    workCellGameObjects[i] = temp;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// Used to ensure that the WorkZone is aligned with the Global grid
    /// </summary>
    void AlignWithGrid()
    {
        //Remove any alignment issues
        Vector3 pos = transform.position;
        pos.x = Mathf.Floor(pos.x) + .5f;
        pos.y = Mathf.Floor(pos.y) + .5f;
        transform.position = pos;
    }

    /// <summary>
    /// Create a Work object to represent each workzone
    /// </summary>
    void SetUpWorkCells()
    {
        //Iterate on x (Cols)
        for(var x = 0; x <  xSize; x++)
        {
            //Iterate on y (Rows)
            for (var y = 0; y < ySize; y++)
            {
                //Create a new workcell on the GameObject

                var newWorkCell = workCellGameObjects[(x * ySize) + y].AddComponent<WorkCell>();
                workCells.Add(newWorkCell);
            }
        }
    }
}
