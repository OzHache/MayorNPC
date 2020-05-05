using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Instance Reference

    /*
     * Awake
     * OnEnable
     * Start
     */
    public static GameManager Instance;  //TODO : remember to always have somethign retreived
    
    //Bool for if the grid is set up and all the obstacles have been assigned. 
    private bool _isGridComplete = false;
    //public getter
    public bool isGridComplete { get { return _isGridComplete; } }
    // Start is called before the first frame update
    void Awake()
    {
        //establish this a the static Instance
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //External call from the Obstacle manager to tell the game manager that the grid has all the obstacles and is complete. 
    public void GridComplete( bool completed)
    {
        // set _isGridComplete to completed
        _isGridComplete = completed;
    }
}
