using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// depreceated
/// </summary>
public class NPCMovement : MonoBehaviour
{
    //Inspector adjustable attributes
    [SerializeField] private Transform target;
    private Vector2 targetPos { get { return target.position; } }
    [Range(0.1f, 1f)] public float speed = 1;
    [Range(0.1f, 1f)] public float stoppingDistance = .5f;
    LayerMask LayersBlockingMovement;

    //CALCULATED PROPERTIES

    private bool reachedTarget { get { return Vector2.Distance(transform.position, targetPos) < stoppingDistance; } }
    private Vector2 pos {get { return transform.position; } }

    //Game Mechanics
    private Rigidbody2D rb;
    private Collider2D npcCollider;

    //Path Calculation
    private List<Vector2> wayPoints = new List<Vector2>();

    private void Awake()
    {
        //rb = gameObject.GetComponent<Rigidbody2D>();
        npcCollider = gameObject.GetComponent<Collider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount < 4) return;
        while (!reachedTarget)
        {
            MoveToLocation();
            break;
        }
    }

    private void MoveToLocation()
    {
        var step = speed * Time.deltaTime;
        Vector2 moveDirection = Vector2.MoveTowards(transform.position, targetPos, step);
        transform.position = moveDirection;
    }
}
