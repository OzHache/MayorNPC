using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Instance Reference
    public static GameManager instance;

    //Events for GameObjects to listen to to wait for the Grid to be completed
    public event Action onGridComplete;



    //Bool for if the grid is set up and all the obstacles have been assigned. 
    public bool isGridComplete;
    // Start is called before the first frame update
    void Awake()
    {
        //establish this a the static Instance
        instance = this;
    }

    //Trigger for the Event that the Grid is set up
    public void GridComplete()
    {
        //As long as there is a listener trigger this
        if(onGridComplete != null)
        {
            isGridComplete = true;
            onGridComplete();
        }
    }
}
