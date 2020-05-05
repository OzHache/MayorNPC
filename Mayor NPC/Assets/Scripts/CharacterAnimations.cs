using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Required Components
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
//This class is designed to manage animaiton for all Characters
public class CharacterAnimations : MonoBehaviour
{
    //If this is the player
    private bool isPlayer;
    
    //player parameters
    private PlayerMovement pMovement;

    //enemy parameters
    private PathFindingMovement nPCMovement;

    //shared parameters
    private Vector2 directionOfTravel;
    private SpriteRenderer renderer;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        //establish if this is the player.
        isPlayer = gameObject.CompareTag("Player");
        if (isPlayer)
        {
            //If this is the player, check if there is a player movment component
            if (GetComponent<PlayerMovement>() == null)
            {
                //log warning for missing component
                Debug.LogWarning("This player is missing a Player movement Script", this.gameObject);
                this.enabled = false;
            }
            //otherwise set the reference
            else
            {
                pMovement = GetComponent<PlayerMovement>();
            }
        }else
        {
            if (GetComponent<PathFindingMovement>() == null)
            {
                //log warning for missing component
                Debug.LogWarning("This NPC is missing a PathFindingMovement Script", this.gameObject);
                this.enabled = false;
            }
            //otherwise set the reference
            else
            {
                nPCMovement = GetComponent<PathFindingMovement>();
            }
        }
        //check for other references needed
        if (GetComponent<Animator>() == null || GetComponent<SpriteRenderer>() == null)
        {
            Debug.LogWarning("This character is missing either an Animator component or a Sprite Renderer", this.gameObject);
        }
        else
        {
            animator = GetComponent<Animator>();
            renderer = GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Determine the direction of travel from the exposed direction property from the gameObjects movement script
        //If its a player
        if (isPlayer)
        {
            var x = 0;
            if (pMovement.direction.x > 0) x = 1;
            else if (pMovement.direction.x < 0) x = -1;

            var y = 0;
            if (pMovement.direction.y > 0) y = 1;
            else if (pMovement.direction.y < 0) y = -1;
            //set the direction of travel
            directionOfTravel = new Vector2(x, y);

        }
        //If it is an NPC
        else
        {
            var x = 0;
            if (nPCMovement.directionOfTravel.x > 0) x = 1;
            else if (nPCMovement.directionOfTravel.x < 0) x = -1;

            var y = 0;
            if (nPCMovement.directionOfTravel.y > 0) y = 1;
            else if (nPCMovement.directionOfTravel.y < 0) y = -1;

            directionOfTravel = new Vector2(x, y);
        }

        UpdateAnimatorParameters();
    }

    //Setting Animation Parameters
    private void UpdateAnimatorParameters()
    {

        //set moving
        animator.SetBool("Moving", directionOfTravel != Vector2.zero);
        //Flip X if needed
        renderer.flipX = directionOfTravel.x < 0;
        

    }
}
