using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Net;
using System;
using Random = UnityEngine.Random;
using System.Runtime.InteropServices;
/// <summary>
/// The NPC Manager is an inheritable class that manages root AI behaviour for all NPCs
/// Types of NPCs will include: Citizens, Raiders, Patrolers, Boss
/// </summary>
[RequireComponent(typeof(PathFindingMovement))]
[RequireComponent(typeof(LineOfSight))]
[RequireComponent(typeof(Occupation))]
public class NPCManager : MonoBehaviour
{
    #region Variables
    //Subscribeable Events
    public event Action FinishedMoving;

    //enum for npcStates
    protected private enum NPCState { Idle, Patrol, Attack, Ranged, Work, Home, Testing, Investigating}
    //Current NPC State
    protected private NPCState currentState;
    public bool istesting = false;
    //Interest radius for looking for new POI's based on position
    [Range(0,40)]
    public float interestRadius;
    //testing Parameters
    [Tooltip("For Testing, assign target")]
    public Transform testingTarget;

    //Target of navigation
    private Transform target;

    //targetPos is assigned based on if this is a test or not.
    private Vector3 targetPos;

    //recall variable for ensuring the next target is not the current or last target. 
    private Vector3 lastTargetPos = -Vector3.one;
    //Refercence to the PathFinding Movement Script
    private PathFindingMovement PFM;
    //Reference to the LOS 
    private LineOfSight LOS;
    //Reference to the occupation
    private Occupation occupation;
    //Memory for POIs 
    Memory memory = new Memory();

    //Interesting Item I am Ivestigating
    InterestItem interestItem = null;

