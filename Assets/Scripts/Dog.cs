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

    public DogModel Data { get; set; }

    public Dog(DogModel dog)
    {
        Data = dog;
        id = dog.Id;
        name = dog.Name;
        thumbnail = GameManager.Instance.FindDogSpriteByName(name);
        checkpointsFinished = dog.CheckpointsDone;
        checkpointGoal = dog.CheckpointsGoal;
        isNew = false;
        leaderboards_opt_in = dog.LeaderboardsOptIn;
        country = dog.Country;
        cityIndex = 0;
    }

    

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

    internal void Initialize(DogConfig dogConfig)
    {
        id = dogConfig.id;
        name = dogConfig.name;
        thumbnail = dogConfig.thumbnail;
    }

    public void SetDog(Dog dog)
    {
        id = dog.id;
        name = dog.name;
        thumbnail = dog.thumbnail;
        checkpointsFinished = dog.checkpointsFinished;
        checkpointGoal = dog.checkpointGoal;
        isNew = dog.isNew;
        leaderboards_opt_in = dog.leaderboards_opt_in;
        country = dog.country;
        cityIndex = dog.cityIndex;
    }

}
