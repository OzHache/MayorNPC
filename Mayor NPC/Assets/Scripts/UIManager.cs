using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// UI Manager will Manage the UI Elements for the Game Manager
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //References to all UI that needs to be updated
    //Day
    [SerializeField]
    TextMeshProUGUI dayText;
    //int for days
    private int day = 0;
    //Hour
    [SerializeField]
    TextMeshProUGUI timeText;

    private void Start()
    {
        GameManager.instance.onSecond += TimeUpdate;
        GameManager.instance.onEndOfDay += OnEndOfDay;
        OnEndOfDay();

    }
    
    private void TimeUpdate()
    {
        //Set up a Format for leading zeros
        string fmt = "00.##";
        //Raw Time in seconds
        var seconds = GameManager.instance.gameTime;
        //Lowest whole number of seconds is the Game "Minutes"
        var gameMinutes = Mathf.Floor(seconds);
        //Hours is the whole number of game minutes / 60
        var hours = Mathf.Floor( gameMinutes / 60);
        //Minutes is the Modulus of gameMinutes % 60
        var minutes = gameMinutes % 60;
        timeText.text = hours.ToString(fmt) + " : " + minutes.ToString(fmt);
    }

    void OnEndOfDay()
    {
        //increase the Day
        day++;
        //Update the UI
        dayText.text = "Day: " + day.ToString();
    }


}
