using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingMovement : MonoBehaviour
{
    public float speed = 5f;

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
        HandleMovement();
    }
    //pass in the position based on the type of NPC this is.
    public void SetTargetPosition (Vector3 targetPosition)
    {
        currentPathIndex = 0;
        pathVectorList = PathFinding.Instance.FindPath(position, targetPosition);

        if(pathVectorList != null  && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }
    }

    private void HandleMovement()
    {
        if(pathVectorList != null)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(position, targetPosition) > 1f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(position, targetPosition);
                //move movedir gives the direction of travel for animation
                transform.position = position + moveDir * speed * Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
                if(currentPathIndex >= pathVectorList.Count)
                {
                    StopMoving();
                    //set animation to Vector3.zero for animation purposes
                }
            }
        }
        else
        {
            //set animation to Vector3.zero for animation purposes
        }
    }
    private void StopMoving()
    {
        pathVectorList = null;
    }
}
