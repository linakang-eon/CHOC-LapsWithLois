using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Firebase;
using Firebase.Firestore;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public MainMenuCanvasManager mainMenuCanvasManager;
    public LobbyCanvasManager lobbyCanvasManager;

    public delegate void WalkingDogsLoadedEvent(List<DogModel> walkingDogs);
    public static event WalkingDogsLoadedEvent onWalkingDogsLoaded;

    public static void WalkingDogsLoaded(List<DogModel> walkingDogs)
    {
        onWalkingDogsLoaded?.Invoke(walkingDogs);
    }



    private CollectionReference db;

    public List<DogConfig> allDogs;
    public List<DogModel> dogModels;
    public List<Dog> walkingDogs;
    public List<Dog> availableDogs;
    public List<GameObject> regions;


    private void Awake()
    {
        Instance = this;

        db = FirebaseFirestore.DefaultInstance.Collection("walkingDogs");

        load();

    }

    // Update Dog in db if already exists
    public void addWalkingDog(Dog dog)
    {
        DogModel model = new DogModel();
        model.Name = dog.name;
        model.Id = dog.id;
        model.CheckpointsDone = dog.checkpointsFinished;
        model.CheckpointsGoal = dog.checkpointGoal;
        model.Country = dog.country;
        model.LeaderboardsOptIn = dog.leaderboards_opt_in;

        db.Document(model.Id).SetAsync(model);

    }

    private void load()
    {
        db.GetSnapshotAsync().ContinueWith(task =>
        {
            var documents = task.Result.Documents;

            dogModels = new List<DogModel>();
            foreach(var document in documents)
            {
                DogModel data = document.ConvertTo<DogModel>();
                dogModels.Add(data);
            }
            WalkingDogsLoaded(dogModels);
            //mainMenuCanvasManager.initializeMainMenuUI(dogModels);
            //lobbyCanvasManager.initializeLobbyUI();

        });

        //    db.Document(model.Id).GetSnapshotAsync().ContinueWith(task =>
        //{
        //    if (task.Result.Exists)
        //    {
        //        DogModel data = task.Result.ConvertTo<DogModel>();
        //        Debug.Log($"username: {data.Name}" + $"id: {data.Id}");
        //    }
        //});
    }

    public Sprite FindDogSpriteByName(string name)
    {
        foreach(DogConfig dogConfig in allDogs)
        {
            if(dogConfig.name == name)
            {
                return dogConfig.thumbnail;
            }
        }

        throw new Exception("GameManager: Could not find Dog Sprite by Name of " + name);

        
    }
}

