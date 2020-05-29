﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General Superclass for all jobs
/// </summary>
public class Occupation : MonoBehaviour
{
    //What This NPC Does
    public enum Job { Unemployed, Farmer}
    public Job job { get; private set; }

    //Status of work
    public enum Status { none, Started, Working, Complete}
    public Status workStatus = Status.none;

    public bool isComplete { get { return workStatus == Status.Complete; } }
    //percentatge complete
    private float percentComplete = 0f;
    //Job location
    public Vector2 jobLocation { get; private set; }
    public Vector2 home { get; private set; }



    //Set up this class
    public Occupation(Job job, Vector2 jobLocation, Vector2 home)
    {
        this.job = job;
        this.jobLocation = jobLocation;
        this.home = home;

    }
    //reference to start working
    public void StartWork()
    {
        workStatus = Status.Started;
    }

    //Update Work progress
    public void updateProgress(float workPercent)
    {
        percentComplete = Mathf.Clamp(percentComplete + workPercent, 0f, 100f);
        if (percentComplete == 100f)
        {
            workStatus = Status.Complete;
        }
        else if (percentComplete > 0)
        {
            workStatus = Status.Working;
        }
    }
    
}
