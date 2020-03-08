using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public Transform target;
    private Vector3 targetPos { get { return target.position; } }
    private Vector3 lastTargetPos;

    private PathFindingMovement PFM;
    // Start is called before the first frame update
    private void Awake()
    {
        PFM = gameObject.GetComponent<PathFindingMovement>();
    }
    void Start()
    {
        PFM.SetTargetPosition(targetPos);
        lastTargetPos = targetPos;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.frameCount %10== 0 && targetPos != lastTargetPos)
        {
            PFM.SetTargetPosition(targetPos);
            lastTargetPos = targetPos;
        }
    }
}
