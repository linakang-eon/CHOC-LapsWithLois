using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

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

            GoToNextCity();
        });

        
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

            StartCoroutine(GoToNextCity());
        }
        StartCoroutine(WaitFor30Seconds(selectedDog));
    }

    IEnumerator GoToNextCity()
    {
        travelAnimation.GetComponent<VideoPlayer>().clip = GameManager.Instance.GetRandomTravelVideo();
        travelAnimation.SetActive(true);

        yield return new WaitForSeconds(7);

        travelAnimation.SetActive(false);

        GameManager.Instance.ActivateFactDialogue(selectedDog.GetComponent<Dog>());

        GameManager.Instance.PlayAudio(selectedDog.GetComponent<Dog>().country);
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

    public void onToggled(GameObject DogProfileToggle, bool playAudioYesOrNo)
    {
        selectedDog = DogProfileToggle;

        if (selectedDog.GetComponent<Dog>().timerDone)
            checkpointButton.SetActive(true);
        else
            checkpointButton.SetActive(false);

        string destination = selectedDog.GetComponent<Dog>().country;

        Transform dogDestination = backgroundAnimation.transform.Find(destination);
        Transform dogCity = dogDestination.GetChild(selectedDog.GetComponent<Dog>().cityIndex);

        if (currentCity.name != dogCity.name)
        {
            if(currentCountry.name != destination)
            {
                currentCountry.SetActive(false);
                currentCountry = dogDestination.gameObject;
                currentCountry.SetActive(true);
            }

            currentCity.SetActive(false);
            currentCity = currentCountry.transform.GetChild(selectedDog.GetComponent<Dog>().cityIndex).gameObject;
            currentCity.SetActive(true);

            switch (destination)
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

        if(DogProfileToggle.transform.GetChild(1).GetComponent<Toggle>().isOn)
        {
            if(playAudioYesOrNo)
            {
                GameManager.Instance.PlayAudio("dogToggleLobby");
            }

            DogProfileToggle.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color32(0, 100, 164, 255);
            DogProfileToggle.transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            DogProfileToggle.transform.GetChild(0).gameObject.SetActive(true);

        }
        else
        {
            

            DogProfileToggle.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 175);
            DogProfileToggle.transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, 200);
            DogProfileToggle.transform.GetChild(0).gameObject.SetActive(false);
        }

        // Disable all other buttons if one is toggled


    }

    public void addNewDog(Dog currentDog)
    {
        GameObject dogLobbyToggle = Instantiate(dogProfilePrefab, dogProfileIcons);
        dogToggles.Add(dogLobbyToggle);
        dogLobbyToggle.GetComponent<Dog>().SetDog(currentDog);
        dogLobbyToggle.transform.GetChild(1).GetComponent<Image>().sprite = currentDog.thumbnail;
        dogLobbyToggle.transform.GetChild(1).GetComponent<Toggle>().group = dogProfileIcons.GetComponent<ToggleGroup>();
        dogLobbyToggle.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = currentDog.name;

        dogLobbyToggle.transform.GetChild(1).GetComponent<Toggle>().isOn = true;
        onToggled(dogLobbyToggle, false);

        dogLobbyToggle.transform.GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener(delegate
        {
            onToggled(dogLobbyToggle, true);
        });

        

        StartCoroutine(WaitFor30Seconds(dogLobbyToggle));
    }

    IEnumerator WaitFor30Seconds(GameObject dogLobbyToggle)
    {
        dogLobbyToggle.GetComponent<Dog>().timerDone = false;
        yield return new WaitForSeconds(10);
        dogLobbyToggle.GetComponent<Dog>().timerDone = true;
        Debug.Log(dogLobbyToggle.GetComponent<Dog>().name + " - checkpoint button activated");
        
    }

    public void updateDog(Dog currentDog, bool toggleTrue)
    {
        foreach(Transform dog in dogProfileIcons)
        {
            if(dog.GetComponent<Dog>().id == currentDog.id)
            {
                dog.GetComponent<Dog>().SetDog(currentDog);
                if(toggleTrue)
                    onToggled(dog.gameObject, false);
                break;
            }

        }

    }

}
