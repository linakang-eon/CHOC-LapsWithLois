using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuCanvasManager : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject mainCanvas;
    public GameObject leaderboardsCanvas;
    public GameObject beginAdventureCanvas;
    public GameObject beginAdventureVideoPlayer;
    public GameObject beginAdventureDialogue;
    public GameObject selectDestinationCanvas;
    public GameObject lobbyCanvas;
    public GameObject passportCanvas;
    public GameObject passportCanvasFrance;
    public GameObject passportCanvasEgypt;
    public GameObject passportCanvasJapan;
    public GameObject passportVideoPlayer;

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

    [Header("Passport UI")]
    public Button startButton;

    [Header("Begin Adventure Canvas")]
    public Button beginAdventureButton;

    public Transform availableDogs;
    public Transform walkingDogs;
    private bool firstTimeRun;
    public GameObject dogTogglePrefab;
    public GameObject dogWalkingTogglePrefab;
    

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.load();

        addCheckpointButton.onClick.AddListener(addCheckpoint);
        subtractCheckpointButton.onClick.AddListener(subtractCheckpoint);

        nextButton.onClick.AddListener(onNextButtonPressed);
        confirmButton.onClick.AddListener(onConfirmButtonPressed);

        firstTimeRun = true;

        passportVideoPlayer.GetComponent<VideoPlayer>().Prepare();
    }

    public void resetMainMenuUI()
    {
        if (GameManager.Instance.dogModels.Count() == 0)
        {
            // Scorched Earth
            for(int i = 0; i < GameManager.Instance.walkingDogs.Count(); i++)
            {
                Dog dogToggle = GameManager.Instance.walkingDogs[i];
                GameObject walkingDogTogglePrefabClone = dogToggle.transform.parent.gameObject;
                dogToggle.transform.SetParent(availableDogs);
                dogToggle.transform.localScale = new Vector3(1f, 1f, 1f);
                dogToggle.Reset();
                Destroy(walkingDogTogglePrefabClone);
                GameManager.Instance.walkingDogs.Remove(dogToggle);
                i--;
            }

            lobbyCanvas.transform.parent.GetComponent<LobbyCanvasManager>().scorchedEarth();
            leaderboardsCanvas.GetComponent<LeaderboardCanvasManager>().scorchedEarth();
        }

        foreach (GameObject dogToggle in GameManager.Instance.allDogs)
        {
            Dog currentDog = dogToggle.GetComponent<Dog>();

            dogToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate { onDogPressed(dogToggle); });

            foreach (DogModel dogModel in GameManager.Instance.dogModels)
            {
                if (currentDog.name == dogModel.Name)
                {
                    currentDog.InitializeFromModel(dogModel);
                    if (!GameManager.Instance.walkingDogs.Contains(currentDog))
                    {
                        GameObject dogWalkingToggle = Instantiate(dogWalkingTogglePrefab, walkingDogs);
                        dogToggle.transform.SetParent(dogWalkingToggle.transform);
                        dogToggle.transform.localScale = new Vector3(1f, 1f, 1f);
                        dogToggle.transform.SetSiblingIndex(0);
                        dogWalkingToggle.GetComponentInChildren<TextMeshProUGUI>().text = currentDog.name;

                        lobbyCanvas.transform.parent.GetComponent<LobbyCanvasManager>().addNewDog(currentDog);
                        if (currentDog.leaderboards_opt_in)
                            leaderboardsCanvas.GetComponent<LeaderboardCanvasManager>().addNewDog(currentDog);
                        GameManager.Instance.walkingDogs.Add(currentDog);
                    }
                    else
                    {
                        lobbyCanvas.transform.parent.GetComponent<LobbyCanvasManager>().updateDog(currentDog, false);
                        leaderboardsCanvas.GetComponent<LeaderboardCanvasManager>().updateDog(currentDog);
                    }

                    break;
                }
            }
        }


    }

    public async Task reloadMainMenuUI()
    {
        await GameManager.Instance.load();
    }


    private void addCheckpoint()
    {
        int number = int.Parse(checkpointGoalText.text);
        number += 1;
        checkpointGoalText.text = number.ToString();
    }

    private void subtractCheckpoint()
    {
        int number = int.Parse(checkpointGoalText.text);
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

        // PLay Audio

        GameManager.Instance.PlayAudio("dogToggleMenu");
    }

    public void onNextButtonPressed()
    {
        if (currentDog.isNew)
        {
            StartCoroutine(StartBeginAdventure());
            GameManager.Instance.PlayAudio("winSound");
        }
        else
        {
            selectDestinationCanvas.SetActive(true);
            GameManager.Instance.PlayAudio("nextSound");
        }
    }

    IEnumerator StartBeginAdventure()
    {
        beginAdventureVideoPlayer.GetComponent<RawImage>().enabled = true;
        beginAdventureVideoPlayer.GetComponent<VideoPlayer>().Play();
        beginAdventureDialogue.SetActive(true);

        yield return new WaitForSeconds(2);

        beginAdventureButton.interactable = true;

        beginAdventureButton.onClick.AddListener(delegate
        {
            beginAdventureVideoPlayer.GetComponent<RawImage>().enabled = false;
            beginAdventureVideoPlayer.GetComponent<VideoPlayer>().Stop();
            beginAdventureVideoPlayer.GetComponent<VideoPlayer>().Prepare();
            beginAdventureDialogue.SetActive(false);
            beginAdventureButton.interactable = false;
            
            selectDestinationCanvas.SetActive(true);
        });

        yield return null;
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

        if (currentDog.isNew)
        {
            currentDog.SetCheckpoints(checkpointGoalText);
            currentDog.initializeData();
            GameManager.Instance.walkingDogs.Add(currentDog);
            GameObject dogWalkingToggle = Instantiate(dogWalkingTogglePrefab, walkingDogs);
            currentDog.gameObject.transform.SetParent(dogWalkingToggle.transform);
            currentDog.gameObject.transform.SetSiblingIndex(0);
            dogWalkingToggle.GetComponentInChildren<TextMeshProUGUI>().text = currentDog.name;


            lobbyCanvas.transform.parent.GetComponent<LobbyCanvasManager>().addNewDog(currentDog);
            if (currentDog.leaderboards_opt_in)
                leaderboardsCanvas.GetComponent<LeaderboardCanvasManager>().addNewDog(currentDog);

        }
        else
        {
            lobbyCanvas.transform.parent.GetComponent<LobbyCanvasManager>().updateDog(currentDog, true);
            if (currentDog.leaderboards_opt_in)
                leaderboardsCanvas.GetComponent<LeaderboardCanvasManager>().updateDog(currentDog);

        }

        GameManager.Instance.addWalkingDogToDB(currentDog);

        StartCoroutine(StartPassportAnimation());
        

    }

    IEnumerator StartPassportAnimation()
    {
        
        passportVideoPlayer.GetComponent<RawImage>().enabled = true;
        passportVideoPlayer.GetComponent<VideoPlayer>().Play();
        startButton.gameObject.SetActive(true);

        //passportCanvas.SetActive(true);
        

        selectDestinationCanvas.SetActive(false);
        switch (currentDog.country)
        {
            case "Egypt":
                passportCanvasEgypt.SetActive(true);
                break;
            case "France":
                passportCanvasFrance.SetActive(true);
                break;
            case "Japan":
                passportCanvasJapan.SetActive(true);
                break;
        }

        yield return new WaitForSeconds(8);

        mainCanvas.SetActive(false);

        startButton.interactable = true;

        // Show fun fact popup
        startButton.onClick.AddListener(delegate
        {
            if (currentDog == null)
                return;
            GameManager.Instance.ActivateFactDialogue(currentDog);
            GameManager.Instance.PlayAudio(currentDog.country);
            resetUI();
            

            passportVideoPlayer.GetComponent<RawImage>().enabled = false;
            passportVideoPlayer.GetComponent<VideoPlayer>().Stop();
            passportVideoPlayer.GetComponent<VideoPlayer>().Prepare();
            startButton.gameObject.SetActive(false);
            startButton.interactable = false;
        });

        
        yield return null;
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
