using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField]private Transform target;
    private Vector2 targetPos { get { return target.position; } }
    [Range(0.1f, 1f)] public float speed = 1;
    [Range(0.1f, 1f)] public float stoppingDistance =.5f;

    //CALCULATED PROPERTIES
    
    private bool reachedTarget { get { return Vector2.Distance(transform.position, targetPos) < stoppingDistance; } }
    
   


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        while (!reachedTarget)
        {
            MoveToLocation();
            break;
        }
    }

    private void MoveToLocation()
    {xs
        var step = speed * Time.deltaTime;
        Vector2 moveDirection = Vector2.MoveTowards(transform.position, targetPos, step);
        transform.position = moveDirection;
    }

  
}
