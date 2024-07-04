using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvasManager : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject mainCanvas;
    public GameObject beginAdventureCanvas;
    public GameObject selectDestinationCanvas;
    public GameObject lobbyCanvas;

    [Header("New Patient")]
    public GameObject backButtonLeftPanel;
    public GameObject rightPanelFirst;
    public GameObject rightPanelSecond;
    public Button addCheckpointButton;
    public Button subtractCheckpointButton;
    public TextMeshProUGUI checkpointGoalText;

    [Header("Returning Patient")]
    public GameObject checkpointsCounter;
    public TextMeshProUGUI checkpointsCounterNumber;

    [Header("Dog Selection UI")]
    public Image dogProfileIcon;
    public TextMeshProUGUI dogProfileName;
    private Dog currentDog;
    private Toggle currentlyPressedDogToggle;
    public Toggle yesLeaderboards;
    public Toggle noLeaderboards;
    public Button nextButton;

    [Header("Select a Destination UI")]
    public ToggleGroup destinationToggleGroup;
    public Button confirmButton;

    public Transform availableDogs;
    public Transform walkingDogs;
    private bool firstTimeRun;




    // Start is called before the first frame update
    void Start()
    {
        List<Transform> walkingDogsList = new List<Transform>();
        foreach(Transform dog in availableDogs)
        {
            if (GameManager.Instance.walkingDogs.Contains(dog.GetComponent<Dog>()))
                walkingDogsList.Add(dog);

        }
        foreach(Transform dog in walkingDogsList)
        {
            dog.GetComponent<Dog>().IsWalking();
            dog.SetParent(walkingDogs);
        }

        addCheckpointButton.onClick.AddListener(addCheckpoint);
        subtractCheckpointButton.onClick.AddListener(subtractCheckpoint);

        nextButton.onClick.AddListener(onNextButtonPressed);
        confirmButton.onClick.AddListener(onConfirmButtonPressed);

        firstTimeRun = true;
    }



    private void addCheckpoint()
    {
        int number = System.Int32.Parse(checkpointGoalText.text);
        number += 1;
        checkpointGoalText.text = number.ToString();
    }

    private void subtractCheckpoint()
    {
        int number = System.Int32.Parse(checkpointGoalText.text);
        if(number > 1)
        {
            number -= 1;
            checkpointGoalText.text = number.ToString();
        }
    }

    public void onDogPressed(GameObject dogPressed)
    {
        if(firstTimeRun)
        {
            backButtonLeftPanel.SetActive(true);
            rightPanelFirst.SetActive(false);
            rightPanelSecond.SetActive(true);
            dogPressed.GetComponent<Toggle>().interactable = false;
            firstTimeRun = false;
        }
        else
        {
            currentlyPressedDogToggle.isOn = false;
            currentlyPressedDogToggle.interactable = true;
        }

        bool isActive = dogPressed.GetComponent<Toggle>().isOn;
        currentDog = dogPressed.GetComponent<Dog>();
        currentlyPressedDogToggle = dogPressed.GetComponent<Toggle>();

        dogProfileIcon.sprite = currentDog.thumbnail;
        dogProfileName.text = currentDog.name;

        if (isActive && !currentDog.isNew)
        {
            checkpointsCounter.SetActive(true);
            addCheckpointButton.gameObject.SetActive(false);
            subtractCheckpointButton.gameObject.SetActive(false);
            if (currentDog.leaderboards_opt_in)
                yesLeaderboards.isOn = true;
            else
                noLeaderboards.isOn = true;
            checkpointGoalText.text = currentDog.checkpointGoal.ToString();
            checkpointsCounterNumber.text = currentDog.checkpointsFinished.ToString();
        }
        else
        {
            checkpointsCounter.SetActive(false);
            addCheckpointButton.gameObject.SetActive(true);
            subtractCheckpointButton.gameObject.SetActive(true);
            checkpointGoalText.text = "0";
        }
    }

    public void onNextButtonPressed()
    {
        if(currentDog.isNew)
        {
            mainCanvas.SetActive(false);
            beginAdventureCanvas.SetActive(true);
        }
        else
        {
            mainCanvas.SetActive(false);
            selectDestinationCanvas.SetActive(true);
        }
    }

    public void resetUI()
    {
        currentlyPressedDogToggle.isOn = false;
        currentlyPressedDogToggle.interactable = true;

        backButtonLeftPanel.SetActive(false);
        rightPanelFirst.SetActive(true);
        rightPanelSecond.SetActive(false);


        checkpointsCounter.SetActive(!currentDog.isNew);
        

        firstTimeRun = true;
        currentDog = null;
        currentlyPressedDogToggle = null;
    }

    public void onConfirmButtonPressed()
    {
        // Add new dog to Out for Walk dogs

        currentDog.leaderboards_opt_in = yesLeaderboards.isOn;
        currentDog.country = destinationToggleGroup.ActiveToggles().FirstOrDefault().name;
        if (currentDog.isNew)
        {
            currentDog.SetCheckpoints(checkpointGoalText);
            GameManager.Instance.walkingDogs.Add(currentDog);
            currentDog.IsWalking();
            currentDog.gameObject.transform.SetParent(walkingDogs);

            // Add a new dog profile icon to Lobby Canvas

            lobbyCanvas.GetComponent<LobbyCanvasManager>().addNewDog(currentDog);


        }



        // Hide MainMenu canvases and bring Lobby Canvas back to front

        selectDestinationCanvas.SetActive(false);
        mainCanvas.SetActive(true);
        mainCanvas.gameObject.transform.parent.gameObject.SetActive(false);
        lobbyCanvas.SetActive(true);

        // Show fun fact popup



        // Reset MainMenu Canvas UI

        resetUI();
    }
}
