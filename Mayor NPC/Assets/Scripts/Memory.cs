using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Memory
{ 
    private List<MemoryObject> memoryObjects = new List<MemoryObject>();

    
    /// <summary>
    /// Adds objects to the memory
    /// </summary>
    /// <param name="gameObject"> Game Object to add to memory</param>
    /// <param name="interstLevel">Interest level of this object, Defaults to 1</param>
    public void Add(GameObject gameObject, float interstLevel = 1)
    {
        memoryObjects.Add(new MemoryObject(gameObject, interstLevel));
    }

    /// <summary>
    /// Check if this game object is in the memory
    /// </summary>
    /// <param name="gameObject">Object to check</param>
    /// <returns>Returns true if this object exist in memory </returns>
    public bool Contains(GameObject gameObject)
    {
        foreach (MemoryObject mo in memoryObjects)
        {
            if (mo.gameObject == gameObject)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Overload to get the Index of the memory
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="i"> out index of the memory in the memory list</param>
    /// <returns> True if the Object is in memory</returns>
    internal bool Contains(GameObject obj, out  int i)
    {
        foreach (MemoryObject mo in memoryObjects)
        {
            if (mo.gameObject == obj)
            {
                i = memoryObjects.IndexOf(mo);
                return true;
            }
        }
        i = -1;
        return false;
    }
    
    /// <summary>
    /// Get the memory of this gameObject
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns>Returns the memory of this gameobject</returns>
    public MemoryObject GetMemoryOf(GameObject gameObject)
    {
        int index;
        //If memory contains this object
        if (Contains(gameObject, out index))
        {
            return memoryObjects[index];
        }
        //otherwise, I have no memory of this place.
        else return null;
    }


}
