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
        Instance = this;

        GameManager.onWalkingDogsLoaded += delegate (List<DogModel> dogModels)
        {
            Debug.Log("StartCoroutine called");
            StartCoroutine(initializeMainMenuUI(dogModels));
            Debug.Log("StartCoroutine ended");
        };



        addCheckpointButton.onClick.AddListener(addCheckpoint);
        subtractCheckpointButton.onClick.AddListener(subtractCheckpoint);

        nextButton.onClick.AddListener(onNextButtonPressed);
        confirmButton.onClick.AddListener(onConfirmButtonPressed);

        firstTimeRun = true;
    }

    private IEnumerator initializeMainMenuUI(List<DogModel> dogModels)
    {

        Debug.Log("callMethod() invoked");

        if (dogTogglePrefab == null)
        {
            Debug.LogError("dogPrefab is not assigned!");
            yield break;
        }

        if (availableDogs == null)
        {
            Debug.LogError("dogPrefabIcons is not assigned!");
            yield break;
        }
        yield return null;

        List<DogConfig> allDogs = GameManager.Instance.allDogs;
        foreach (DogConfig dogConfig in allDogs)
        {
            GameObject dogGameObject = Instantiate(dogTogglePrefab, availableDogs);
            Dog currentDog = dogGameObject.GetComponent<Dog>();
            currentDog.Initialize(dogConfig);
            dogGameObject.GetComponent<Image>().sprite = currentDog.thumbnail;
            dogGameObject.GetComponent<Toggle>().onValueChanged.AddListener(delegate { onDogPressed(dogGameObject); });

            foreach (DogModel dogModel in GameManager.Instance.dogModels)
            {
                if (currentDog.name == dogModel.Name)
                {
                    currentDog.Data = dogModel;
                    GameObject dogWalkingToggle = Instantiate(dogWalkingTogglePrefab, walkingDogs);
                    dogGameObject.transform.SetParent(dogWalkingToggle.transform);
                    dogGameObject.transform.SetSiblingIndex(0);
                    dogWalkingToggle.GetComponentInChildren<TextMeshProUGUI>().text = currentDog.name;

                    GameManager.Instance.walkingDogs.Add(currentDog);

                }
            }
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
            GameManager.Instance.addWalkingDog(currentDog);

        }
        else
        {
            lobbyCanvas.transform.parent.GetComponent<LobbyCanvasManager>().updateDog(currentDog);
            GameManager.Instance.addWalkingDog(currentDog);
        }

        currentDog.leaderboards_opt_in = yesLeaderboards.isOn;
        currentDog.country = destinationToggleGroup.ActiveToggles().FirstOrDefault().name;

        // Hide MainMenu canvases and bring Lobby Canvas back to front

        selectDestinationCanvas.SetActive(false);
        mainCanvas.SetActive(false);

        // Show fun fact popup



        // Reset MainMenu Canvas UI

        resetUI();
    }

    public void openMainMenuCanvas()
    {
        mainCanvas.SetActive(true);

    }

    public void openLeaderboardsCanvas()
    {
        leaderboardsCanvas.SetActive(true);

    }

}
