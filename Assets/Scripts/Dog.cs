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
    public int cityIndex;

    public bool timerDone = false;

    public DogModel Data { get; set; }

    //public string currentDeviceUUID = "";

    public void IsWalking()
    {
        isNew = false;
    }

    internal void SetCheckpoints(TextMeshProUGUI checkpointGoalText)
    {
        checkpointsGoal = Int32.Parse(checkpointGoalText.text);
        //currentDeviceUUID = SystemInfo.deviceUniqueIdentifier;
    }

    public bool checkpoint()
    {
        checkpointsDone += 1;
        Data.CheckpointsDone += 1;
        cityIndex += 1;
        if (cityIndex == 5)
            cityIndex = 0;
        Data.CityIndex = cityIndex;
        //currentDeviceUUID = SystemInfo.deviceUniqueIdentifier;
        GameManager.Instance.addWalkingDog(this);
        return checkpointsDone == checkpointsGoal;
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
        cityIndex = dog.cityIndex;
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
        cityIndex = dogModel.CityIndex;
    }

    internal void initializeData()
    {
        Data = new DogModel();
        Data.Id = id;
        Data.Name = name;
        Data.CityIndex = 0;
        Data.CheckpointsDone = 0;
        Data.CheckpointsGoal = checkpointsGoal;
        Data.Country = country;
        Data.LeaderboardsOptIn = leaderboards_opt_in;

        cityIndex = 0;
    }

    internal void Reset()
    {
        Data.CityIndex = 0;
        Data.CheckpointsDone = 0;
        Data.CheckpointsGoal = 0;
        Data.Country = "";
        Data.LeaderboardsOptIn = false;

        checkpointsDone = 0;
        checkpointsGoal = 0;
        isNew = true;
        leaderboards_opt_in = false;
        country = "";
        cityIndex = 0;
        //currentDeviceUUID = "";


    }
}
