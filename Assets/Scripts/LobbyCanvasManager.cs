using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCanvasManager : MonoBehaviour
{
    public GameObject backgroundAnimation;

    public GameObject selectedDog;
    public GameObject currentCountry;

    public GameObject congratulatoryImage;

    public GameObject checkpointButton;

    public GameObject dogProfilePrefab;
    public Transform dogProfileIcons;


    void Start()
    {
        

        checkpointButton.GetComponent<Button>().onClick.AddListener(delegate 
        {
            
            selectedDog.GetComponent<Dog>().checkpoint(); 
            congratulatoryImage.SetActive(true);
            checkpointButton.SetActive(false);

        });
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

        checkpointButton.SetActive(selectedDog.GetComponent<Toggle>().isOn);

        string destination = selectedDog.GetComponent<Dog>().country;

        Transform currentDestination = backgroundAnimation.transform.Find(destination);

        if (currentCountry != currentDestination.gameObject)
        {
            currentCountry.SetActive(false);
            currentCountry = currentDestination.gameObject;
            currentCountry.SetActive(true);
            //currentCountry.transform.GetChild(selectedDog.GetComponent<Dog>().cityIndex).gameObject.SetActive(true);

        }
        
    }

    public void addNewDog(Dog currentDog)
    {
        GameObject dogLobbyToggle = Instantiate(dogProfilePrefab, dogProfileIcons);
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
