using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General Workcell
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class WorkCell : MonoBehaviour
{
    public enum WorkCellState { dry, watered, growing}
    private WorkCellState state = WorkCellState.dry;
    public Vector2 position { get; private set; }
    public GameObject obj { get; private set; }
    public GameObject dependant;
    public float percentWorkComplete { get; private set; }
    private float percentPerWorkAction = 10f;
    public bool needsWork { get { return state == WorkCellState.dry; } }
    public bool isComplete { get { return percentWorkComplete == 100f; } }

    /// <summary>
    /// Add Work to the Cell
    /// </summary>
    /// <param name="modifer">Any Adjustments needed to the work performance, Lower is slower</param>
    /// <returns>Returns true if this cell is finished working</returns>
    public bool Work(float modifer = 1)
    {
        switch (state)
        {
            case WorkCellState.dry:
                //if there is a spriteRenderer
                state = WorkCellState.watered;
                if (obj.GetComponent<SpriteRenderer>())
                {
                    StartCoroutine(SpriteUpdater());
                    state = dependant != null ? WorkCellState.growing : WorkCellState.watered;
                    state = WorkCellState.watered;
                    obj.tag = "Untagged";
                }
                break;

        }
        //Add to percentWorkComplete with modifier
        percentWorkComplete = Mathf.Clamp(percentWorkComplete + (percentPerWorkAction * modifer), 0f, 100f);
        //Tell the dependant that the work has been activated
        if(dependant != null)
            dependant.SendMessage("Activate", SendMessageOptions.DontRequireReceiver);
        //Get rid of the Arrow
        gameObject.SendMessage("RemoveArrow", SendMessageOptions.DontRequireReceiver);
        return isComplete;
        
    }

    IEnumerator SpriteUpdater()
    {
        //Get the sprite renderer
        SpriteRenderer ren = obj.GetComponent<SpriteRenderer>();
        switch (state)
        {
            //On dry, reset this to Dry
            case WorkCellState.dry:
                ren.color = Color.white;
                break;
            case WorkCellState.watered:
            case WorkCellState.growing:
                //transition to wet
                Color colorToLerpTo = new Color(0.6f, 0.5f, 0.3f, 1);
                for (float time = 0f; time < 3; time += Time.deltaTime)
                {
                    Color newColor = Color.Lerp(Color.white, colorToLerpTo, time / 3);
                    ren.color = newColor;
                    yield return null;
                }
                break;

        }
        yield return null;
    }

    private void Start()
    {
        //set the collider to istrigger
        gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        gameObject.tag = "Interactable";
        this.position = gameObject.transform.position;
        this.obj = gameObject;
        percentWorkComplete = 0;
        this.percentPerWorkAction = 10;
        GameManager.instance.onEndOfDay += NewDay;
    }

    internal void NewDay()
    {
        //cycles from dry to watered, work will check 
        switch (state)
        {
            case WorkCellState.dry:
                if(dependant != null)
                {
                    //Penalty for not watering
                    percentPerWorkAction *= .9f;
                    //Send a message to the dependant
                    dependant.SendMessage("Updater", WorkCellState.dry, SendMessageOptions.DontRequireReceiver);
                }
                break;
            case WorkCellState.watered:
                if (dependant != null)
                {
                    //allow the work added to this object to increase by 10 percent up to 12%
                    percentPerWorkAction = Mathf.Clamp(percentPerWorkAction *= 1.1f, 0, 12f);
                    //Send a message to the dependant
                    dependant.SendMessage("Updater", percentPerWorkAction, SendMessageOptions.DontRequireReceiver);
                    // set the sate to growing
                    state = WorkCellState.growing;
                }
                else { state = WorkCellState.dry; }
                break;
        }
        gameObject.tag = "Interactable";
        StartCoroutine(SpriteUpdater());

    }
}
