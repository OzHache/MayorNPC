using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Used for Tracking an NPC's memory as a listable item with Discovery location, time, Type of POI and If this is the player
/// </summary>
class MemoryObject
{
    //The game object that this memory represents 
    public GameObject gameObject { get; private set; }
    //Discovery Location
    public Vector2 discoveryLocation { get; private set; }
    private DateTime discoveryTime;
    //time since last discovered
    public float timeSinceLastDiscovered { get { return (float)(DateTime.Now - discoveryTime).TotalSeconds; } }
    //Is player
    public bool isPlayer { get; private set; }
    //what type of POI this is
    public POI poiType { get; private set; }
    //Get or set the interst level, increases if the object is not where memory placed it
    private float _interestLevel;
    public float interestLevel
    {
        get
        {
            if ((Vector2)gameObject.transform.position != discoveryLocation)
                //if it is not where it is supposed to be, Incease current interest
                return _interestLevel + 10;
            else
                return _interestLevel;
        }
    }

    public MemoryObject(GameObject gameObject, float interestLevel)
    {
        //Set refences
        this.gameObject = gameObject;
        this.discoveryLocation = gameObject.transform.position;
        this.discoveryTime = DateTime.Now;
        this._interestLevel = interestLevel;
        //Set if this is the player
        isPlayer = gameObject.CompareTag("Player");

        //Set POI type if this is a POI
        poiType = gameObject.GetComponent<Obstacle>() ? gameObject.GetComponent<Obstacle>().poi : POI.None; 
    }

}
