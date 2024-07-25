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
    private int cityIndex;

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

    [FirestoreProperty]
    public int CityIndex
    {
        get => cityIndex;
        set => cityIndex = value;
    }
}
