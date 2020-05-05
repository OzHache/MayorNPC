using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The NPC Manager is an inheritable class that manages root AI behaviour for all NPCs
/// Types of NPCs will include: Citizens, Raiders, Patrolers, Boss
/// </summary>
[RequireComponent(typeof(PathFindingMovement))]
public class NPCManager : MonoBehaviour
{
    //enum for npcStates
    protected private enum NPCStates { Idle, Patrol, Attack, Ranged, Work, Home, Testing}
    //Current NPC State
    protected private NPCStates currentState;
    public bool istesting = false;
    [Tooltip("For Testing, assign target")]
    public Transform testingTarget;
    private Transform target;
    //targetPos is assigned based on if this is a test or not.
    private Vector3 targetPos { get { return currentState == NPCStates.Testing? testingTarget.position : target.position; } }
    //recall variable for ensuring the next target is not the current or last target. 
    private Vector3 lastTargetPos;
    //Refercence to the PathFinding Movement Script
    private PathFindingMovement PFM;
    // Start is called before the first frame update
    private void Awake()
    {
        
    }
    void Start()
    {
        //if testing, set state to testing
        if (istesting)
        {
            currentState = NPCStates.Testing;
        }
        else
        {
            currentState = NPCStates.Idle;
        }
        //Check if the required components are available;
        if(GetComponent<PathFindingMovement>() == null)
        {
            //if there is not Path finding movement script, throw a warning and disable the script
            Debug.LogWarning("This NPC needs a Path Finding Movement Script component " + this.gameObject.name, this.gameObject);
        }
        //assign the pathfindingMovement component
        PFM = gameObject.GetComponent<PathFindingMovement>();
        //Start the NPCAI() coroutine
        StartCoroutine(NPCAI());
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator NPCAI()
    {
        //wait for the grid system to be populated with the obstacles
        while (!GameManager.Instance.isGridComplete)
        {
            Debug.Log(Time.frameCount + " Frames it takes to complete the grid");
            yield return null;
        }
        //Primary AI loop
        int breaker = 0;
        for (; ; )
        {
            //switch over the AI State 

            switch (currentState)
            {
                //Cases  Idle, Patrol, Attack, Ranged, Work, Home, Testing
                //in the case of Testing
                case NPCStates.Testing:
                    //if there is no target Set, set the target to the testing target
                    if(lastTargetPos != testingTarget.position)
                    {
                        PFM.SetTargetPosition(testingTarget.position);
                        StartCoroutine(Moving());
                    }
                    break;
                    //In the case of Idle, Look for something intersting and go there
                    //In the case of Patrolc(Guards and Enemies) patrol between all patrol points, if there is something intersting and I haven't investigated it yet and it is a random interval, go look at it. 
                    //In the case of Attack, Go to my target and attack
                    //In the case of Ranged, get close enough to my target and use a ranged attack
                    //In the case of Work, go between my work POI and work on those items
                    //In the case of Home, Go home

            }
            
            yield return null;
        }
        Debug.Log("NPCAI stopped after " + breaker + " iterations");
    }
    IEnumerator Moving()
    {
        //set the destination
        PFM.SetTargetPosition(targetPos);
        bool ismoving = true;
        //while I have not stopped moving
        while (ismoving)
        {
            ismoving = PFM.HandleMovement();
            //movement is a not physics based movement so this coroutine needs to run on frames update
            yield return null;
            
        }
        //stop this coroutine when I have made it to my destination
        StopCoroutine(Moving());
    }
    
}
