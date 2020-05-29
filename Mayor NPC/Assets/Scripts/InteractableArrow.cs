using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class InteractableArrow : MonoBehaviour
{
    //Reference to Animator
    public Animator animator;
    //Reference to GameObject this represents
    public GameObject interactableObject;

    public void Activate(GameObject interactableObject)
    {
        animator = GetComponent<Animator>();
        animator.enabled = true;
        this.interactableObject = interactableObject;
    }
    private void OnMouseEnter()
    {
        animator.enabled = false;
    }

    private void OnMouseDown()
    {
        interactableObject.SendMessage("Work", 1f, SendMessageOptions.DontRequireReceiver);
        Debug.Log("Working on" + interactableObject.name);
    }
    private void OnMouseExit()
    {
        animator.enabled = true;
    }

}
