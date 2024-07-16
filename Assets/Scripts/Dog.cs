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
    public int checkpointsDone;
    public int checkpointsGoal;
    public bool isNew;
    public bool leaderboards_opt_in;
    public string country;

    public DogModel Data { get; set; }    

    public void IsWalking()
    {
        isNew = false;
    }

    internal void SetCheckpoints(TextMeshProUGUI checkpointGoalText)
    {
        checkpointsGoal = Int32.Parse(checkpointGoalText.text);
    }

    public void checkpoint()
    {
        checkpointsDone += 1;
        Data.CheckpointsDone += 1;
    }

    internal void Initialize(DogConfig dogConfig)
    {
        id = dogConfig.id;
        name = dogConfig.name;
        thumbnail = dogConfig.thumbnail;
        Data = new DogModel();
        Data.Id = id;
        Data.Name = name;
    }

    public void SetDog(Dog dog)
    {
        Data = dog.Data;
        id = dog.id;
        name = dog.name;
        thumbnail = dog.thumbnail;
        checkpointsDone = dog.checkpointsDone;
        checkpointsGoal = dog.checkpointsGoal;
        isNew = dog.isNew;
        leaderboards_opt_in = dog.leaderboards_opt_in;
        country = dog.country;
    }

    internal void InitializeFromModel(DogModel dogModel)
    {
        Data = dogModel;
        id = dogModel.Id;
        name = dogModel.Name;
        thumbnail = GameManager.Instance.FindDogSpriteByName(name);
        checkpointsDone = dogModel.CheckpointsDone;
        checkpointsGoal = dogModel.CheckpointsGoal;
        isNew = false;
        leaderboards_opt_in = dogModel.LeaderboardsOptIn;
        country = dogModel.Country;
    }
}