    #endregion Variables
    // Start is called before the first frame update
    private void Awake()
    {
        //
    }
    void Start()
    {
        //if testing, set state to testing
        if (istesting)
        {
            currentState = NPCState.Testing;
            targetPos = testingTarget.position;
        }
        else
        {
            currentState = NPCState.Idle;
        }

        //Check if the required components are available;
        if(GetComponent<PathFindingMovement>() == null)
        {
            //if there is not Path finding movement script, throw a warning and disable the script
            Debug.LogWarning("This NPC needs a Path Finding Movement Script component " + this.gameObject.name, this.gameObject);
        }
        LOS = gameObject.GetComponent<LineOfSight>();

        //assign the pathfindingMovement component
        PFM = gameObject.GetComponent<PathFindingMovement>();

        //assign occupation
        occupation = gameObject.GetComponent<Occupation>();

        //Start listening for the Grid to be complete
        GameManager.instance.onGridComplete += StartAI;
    }
    public void StartAI()
    {
        //Start the NPCAI() coroutine
        StartCoroutine(NPCAI());
    }
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Color color = Color.yellow;
        color.a = .2f;
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, interestRadius);
    }
    IEnumerator NPCAI()
    {
        //wait for the grid system to be populated with the obstacles
        while (!GameManager.instance.isGridComplete)
        {
            Debug.Log(Time.frameCount + " Frames it takes to complete the grid");
            yield return null;
        }
        //Primary AI loop
        //switch over the AI State 

        //What should I do now
        GiveTask();

        switch (currentState)
        {
            //Cases  Idle, Patrol, Attack, Ranged, Work, Home, Testing
            //in the case of Testing
            case NPCState.Testing:
                //if there is no target Set, set the target to the testing target
                if(lastTargetPos != testingTarget.position)
                {
                    PFM.SetTargetPosition(testingTarget.position);
                    StartCoroutine(Moving());
                }
                break;
            //In the case of Idle, Look for something intersting and go there
            case NPCState.Idle:
                StartCoroutine(Idle());
                break;
            case NPCState.Investigating:
                StartCoroutine(Investigate());
                break;
            case NPCState.Work:
                gameObject.SendMessage("Work",SendMessageOptions.DontRequireReceiver);
                break;

                //In the case of Patrol(Guards and Enemies) patrol between all patrol points, if there is something intersting and I haven't investigated it yet and it is a random interval, go look at it. 
                //In the case of Attack, Go to my target and attack
                //In the case of Ranged, get close enough to my target and use a ranged attack
                //In the case of Work, go between my work POI and work on those items
                //In the case of Home, Go home

        }
            
        yield return null;
        
    }
    public void MoveTo(Vector2 location)
    {
        targetPos = location;
        //move to a location but do not restart the AI
        StartCoroutine(Moving(false));
    }
    //Manage Movement    
    IEnumerator Moving(bool restartAI = true)
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
        //Only restart AI if the AI should be restarted
        if(restartAI)
        StartCoroutine(NPCAI());
        //As long as there is a listener trigger this
        if (FinishedMoving != null)
        {
            FinishedMoving();
        }
    }
    // Manage Idle
    IEnumerator Idle()
    {
        ///Idle happens when I am not working, there is no place to work 
        ///I will either look for 

        //Idle in place for a few Seconds

        float idleTimer = Random.Range(.5f, 3f);

        yield return new WaitForSeconds(idleTimer);

        //Get interest Points in my Radius
        List<Obstacle> POIs = ObstacleManager.GetObstacleManager.GetPOIs((Vector2)transform.position, interestRadius);
        //Only keep the POI's I can see
        POIs = LOS.CheckLOS(POIs);


        //if there is no POIs in the list find a valid location to wander to within 1/2 interest radius
        if (POIs.Count > 0)
        {

            //Create an array of POIs and thier interest level
            List<InterestItem> interestList = new List<InterestItem>();
            for (var i = 0; i < POIs.Count; i++)
            {
                //If I have no memory of this object
                if (!memory.Contains(POIs[i].gameObject))
                {
                    //Points for interest 10 - distance to the object in Integers
                    var interestPoints = (10 - (int)Mathf.Clamp(Mathf.FloorToInt(Vector2.Distance(POIs[i].position, transform.position)), 1, 10));
                    interestList.Add(new InterestItem(POIs[i].gameObject, interestPoints));
                }
                else
                {
                    //if it is in memory add it to my interest array use that interst value
                    MemoryObject memoryObject = memory.GetMemoryOf(POIs[i].gameObject);
                    interestList.Add(new InterestItem(memoryObject.gameObject, memoryObject.interestLevel));
                }
            }
            //Sort the list
            interestList.Sort();
            //Reverse so it is the highest first
            interestList.Reverse();

            Debug.Log("Ooo! Something Shiney: " + interestList[0].gameObject.name);
            interestItem = interestList[0];
            //Then Navigate to that gameObject with the highest interest
            targetPos = interestList[0].gameObject.transform.position;
            currentState = NPCState.Investigating;
            StartCoroutine(NPCAI());

        }
        else
        {
            //Otherwise, find a random location within half interest range and wander there.
            //Variables for my random location
            
            
            // direction to travel
            Vector2 direction;
            //Position to travel to
            Vector2 position;
            RaycastHit2D hit;
            //checkBool
            bool isValid = false;
            //Pick a random angle
            float z = Random.Range(0, 360);
            direction = (Vector2)(Quaternion.Euler(0, 0, z) * Vector2.right);
            //raycast
            hit = Physics2D.Raycast(transform.position, direction, interestRadius);
            Debug.DrawRay(transform.position, direction, Color.blue, 1f);
            yield return null;
            isValid = (hit.distance < 2 || !hit);

            while (!isValid) { 
                //Pick a random angle
                z = Random.Range(0, 360);
                direction = (Vector2)(Quaternion.Euler(0, 0, z) * Vector2.right);
                //raycast
                hit = Physics2D.Raycast(transform.position, direction, interestRadius);
                Debug.DrawRay(transform.position, direction, Color.blue, 1f);
                yield return null;
                isValid = (hit.distance < 2 || !hit);
                //if hit is null, we can move up to a randomDistance between 1 and interest Radius/2
            }
            //do this while the hit distance is less than 2 or until we don't hit anything
            
            if (!hit)
            {
                //pick a position along the ray
                position = (Vector2)transform.position + (direction * Random.Range(2, interestRadius / 2));
                //normalize position to whole numbers
                position.x = Mathf.Floor(position.x);
                position.y = Mathf.Floor(position.y);
            }
            else
            {
                //otherwise, find a position between hit and this object that is greater 2  and less than half the interest Radius
                position = (Vector2)transform.position + (direction * Random.Range(2, hit.distance -1f));
                position.x = Mathf.Floor(position.x) +.5f;
                position.y = Mathf.Floor(position.y) +.5f;
                
                Debug.DrawLine(transform.position, position, Color.red, 2f);
            }
            
            //Once I have a valid place to wander to, move
            targetPos = position;
            StartCoroutine(Moving());
            Debug.LogWarning("I have nothing to do", gameObject);
        }
    }

    IEnumerator Investigate()
    {
        //Start Moving towards the Object
        StartCoroutine(Moving());
        //Wait until I am done Investigating
        while (Vector2.Distance(transform.position, interestItem.gameObject.transform.position) > 1.5f)
        {
            yield return null;
        }
        //check if the item is in memory
        if (!memory.Contains(interestItem.gameObject))
        {
            memory.Add(interestItem.gameObject);
            Debug.Log("I know about this item now", interestItem.gameObject);
        }
        interestItem = null;
        currentState = NPCState.Idle;
    }

    //Determine what I should be doing at this time
    public void GiveTask()
    {
        //get the game time
        int hour = (int)(GameManager.instance.GetTime() / 60f);
        //based on the hour set work to task
        if(currentState == NPCState.Investigating)
        {
            return;
        }
        switch (hour)
        {
            //When it is between 0 and 3
            case int n when n < 3:
                //TODO: Come back and adjust this to Idle after confirming that it works
                currentState = NPCState.Work;
                if (occupation.isComplete)
                    currentState = NPCState.Idle;
                break;
            //When it is between 3 and 6
            case int n when n >= 3 && n <6:
                currentState = NPCState.Work;
                break;
            //When it is between 6 and 9
            case int n when n >= 6 && n < 9:
                //If I am done working idle
                if (occupation.isComplete)
                    currentState = NPCState.Idle;
                //Otherwise Work
                currentState = NPCState.Work;
                //if work is done 
                break;
            //When it is less than 12
            case int n when n < 12:
                currentState = NPCState.Home;
                break;
        }
    }
    internal class InterestItem : IEquatable<InterestItem>, IComparable<InterestItem>
    {
        internal GameObject gameObject = null;
        internal float interest = float.MinValue;
        public InterestItem( GameObject gameObject = null, float interest = float.MinValue)
        {
            this.gameObject = gameObject;
            this.interest = interest;
        }

        //Implement IComparable
        public int CompareTo(InterestItem obj)
        {
            if (obj == null) return 1;
            else return this.interest.CompareTo(obj.interest);
        }
        //Implement IEquatable
        public bool Equals(InterestItem other)
        {
            if (other == null)
                return false;
            else return this.interest == other.interest;
        }
    }
}
