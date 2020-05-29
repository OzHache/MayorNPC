using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This Class help to magange LOS 
/// </summary>
public class LineOfSight : MonoBehaviour
{
    [SerializeField]
    LayerMask BlockingLayerMask;
    public List<Obstacle> CheckLOS(List<Obstacle> POIList)
    {
        //arry to keep track of invalid POIs
        int[] removeAt = new int[POIList.Count];
        //Loop over all the POIs in the POIList 
        for (int i = 0; i< POIList.Count; i++) {
            var poi = POIList[i];
            // Raycast to the obstacle
            // TODO: this needs to be up
            RaycastHit2D hit = Physics2D.Raycast(transform.position, poi.position - transform.position);
            // if the object that I hit is not tagged POI, remove it from the list. 
            // This will remove any object that has a blocked line of sight
            if (!hit.transform.CompareTag("POI"))
            {
                removeAt[i] = 1;
            }
        }
        //remove the invalid POIs
        for (int i = removeAt.Length -1; i>= 0; i--)
        {
            if (removeAt[i] == 1)
            {
                POIList.RemoveAt(i);
            }
        }
        return POIList;
    }
    //overload of Check to see if you can see that position without something blocking your view
    public bool CheckLOS(Vector2 position, GameObject obj)
    {
        //Distance to the Object
        float distance = Vector2.Distance(position, transform.position);
        //Return true if the object Hit is the obj we want to see
        return Physics2D.Raycast(transform.position, position - (Vector2)transform.position, distance).collider.gameObject == obj;
    }
    //Overload to check to see if there is exacally nothing at the spot
    public bool CheckLOS(Vector2 position)
    {
        //Distance to the Object
        float distance = Vector2.Distance(position, transform.position);
        //Return true if the object Hit is the obj we want to see
        return Physics2D.Raycast(transform.position, position - (Vector2)transform.position, distance).transform == null;
    }
}
