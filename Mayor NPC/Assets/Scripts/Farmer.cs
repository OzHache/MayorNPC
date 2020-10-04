using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : Occupation
{
    //Tracking movement to trigger events
    private bool isMoving = false;


    private void Start()
    {
        //Set Job
        job = Job.Farmer;
        //Set work time
        workTime = 1.5f;
        //Set the Animator to work
        //TODO: Set the Animator to work
    }
    public
    //reference to the place I work
    Transform workPlace;

    public WorkCell currentCell { get; private set; }

    public void Work()
    {
        //If the workplace is not set, see if you can find one
        if (workPlace == null)
        {
            var zones = FindObjectsOfType<WorkZone>();
            foreach (WorkZone zone in zones)
            {
                if (zone.Worker == null && zone.job == job)
                {
                    zone.Worker = this;
                    init(job, zone.entrance, home == null ? (Vector2)transform.position : home, zone);
                    workPlace = zone.gameObject.transform;
                    break;
                }
            }

        }
        if (workPlace != null && (Vector2)transform.position != (Vector2)workPlace.position)
        {
            gameObject.SendMessage("MoveTo", (Vector2)workPlace.position);
            gameObject.GetComponent<NPCManager>().FinishedMoving += StopMoving;
            currentCell = workZone.GetWork();
            StartCoroutine(Working());
            
        }
        else if (workPlace != null)
        {
            //if I am at the entrance, start my work by getting the first workcell
            currentCell = workZone.GetWork();
            StartCoroutine(Working());
        }


    }
    IEnumerator Working()
    {
        //if there is no move work to do restart AI. 
        if(currentCell == null)
        {
            workStatus = Status.Complete; 
            gameObject.GetComponent<NPCManager>().StartAI();
            yield break;
        }
        else
        {
            //wait to be told that we have arrived at the work location
            isMoving = true;
            
            while (isMoving)
            {
                yield return null;
            }
            workStatus = Status.Working;
        }
        while (currentCell != null)
        {
            //move to the work location
            gameObject.SendMessage("MoveTo", currentCell.position);
            //Set moving to true
            isMoving = true;
            //Start listening for a stop movement
            gameObject.GetComponent<NPCManager>().FinishedMoving += StopMoving;
            while (isMoving)
            {
                yield return null;
            }
            //Now that you have arrived. Work on the cell
            currentCell.Work();
            yield return new WaitForSeconds(workTime);
            currentCell = workZone.GetWork();
        }
        workStatus = Status.Complete;
        gameObject.GetComponent<NPCManager>().StartAI();
        yield break;
    }

    void StopMoving()
    {
        // set moving to true
        isMoving = false;
        // Stop listening for movement
        gameObject.GetComponent<NPCManager>().FinishedMoving -= StopMoving;
    }



}
