using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //step speed in grid squares per second;
    public float speed = 2f;
    //map Size
    private float mapX = 0f;
    private float mapY = 0f;

    //Movement Limitations
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        mapX = GameManager.instance.gridSize.x;
        mapY = GameManager.instance.gridSize.y;
        var vertical = Camera.main.orthographicSize;
        var horizontal = vertical * Screen.width / Screen.height;

        //Set limits
        minX = horizontal;
        maxX = mapX - horizontal;

        minY = vertical;
        maxY = mapY - vertical;
        //Find the player
        if(GameObject.FindGameObjectWithTag("Player") == null)
        {
            //if there is no player tagged, alert and disbale this component
            Debug.LogError("There is no GameObject with the Tag Player", gameObject);
            enabled = false;
            return;
        }
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    private void LateUpdate()
    {
        //Update after all other movemnt updates
        var MoveTo = Vector3.Lerp(transform.position, player.position, Time.deltaTime * speed);
        MoveTo.z = transform.position.z;

        //clamp movement
        MoveTo.x = Mathf.Clamp(MoveTo.x, minX, maxX);
        MoveTo.y = Mathf.Clamp(MoveTo.y, minY, maxY);
        transform.position = MoveTo;
            
    }
}
