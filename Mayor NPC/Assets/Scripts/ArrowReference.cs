using System;
using UnityEngine;
class ArrowReference:MonoBehaviour
{
    public GameObject ArrowObject;

    //Messager reciever from the Interactble zone leaving
    public void RemoveArrow()
    {
        Destroy(ArrowObject);
        Destroy(this);
    }
}

