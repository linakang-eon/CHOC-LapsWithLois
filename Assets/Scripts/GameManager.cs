using System;
using System.Collections.Generic;
using UnityEngine;

using Firebase.Firestore;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public MainMenuCanvasManager mainMenuCanvasManager;
    public LobbyCanvasManager lobbyCanvasManager;
    public LeaderboardCanvasManager leaderboardCanvasManager;

    public GameObject LoadingScreen;

    public Transform factsDialogue;
    public GameObject newUserDialogue;

    public GameObject resetAllDogsDialogue;
    private int resetNumber;
    public List<GameObject> resetButtonClickIndicators;

    private CollectionReference db;

    public GameObject travelScreen;
    public List<GameObject> travelVideos;

    public GameObject goalReachedScreen;
    public List<GameObject> goalReachedFranceVideos;
    public List<GameObject> goalReachedEgyptVideos;
    public List<GameObject> goalReachedJapanVideos;
    public GameObject currentGoalReachedVideo;

    public List<GameObject> passportMontageVideos;

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

    private GameObject currentFactsDialogue;

    private string todaysDate = DateTime.Now.Date.ToString();
    private Coroutine countToTen;

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
            today.Name = DateTime.Now.Date.ToString();
            todaysDate = DateTime.Now.Date.ToString();
            db.Document(today.Id).SetAsync(today);

        }
        else
        {
            for (int i = 1; i < documents.Count(); i++)
            {
                DogModel data = documents.ElementAt(i).ConvertTo<DogModel>();
                dogModels.Add(data);
            }
            todaysDate = DateTime.Now.Date.ToString();

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
        
        currentFactsDialogue = factsDialogue.Find(cityName).gameObject;
        currentFactsDialogue.SetActive(true);
    }

    public void TurnOffFactsDialogue()
    {
        if(currentFactsDialogue != null)
            currentFactsDialogue.SetActive(false);
    }


    public IEnumerator PlayRandomTravelVideo(Dog selectedDog)
    {
        travelScreen.SetActive(true);
        System.Random rndm = new System.Random();
        GameObject video = travelVideos[rndm.Next(0, travelVideos.Count)];


        video.GetComponent<RawImage>().enabled = true;
        video.GetComponent<VideoPlayer>().Play();
        video.GetComponent<Animator>().enabled = true;
        video.GetComponent<Animator>().Play("Travel", 0, 0f);

        yield return new WaitForSeconds(7);

        ActivateFactDialogue(selectedDog);


        video.GetComponent<VideoPlayer>().Stop();
        video.GetComponent<RawImage>().enabled = false;
        video.GetComponent<VideoPlayer>().Prepare();
        video.GetComponent<Animator>().enabled = false;

        travelScreen.SetActive(false);

        PlayAudio(selectedDog.country);

    }
    internal GameObject GetRandomPassportVideo()
    {
        System.Random rndm = new System.Random();
        return passportMontageVideos[rndm.Next(0, passportMontageVideos.Count)];
    }
    

    public void PlayRandomGoalReachedVideo(string country)
    {
        goalReachedScreen.SetActive(true);
        System.Random rndm = new System.Random();

        if (country == "France")
            currentGoalReachedVideo = goalReachedFranceVideos[rndm.Next(0, goalReachedFranceVideos.Count)];
        else if (country == "Egypt")
            currentGoalReachedVideo = goalReachedEgyptVideos[rndm.Next(0, goalReachedEgyptVideos.Count)];
        else
            currentGoalReachedVideo = goalReachedJapanVideos[rndm.Next(0, goalReachedJapanVideos.Count)];


        currentGoalReachedVideo.GetComponent<RawImage>().enabled = true;
        currentGoalReachedVideo.GetComponent<VideoPlayer>().Play();
    }

    public void StopGoalReachedVideo()
    {
        currentGoalReachedVideo.GetComponent<VideoPlayer>().Stop();
        currentGoalReachedVideo.GetComponent<VideoPlayer>().Prepare();
        currentGoalReachedVideo.GetComponent<RawImage>().enabled = false;
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

    public void StopCurrentAudio()
    {
        gameObject.GetComponent<AudioSource>().Stop();
    }

    internal void ActivateNewUserDialogue()
    {
        newUserDialogue.SetActive(true);
    }

    public void OpenResetDogsDialogue(int leftOrRight)
    {
        // The user must click the left (aka 0) first, then right (aka 1), and lastly left (aka 0) in ORDER to activate RESET

        // If the user does not perform this routine within 10 seconds,
        //  the number will reset and the user must redo the order from beginning in order to activate the reset

        if (resetNumber == 0)
        {
            countToTen = StartCoroutine(CountToTen());

            if (leftOrRight == 0)
            {
                resetButtonClickIndicators[0].SetActive(true);
                resetNumber = 1;
            }
        }
        else if (resetNumber == 1 && leftOrRight == 1)
        {
            resetNumber = 2;
            resetButtonClickIndicators[1].SetActive(true);
        }
        else if (resetNumber == 2 && leftOrRight == 0)
        {
            resetNumber = 0;
            resetAllDogsDialogue.SetActive(true);
            resetButtonClickIndicators[0].SetActive(false);
            resetButtonClickIndicators[1].SetActive(false);
            StopCoroutine(countToTen);
        }
        else
        {
            resetNumber = 0;
            StopCoroutine(countToTen);
            resetButtonClickIndicators[0].SetActive(false);
            resetButtonClickIndicators[1].SetActive(false);
        }
    }

    public void ResetAllDogs()
    {
        resetButtonClickIndicators[0].SetActive(false);
        resetButtonClickIndicators[1].SetActive(false);
        todaysDate = "0";
        load();
    }

    IEnumerator CountToTen()
    {
        yield return new WaitForSeconds(10);

        resetNumber = 0;
        resetButtonClickIndicators[0].SetActive(false);
        resetButtonClickIndicators[1].SetActive(false);
    }
}

