using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
/// <summary>
/// The Obstacle class is an inheritable class that will send themselves to the Obstacle Manager to be added to the grid. 
/// The next step is to create a class that holds a POI value 
/// </summary>
public enum POI { None, Something}
public class Obstacle : MonoBehaviour
{
    //POI will dictate the category for the POI if this is one.
    public POI poi;
    //return true if this is not a POI.
    public bool isPOI { get { return poi != POI.None; } }

    //position in the world

    public Vector3 position { get { return transform.position; }}
   

   //On start, add me to the getObstacle Manager
    void Start()
    {
        //determin if my position is 
        var x = gameObject.transform.position;
    }
}
