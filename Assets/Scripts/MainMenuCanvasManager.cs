using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvasManager : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject mainCanvas;
    public GameObject leaderboardsCanvas;
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
    public GameObject dogTogglePrefab;
    public GameObject dogWalkingTogglePrefab;
    

    public static MainMenuCanvasManager Instance { get; private set; }



    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.load();

        Instance = this;

        addCheckpointButton.onClick.AddListener(addCheckpoint);
        subtractCheckpointButton.onClick.AddListener(subtractCheckpoint);

        nextButton.onClick.AddListener(onNextButtonPressed);
        confirmButton.onClick.AddListener(onConfirmButtonPressed);

        firstTimeRun = true;
    }

    public void resetMainMenuUI()
    {

        foreach (GameObject dogToggle in GameManager.Instance.allDogs)
        {
            Dog currentDog = dogToggle.GetComponent<Dog>();

            dogToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate { onDogPressed(dogToggle); });

            foreach (DogModel dogModel in GameManager.Instance.dogModels)
            {
                if (currentDog.name == dogModel.Name && !GameManager.Instance.walkingDogs.Contains(currentDog))
                {
                    currentDog.InitializeFromModel(dogModel);
                    GameObject dogWalkingToggle = Instantiate(dogWalkingTogglePrefab, walkingDogs);
                    dogToggle.transform.SetParent(dogWalkingToggle.transform);
                    dogToggle.transform.localScale = new Vector3(1f, 1f, 1f);
                    dogToggle.transform.SetSiblingIndex(0);
                    dogWalkingToggle.GetComponentInChildren<TextMeshProUGUI>().text = currentDog.name;

                    lobbyCanvas.transform.parent.GetComponent<LobbyCanvasManager>().addNewDog(currentDog);
                    GameManager.Instance.walkingDogs.Add(currentDog);
                    break;
                }
            }
        }


    }

    public async Task reloadMainMenuUI()
    {

        await GameManager.Instance.load();

        foreach (GameObject dogToggle in GameManager.Instance.allDogs)
        {

            Dog currentDog = dogToggle.GetComponent<Dog>();

            foreach (DogModel walkingDog in GameManager.Instance.dogModels)
            {
                if (currentDog.name == walkingDog.Name && !GameManager.Instance.walkingDogs.Contains(currentDog))
                {
                    currentDog.InitializeFromModel(walkingDog);
                    GameObject dogWalkingToggle = Instantiate(dogWalkingTogglePrefab, walkingDogs);
                    dogToggle.transform.SetParent(dogWalkingToggle.transform);
                    dogToggle.transform.localScale = new Vector3(1f, 1f, 1f);
                    dogToggle.transform.SetSiblingIndex(0);
                    dogWalkingToggle.GetComponentInChildren<TextMeshProUGUI>().text = currentDog.name;
                    lobbyCanvas.transform.parent.GetComponent<LobbyCanvasManager>().addNewDog(currentDog);

                    GameManager.Instance.walkingDogs.Add(currentDog);

                    break;
                }
            }
        }

        foreach (DogModel walkingDog in GameManager.Instance.dogModels)
        {
            // check if they have checkpointed? if so, reset to available dogs
        }
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
            checkpointGoalText.text = currentDog.checkpointsGoal.ToString();
            checkpointsCounterNumber.text = currentDog.checkpointsDone.ToString();
        }
        else if(currentDog.isNew)
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
            beginAdventureCanvas.SetActive(true);
        selectDestinationCanvas.SetActive(true);

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
        currentDog.leaderboards_opt_in = yesLeaderboards.isOn;
        currentDog.country = destinationToggleGroup.ActiveToggles().FirstOrDefault().name;
        // Add new dog to Out for Walk dogs

        if (currentDog.isNew)
        {
            currentDog.SetCheckpoints(checkpointGoalText);
            currentDog.IsWalking();
            GameManager.Instance.walkingDogs.Add(currentDog);
            GameObject dogWalkingToggle = Instantiate(dogWalkingTogglePrefab, walkingDogs);
            currentDog.gameObject.transform.SetParent(dogWalkingToggle.transform);
            currentDog.gameObject.transform.SetSiblingIndex(0);
            dogWalkingToggle.GetComponentInChildren<TextMeshProUGUI>().text = currentDog.name;

            // Add a new dog profile icon to Lobby Canvas

            lobbyCanvas.transform.parent.GetComponent<LobbyCanvasManager>().addNewDog(currentDog);
        }
        else
        {
            lobbyCanvas.transform.parent.GetComponent<LobbyCanvasManager>().updateDog(currentDog);
            
        }


        GameManager.Instance.addWalkingDog(currentDog);
        // Hide MainMenu canvases and bring Lobby Canvas back to front

        selectDestinationCanvas.SetActive(false);
        mainCanvas.SetActive(false);

        // Show fun fact popup



        // Reset MainMenu Canvas UI

        resetUI();
    }

    public void openMainMenuCanvas()
    {

        //await GameManager.Instance.load();
        reloadMainMenuUI();
        mainCanvas.SetActive(true);

    }

    public void openLeaderboardsCanvas()
    {
        leaderboardsCanvas.SetActive(true);

    }

}
