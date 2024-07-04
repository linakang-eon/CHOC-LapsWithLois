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
    // Start is called before the first frame update
    void Start()
    {
        List<Dog> walkingDogsList = GameManager.Instance.walkingDogs;
        foreach (Dog dog in walkingDogsList)
        {
            addNewDog(dog);
        }

        

        checkpointButton.GetComponent<Button>().onClick.AddListener(delegate {
            selectedDog.GetComponent<DogProfileToggle>().dog.checkpoint(); 
            congratulatoryImage.SetActive(true);
            
            Transform nextCity = currentCountry.transform.GetChild(selectedDog.GetComponent<DogProfileToggle>().dog.cityIndex);
            if (nextCity == null)
            {
                Debug.Log("USer has finished their lap");
            }
            else
            {
                currentCountry.transform.GetChild(selectedDog.GetComponent<DogProfileToggle>().dog.cityIndex - 1).gameObject.SetActive(false);
                nextCity.gameObject.SetActive(true);
            }
            checkpointButton.SetActive(false);
            //Change to next scene within Destination
        });
    }

    public void onToggled(GameObject DogProfileToggle)
    {
        selectedDog = DogProfileToggle;

        checkpointButton.SetActive(selectedDog.GetComponent<Toggle>().isOn);

        string destination = selectedDog.GetComponent<DogProfileToggle>().dog.country;

        Transform currentDestination = backgroundAnimation.transform.Find(destination);

        if (currentCountry != currentDestination.gameObject)
        {
            currentCountry.SetActive(false);
            currentCountry = currentDestination.gameObject;
            currentCountry.SetActive(true);
            currentCountry.transform.GetChild(selectedDog.GetComponent<DogProfileToggle>().dog.cityIndex).gameObject.SetActive(true);

        }
        
    }

    public void addNewDog(Dog currentDog)
    {
        GameObject dogLobbyToggle = Instantiate(dogProfilePrefab, dogProfileIcons);
        dogLobbyToggle.GetComponent<DogProfileToggle>().SetDog(currentDog);
        dogLobbyToggle.GetComponent<Toggle>().group = dogProfileIcons.GetComponent<ToggleGroup>();
        dogLobbyToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate
        {
            onToggled(dogLobbyToggle);
        });
    }
}
