using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Vector2 gridSize;
    //Instance Reference
    public static GameManager instance;

    //Events for GameObjects to listen to to wait for the Grid to be completed
    public event Action onGridComplete;
    //Events for GameObjects to listen for the end of the day
    public event Action onEndOfDay;
    //Event for Time updates based on whole Seconds
    public event Action onSecond;

    //day lenght
    public int dayInMinutes = 12;

    //Sleep timer
    public float sleeptimer = 5f;
    //In Game Clock
    public float gameTime { get; private set; }

    //isSleeping
    private bool isSleeping = false;




    //Bool for if the grid is set up and all the obstacles have been assigned. 
    public bool isGridComplete;
    // Start is called before the first frame update
    void Awake()
    {
        //establish this a the static Instance
        instance = this;
        StartCoroutine(DayCycle());
    }

    //enumerator that manages Day Cycles
    IEnumerator DayCycle()
    {
        gameTime = 0;
            

        while(gameTime / 60f < dayInMinutes)
        {
            yield return null;
            //check for a Seconds update
            if(Mathf.Floor(gameTime) < Mathf.Floor(gameTime+ Time.deltaTime)){
                //Trigger seconds
                onSecond();
            }
            //Each frame add time to the game time
            gameTime += Time.deltaTime;
            //If the space is pressed in testing, the game will advance a day
            if (Input.GetKeyDown(KeyCode.Space))
                gameTime = dayInMinutes *60;

        }
        //Start sleep
        StartCoroutine(Sleep());
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
    public float GetTime()
    {
        //calculate what hour the game should be in
        return gameTime;

    }

    private void Update()
    {
    }
    IEnumerator Sleep()
    {
        //set is sleeping to true
        isSleeping = true;
        // wait for the end of th night
        yield return new WaitForSeconds(sleeptimer);
        //set is sleeping to false
        isSleeping = false;
        //Trigger then end of the day
        onEndOfDay();
        //Start the Day Coroutine
        StartCoroutine(DayCycle());
        Debug.Log("Slept");
    }

}
