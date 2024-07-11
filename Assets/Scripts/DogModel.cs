using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;
using UnityEngine;

[FirestoreData]
public class DogModel
{
    private string name = "Turbo";
    private string id = "0";
    private int checkpointsDone = 3;
    private int checkpointsGoal = 6;
    private string country = "Egypt";
    private bool leaderboardsOptIn = false;

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
