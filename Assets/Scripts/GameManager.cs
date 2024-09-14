using System;
using System.Collections.Generic;
using UnityEngine;

using Firebase.Firestore;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public MainMenuCanvasManager mainMenuCanvasManager;
    public LobbyCanvasManager lobbyCanvasManager;
    public LeaderboardCanvasManager leaderboardCanvasManager;

    public GameObject LoadingScreen;

    public Transform factsDialogue;


    private CollectionReference db;

    public List<GameObject> travelVideos;

    public List<GameObject> goalReachedFranceVideos;
    public List<GameObject> goalReachedEgyptVideos;
    public List<GameObject> goalReachedJapanVideos;

    public List<AudioClip> countryBGM;
    public List<AudioClip> sfx;
    public List<AudioClip> dogBarks;

    public List<GameObject> allDogs;
    public List<DogModel> dogModels;
    public List<Dog> walkingDogs;
    public List<Dog> availableDogs;
    public List<GameObject> countries;

    private int dogBarkPrevious = 0;

    private bool firstTimeRun = true;



    private void Awake()
    {
        Instance = this;

        db = FirebaseFirestore.DefaultInstance.Collection("walkingDogs");

        foreach(GameObject video in travelVideos)
        {
            video.GetComponent<VideoPlayer>().Prepare();
        }

        foreach (GameObject video in goalReachedFranceVideos)
            video.GetComponent<VideoPlayer>().Prepare();
        foreach (GameObject video in goalReachedEgyptVideos)
            video.GetComponent<VideoPlayer>().Prepare();
        foreach (GameObject video in goalReachedJapanVideos)
            video.GetComponent<VideoPlayer>().Prepare();
    }

    // Update Dog in db if already exists
    public void addWalkingDogToDB(Dog dog)
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

        if(firstTimeRun)
        {
            StartListeningForUpdates();

            LoadingScreen.SetActive(false);

            firstTimeRun = false;
        }    

        mainMenuCanvasManager.resetMainMenuUI();

    }
    public void StartListeningForUpdates()
    {
        ListenerRegistration registration = db.Listen(
        querySnapshot =>
        {
            load(querySnapshot);
        });
    }

    internal void ActivateFactDialogue(Dog currentDog)
    {
        if (currentDog == null)
        {
            Debug.Log("ActivateFactDialogue could not work");
            return;
        }
        string country = currentDog.country;
        int cityIndex = currentDog.cityIndex;
        string cityName;

        Transform cityTransform = null;

        switch (country)
        {
            case "France":
                cityTransform = countries[0].transform.GetChild(cityIndex);
                
                break;
            case "Egypt":
                cityTransform = countries[1].transform.GetChild(cityIndex);
                break;
            case "Japan":
                cityTransform = countries[2].transform.GetChild(cityIndex);
                break;
        }
        cityName = "facts " + cityTransform.name;
        factsDialogue.Find(cityName).gameObject.SetActive(true);
        

    }

    internal GameObject GetRandomTravelVideo()
    {
        System.Random rndm = new System.Random();
        return travelVideos[rndm.Next(0, travelVideos.Count)];
    }

    internal GameObject GetRandomGoalReachedVideo(string country)
    {
        
        System.Random rndm = new System.Random();

        if (country == "France")
            return goalReachedFranceVideos[rndm.Next(0, goalReachedFranceVideos.Count)];
        if (country == "Egypt")
            return goalReachedEgyptVideos[rndm.Next(0, goalReachedEgyptVideos.Count)];
        
        return goalReachedJapanVideos[rndm.Next(0, goalReachedJapanVideos.Count)];

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
                gameObject.GetComponent<AudioSource>().PlayOneShot(countryBGM[0], 0.3f);
                break;
            case "Egypt":
                gameObject.GetComponent<AudioSource>().PlayOneShot(countryBGM[1], 0.3f);
                break;
            case "Japan":
                gameObject.GetComponent<AudioSource>().PlayOneShot(countryBGM[2], 0.3f);
                break;
            case "dogToggleLobby":
                //System.Random rndm = new System.Random();
                //int rndmNumber = rndm.Next(0, 5);
                //while(dogBarkPrevious == rndmNumber)
                //    rndmNumber = rndm.Next(0, 5);
                dogBarkPrevious += 1;
                if (dogBarkPrevious == 5)
                    dogBarkPrevious = 0;
                gameObject.GetComponent<AudioSource>().PlayOneShot(dogBarks[dogBarkPrevious], 0.3f);
                break;
            case "dogToggleMenu":
                gameObject.GetComponent<AudioSource>().PlayOneShot(sfx[2], 0.1f);
                break;
            case "winSound":
                gameObject.GetComponent<AudioSource>().PlayOneShot(sfx[3], 0.3f);
                break;
            case "nextSound":
                gameObject.GetComponent<AudioSource>().PlayOneShot(sfx[1], 0.3f);
                break;
        }
    }
}

