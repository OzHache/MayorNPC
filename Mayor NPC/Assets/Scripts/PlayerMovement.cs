using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    //Movement Scalars
    [Range(1, 10)] public float speed = 5;
    [Range(1, 10)] public float acceleration = 100;

    //Publicly exposed direction for animation
    public Vector2 direction = Vector2.zero;
    //Refernce to the rigidbody
    private Rigidbody2D rb;
    //Calculated property to determine if we are moving based on the direction property not being zero
    private bool moving { get { return direction != Vector2.zero; } }
    // Start is called before the first frame update
    void Start()
    {
        //Check for required components
        if (GetComponent<Rigidbody2D>() == null)
        {
            Debug.LogWarning("The player needs a rigidbody to move", this.gameObject);
            this.enabled = false;
        }
        else
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //Get the input
        GetInput();
    }
    void FixedUpdate()
    {
        
        //If we hav input to move, then attempt to move, otherwise, slow down
        if (moving) AttemptMove();
        else SlowDown();
    }

    private void SlowDown()
    {
        //stop movement
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
        rb.velocity = direction * speed;
        //set the rigidody velocity to the direction times acceleration
        //rb.AddForce(direction * acceleration, ForceMode2D.Force);
        //Limit movement to max speed
        //rb.velocity = Vector2.ClampMagnitude(rb.velocity, speed);
    }
}
