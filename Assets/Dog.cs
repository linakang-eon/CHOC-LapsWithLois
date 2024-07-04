using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dog : MonoBehaviour
{
    public string id;
    public string name;
    public Sprite thumbnail;
    public int checkpointsFinished;
    public int checkpointGoal;
    public bool isNew;
    public bool leaderboards_opt_in;
    public string country;
    public int cityIndex;

    public void SetConfig(DogConfig dogConfig)
    {
        id = dogConfig.id;
        name = dogConfig.name;
        thumbnail = dogConfig.thumbnail;
    }

    public void IsWalking()
    {
        isNew = false;
    }

    internal void SetCheckpoints(TextMeshProUGUI checkpointGoalText)
    {
        checkpointGoal = Int32.Parse(checkpointGoalText.text);
    }

    public void checkpoint()
    {
        checkpointsFinished += 1;
        cityIndex += 1;
    }
}
