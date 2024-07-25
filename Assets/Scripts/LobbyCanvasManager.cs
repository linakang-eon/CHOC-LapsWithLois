using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCanvasManager : MonoBehaviour
{
    [Header("Animations")]
    public GameObject backgroundAnimation;
    public GameObject goodJobAnimation;
    public GameObject travelAnimation;
    public GameObject goalReachedFrance;
    public GameObject goalReachedEgypt;
    public GameObject goalReachedJapan;
    public GameObject goalReachedBackButton;
    public GameObject goalReachedContinueButton;
    public GameObject loisEgypt;
    public GameObject loisFrance;
    public GameObject loisJapan;


    [Header("Currents")]
    private GameObject selectedDog;
    public GameObject currentCountry;
    public GameObject currentCity;
    public GameObject currentGoalReachedScreen;

    [Header("UI")]
    public GameObject checkpointButton;
    public GameObject continueButton;
    public GameObject dogProfilePrefab;
    public Transform dogProfileIcons;

    private List<GameObject> dogToggles;

    

    void Start()
    {
        dogToggles = new List<GameObject>();

        checkpointButton.GetComponent<Button>().onClick.AddListener(delegate {
            StartCoroutine(Checkpoint());
        });

        goalReachedBackButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            currentCity.SetActive(false);
            currentCountry.transform.GetChild(selectedDog.GetComponent<Dog>().cityIndex).gameObject.SetActive(true);

            // Go to main menu
            currentGoalReachedScreen.SetActive(false);
            GameManager.Instance.mainMenuCanvasManager.openMainMenuCanvas();
        });

        goalReachedContinueButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            currentCity.SetActive(false);
            currentCountry.transform.GetChild(selectedDog.GetComponent<Dog>().cityIndex).gameObject.SetActive(true);

            // Go to walking lois
            currentGoalReachedScreen.SetActive(false);
        });

        GameManager.Instance.StartListeningForUpdates();
    }


    IEnumerator Checkpoint()
    {
        checkpointButton.SetActive(false);

        goodJobAnimation.SetActive(true);
        
        yield return new WaitForSeconds(3);

        goodJobAnimation.SetActive(false);

        // Check if they met their goal

        
        bool checkPointsCompleted = selectedDog.GetComponent<Dog>().checkpoint();
        
        if(checkPointsCompleted)
        {
            string country = selectedDog.GetComponent<Dog>().country;

            if (country == "France")
                currentGoalReachedScreen = goalReachedFrance;
            else if (country == "Egypt")
                currentGoalReachedScreen = goalReachedEgypt;
            else
                currentGoalReachedScreen = goalReachedJapan;

            currentGoalReachedScreen.SetActive(true);

            goalReachedBackButton.SetActive(true);
            goalReachedContinueButton.SetActive(true);

        }
        else
        {
            currentCity.SetActive(false);
            currentCountry.transform.GetChild(selectedDog.GetComponent<Dog>().cityIndex).gameObject.SetActive(true);

            travelAnimation.SetActive(true);

            yield return new WaitForSeconds(7);

            travelAnimation.SetActive(false);

            GameManager.Instance.PlayAudio(selectedDog.GetComponent<Dog>().country);
        }


    }

    internal void scorchedEarth()
    {
        for(int i = 0; i < dogToggles.Count(); i++)
        {
            GameObject dogToggle = dogToggles[i];
            dogToggles.Remove(dogToggle);
            Destroy(dogToggle);
            i--;
        }

    }

    public void initializeLobbyUI()
    {
        List<Dog> walkingDogsList = GameManager.Instance.walkingDogs;
        foreach (Dog dog in walkingDogsList)
        {
            addNewDog(dog);
        }
    }


    public void onToggled(GameObject DogProfileToggle)
    {
        selectedDog = DogProfileToggle;

        if(selectedDog.GetComponent<Dog>().currentDeviceUUID != SystemInfo.deviceUniqueIdentifier)
        {
            checkpointButton.SetActive(selectedDog.GetComponent<Toggle>().isOn);
        }
        

        string destination = selectedDog.GetComponent<Dog>().country;

        Transform currentDestination = backgroundAnimation.transform.Find(destination);

        currentCountry.SetActive(false);
        currentCity.SetActive(false);

        currentCountry = currentDestination.gameObject;
        currentCountry.SetActive(true);

        currentCity = currentCountry.transform.GetChild(selectedDog.GetComponent<Dog>().cityIndex).gameObject;
        currentCity.SetActive(true);

        switch(destination)
        {
            case "France":
                loisFrance.SetActive(true);
                loisEgypt.SetActive(false);
                loisJapan.SetActive(false);
                break;
            case "Egypt":
                loisEgypt.SetActive(true);
                loisFrance.SetActive(false);
                loisJapan.SetActive(false);
                break;
            case "Japan":
                loisJapan.SetActive(true);
                loisEgypt.SetActive(false);
                loisFrance.SetActive(false);
                break;
        }

    }

    public void addNewDog(Dog currentDog)
    {
        GameObject dogLobbyToggle = Instantiate(dogProfilePrefab, dogProfileIcons);
        dogToggles.Add(dogLobbyToggle);
        dogLobbyToggle.GetComponent<Dog>().SetDog(currentDog);
        dogLobbyToggle.GetComponent<Image>().sprite = currentDog.thumbnail;
        dogLobbyToggle.GetComponent<Toggle>().group = dogProfileIcons.GetComponent<ToggleGroup>();
        dogLobbyToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate
        {
            onToggled(dogLobbyToggle);
        });
    }

    public void updateDog(Dog currentDog)
    {
        foreach(Transform dog in dogProfileIcons)
        {
            if(dog.GetComponent<Dog>().id == currentDog.id)
            {
                dog.GetComponent<Dog>().SetDog(currentDog);
                break;
            }

        }

    }

}
