using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Root behaviour for pathfinding on the grid system.
public class PathFindingMovement : MonoBehaviour
{
    public float speed = 5f;

    //Exposed Variable for handeling Animation
    [HideInInspector]public Vector2 directionOfTravel { get; private set; }        

    private int currentPathIndex;
    private List<Vector3> pathVectorList = new List<Vector3>();
    private Vector3 position { get { return transform.position; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //pass in the position based on the type of NPC this is.
    public bool SetTargetPosition (Vector3 targetPosition)
    {
        currentPathIndex = 0;
        pathVectorList = PathFinding.Instance.FindPath(position, targetPosition);

        if(pathVectorList != null  && pathVectorList.Count > 1)
        {
            //we don't need to move to the start position, thats the position we are on.
            pathVectorList.RemoveAt(0);
        }
        return pathVectorList != null;
    }
    public bool validPathExist(Vector3 targetPosition)
    {
       return  PathFinding.Instance.FindPath(position, targetPosition) == null;
    }


    /// <summary>
    /// Handles giving rigidbody velocity while I have not reached my destination, 
    /// needs to be called every fixedupdate time.
    /// </summary>
    /// <returns>True if still moving, false if stopped</returns>

    public bool HandleMovement()
        //are we setting a new target, do we have speed set above 0. 

    {
        if(pathVectorList != null && currentPathIndex != pathVectorList.Count - 1)
        {
            //Target position  is set to the current index position of the path Vector list.
            Vector3 targetPosition = pathVectorList[currentPathIndex];

            // if we are further than .25 units away keep moving that way
            if (Vector3.Distance(position, targetPosition) > .01f )
            {
                //Set the direction to be facing the target position from the transform position (normalized)
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                //float distanceBefore = Vector3.Distance(position, targetPosition);
                
                //move the Transform in the move dir, at speed * deltaTime.
                transform.position += (moveDir * (speed * Time.deltaTime));

                //Public expose for animation
                directionOfTravel = (Vector2)moveDir;
            }
            else
            {
                //Current Path Index iterates up so that the index moves up to the next path node position
                currentPathIndex++;
                //If we have reached the end of the index
                if(currentPathIndex >= pathVectorList.Count-1)
                {
                    //Trigger stop moving which nulls the pathvector list
                    StopMoving();
                    //Expose Vector2.zero to the animation
                    directionOfTravel = Vector2.zero;
                }
            }
            //still moving
            Debug.Log(targetPosition + "This is where I am moving to.", this.gameObject);
            return true;
        }
        else
        //after the path has reached the end and there path vectors list is nulled by StopMoveing
        {
            //set animation to Vector3.zero for animation purposes
            directionOfTravel = Vector2.zero;
            //tell the NPC Manager that we have stopped moving.
            return false;
        }
    }
    private void StopMoving()
    {
        pathVectorList = null;
    }
}
