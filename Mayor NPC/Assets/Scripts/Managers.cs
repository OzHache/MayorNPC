using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Ensure all the required managers are loaded to this GameObject

[RequireComponent(typeof(GameManager))]
[RequireComponent(typeof(ObstacleManager))]
[RequireComponent(typeof(SceneLoader))]
public class Managers : MonoBehaviour
{
   
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
