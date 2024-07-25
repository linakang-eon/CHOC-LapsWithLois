using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Firebase;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public MainMenuCanvasManager mainMenuCanvasManager;
    public LobbyCanvasManager lobbyCanvasManager;
    public LeaderboardCanvasManager leaderboardCanvasManager;

    //public delegate void WalkingDogsLoadedEvent();
    //public static event WalkingDogsLoadedEvent onWalkingDogsLoaded;

    //public static void WalkingDogsLoaded()
    //{
    //    onWalkingDogsLoaded?.Invoke();
    //}

    

    public GameObject LoadingScreen;


    private CollectionReference db;

    public List<AudioClip> audios;

    public List<GameObject> allDogs;
    public List<DogModel> dogModels;
    public List<Dog> walkingDogs;
    public List<Dog> availableDogs;
    public List<GameObject> regions;


    private void Awake()
    {
        Instance = this;

        db = FirebaseFirestore.DefaultInstance.Collection("walkingDogs");

    }

    // Update Dog in db if already exists
    public void addWalkingDog(Dog dog)
    {
        DogModel model = new DogModel();
        model.Id = dog.id;
        model.Name = dog.name;
        model.CheckpointsDone = dog.checkpointsDone;
        model.CheckpointsGoal = dog.checkpointsGoal;
        model.Country = dog.country;
        model.LeaderboardsOptIn = dog.leaderboards_opt_in;
        model.CityIndex = dog.cityIndex;

        db.Document(model.Id).SetAsync(model);

    }

    //public async Task update(QuerySnapshot snapshot)
    //{
    //    IEnumerable<DocumentSnapshot> documents = snapshot.Documents as IEnumerable<DocumentSnapshot>;

    //    dogModels = new List<DogModel>();

    //    for (int i = 1; i < documents.Count(); i++)
    //    {
    //        dogModels.Add(documents.ElementAt(i).ConvertTo<DogModel>());
    //    }

    //    mainMenuCanvasManager.resetMainMenuUI();
    //}

    public async Task load(QuerySnapshot snap = null)
    {
        if(snap == null)
            snap = await db.GetSnapshotAsync();

        IEnumerable<DocumentSnapshot> documents = snap.Documents as IEnumerable<DocumentSnapshot>;

        DogModel date = documents.ElementAt(0).ConvertTo<DogModel>();

        string todaysDate = DateTime.Now.Date.ToString();

        dogModels = new List<DogModel>();

        if (date.Name != todaysDate)
        {
            // delete all documents

            for (int i = 0; i < documents.Count(); i++)
            {
                DogModel data = documents.ElementAt(i).ConvertTo<DogModel>();
                db.Document(data.Id).DeleteAsync();

            }

            DogModel today = new DogModel();
            today.Id = "0";
            today.Name = todaysDate;
            db.Document(today.Id).SetAsync(today);
        }
        else
        {
            for (int i = 1; i < documents.Count(); i++)
            {
                DogModel data = documents.ElementAt(i).ConvertTo<DogModel>();
                dogModels.Add(data);
            }

        }

        mainMenuCanvasManager.resetMainMenuUI();
        
        LoadingScreen.SetActive(false);


    }

    public void StartListeningForUpdates()
    {
        ListenerRegistration registration = db.Listen(
        querySnapshot =>
        {
            load(querySnapshot);
        });
    }

    public Sprite FindDogSpriteByName(string name)
    {
        foreach(GameObject dogObject in allDogs)
        {
            if(dogObject.GetComponent<Dog>().name == name)
            {
                return dogObject.GetComponent<Image>().sprite;
            }
        }

        throw new Exception("GameManager: Could not find Dog Sprite by Name of " + name);

        
    }

    public void PlayAudio(string audio)
    {
        switch(audio)
        {
            case "France":
                gameObject.GetComponent<AudioSource>().PlayOneShot(audios[0], 1);
                break;
            case "Egypt":
                gameObject.GetComponent<AudioSource>().PlayOneShot(audios[1], 1);
                break;
            case "Japan":
                gameObject.GetComponent<AudioSource>().PlayOneShot(audios[2], 1);
                break;
        }
    }
}

