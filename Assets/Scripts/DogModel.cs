using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;
using UnityEngine;

[FirestoreData]
public class DogModel
{
    private string id;
    private string name;
    private int checkpointsDone;
    private int checkpointsGoal;
    private string country;
    private bool leaderboardsOptIn;

    //public DogModel(string id, string name, int chkDone = 0, int chkGoal = 0, string country = "BLANK", bool leaderboardsOpt = false)
    //{
    //    this.id = id;
    //    this.name = name;
    //    this.checkpointsDone = chkDone;
    //    this.checkpointsGoal = chkGoal;
    //    this.country = country;
    //    this.leaderboardsOptIn = leaderboardsOpt;
    //}

    [FirestoreProperty]
    public string Id
    {
        get => id;
        set => id = value;
    }

    [FirestoreProperty]
    public string Name
    {
        get => name;
        set => name = value;
    }

    [FirestoreProperty]
    public int CheckpointsDone
    {
        get => checkpointsDone;
        set => checkpointsDone = value;
    }

    [FirestoreProperty]
    public int CheckpointsGoal
    {
        get => checkpointsGoal;
        set => checkpointsGoal = value;
    }

    [FirestoreProperty]
    public string Country
    {
        get => country;
        set => country = value;
    }

    [FirestoreProperty]
    public bool LeaderboardsOptIn
    {
        get => leaderboardsOptIn;
        set => leaderboardsOptIn = value;
    }


}
