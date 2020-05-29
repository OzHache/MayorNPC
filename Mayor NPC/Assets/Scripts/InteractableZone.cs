using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// Creates a Box collider around the player that that highlights interactable objects
/// </summary>
public class InteractableZone : MonoBehaviour
{
    //Interactable Range
    public int range;
    //The box collider on this object
    private BoxCollider2D box;

    [SerializeField]
    private GameObject interactableArrow;
    private void Start()
    {
        //Add the box collider2D
        box = gameObject.AddComponent<BoxCollider2D>();
        //Resize to range / local scale
        box.size = new Vector2(range/ transform.localScale.x, range / transform.localScale.x);
        box.isTrigger = true;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if this object already has an interactable arrow
        if (collision.gameObject.GetComponent<InteractableArrow>() || collision.gameObject.GetComponent<ArrowReference>())
        {
            return;
        }
        //otherwise, is it interactable
        if (collision.gameObject.CompareTag("Interactable"))
        {
            var arrow = Instantiate(interactableArrow, collision.transform);
            arrow.SendMessage("Activate", collision.gameObject);
            var reference = collision.gameObject.AddComponent<ArrowReference>();
            reference.ArrowObject = arrow;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(Vector2.Distance(transform.position, other.transform.position) > 1f)
                other.gameObject.SendMessage("RemoveArrow", SendMessageOptions.DontRequireReceiver);
    }
}
