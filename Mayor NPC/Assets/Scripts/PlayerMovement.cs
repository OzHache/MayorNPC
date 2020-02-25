using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Range(1, 10)] public float speed = 5;
    [Range(1, 10)] public float acceleration = 100;
    Vector2 direction = Vector2.zero;
    private Rigidbody2D rb;
    private bool moving { get { return direction != Vector2.zero; } }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetInput();
        if (moving)
        {
            AttemptMove();
        }
        else
        {
            SlowDown();
        }
    }

    private void SlowDown()
    {
        rb.velocity = Vector2.zero;
    }

    //Get basic movement details
    private void GetInput()
    {
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");

        //set direction to a full value
        direction = new Vector2(x, y).normalized;
    }

    //make an attempt to move
    private void AttemptMove()
    {

        rb.velocity = (direction * acceleration);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, speed);
    }
}
