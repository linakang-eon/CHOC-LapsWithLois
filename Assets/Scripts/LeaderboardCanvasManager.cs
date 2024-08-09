using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardCanvasManager : MonoBehaviour
{
    //[Header("")]

    public GameObject TopThreePanel;
    public GameObject Bronze;
    public GameObject Silver;
    public GameObject Gold;
    public GameObject NoDogsDialogue;

    public Transform CurrentWalkingDogsTransform;

    public GameObject CheckpointPrefab;

    private List<Dog> sortedWalkingDogs = new List<Dog>();
    private List<GameObject> sortedWalkingDogsGameObjects = new List<GameObject>();

    public void InitializeLeaderboards()
    {
        sort(GameManager.Instance.walkingDogs);


        if (sortedWalkingDogs.Count > 0)
        {
            Gold.transform.Find("Image").GetComponent<Image>().sprite = sortedWalkingDogs[0].thumbnail;
            Gold.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = sortedWalkingDogs[0].checkpointsDone.ToString();
            Gold.SetActive(true);
            if(sortedWalkingDogs.Count > 1)
            {
                Silver.transform.Find("Image").GetComponent<Image>().sprite = sortedWalkingDogs[1].thumbnail;
                Silver.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = sortedWalkingDogs[1].checkpointsDone.ToString();
                Silver.SetActive(true);
                if(sortedWalkingDogs.Count > 2)
                {
                    Bronze.transform.Find("Image").GetComponent<Image>().sprite = sortedWalkingDogs[2].thumbnail;
                    Bronze.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = sortedWalkingDogs[2].checkpointsDone.ToString();
                    Bronze.SetActive(true);

                    if(sortedWalkingDogs.Count > 3)
                    {

                        for (int i = 3; i < sortedWalkingDogs.Count; i++)
                        {
                            GameObject dogPrefab = Instantiate(CheckpointPrefab, CurrentWalkingDogsTransform);
                            dogPrefab.GetComponent<Dog>().SetDog(sortedWalkingDogs[i]);
                            dogPrefab.transform.Find("Image").GetComponent<Image>().sprite = sortedWalkingDogs[i].thumbnail;
                            dogPrefab.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = sortedWalkingDogs[i].checkpointsDone.ToString();
                        }

                    }

                }

            }
        }
        else
        {
            Debug.Log("There are no walking dogs to show in leaderboards");
        }


    }

    public void addNewDog(Dog dog)
    {
        bool added = false;
        for (int i = 0; i < sortedWalkingDogs.Count; i++)
        {
            if (dog.checkpointsDone > sortedWalkingDogs[i].checkpointsDone)
            {
                sortedWalkingDogs.Insert(i, dog);
                added = true;
                break;
            }
        }
        if(!added)
        {
            sortedWalkingDogs.Add(dog);
        }

        

        //based on how many checkpoints done by this dog, put him in Gold, Silver, Bronze, or new dog

        if (sortedWalkingDogs.Count > 0)
        {
            Gold.transform.Find("Image").GetComponent<Image>().sprite = sortedWalkingDogs[0].thumbnail;
            Gold.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = sortedWalkingDogs[0].checkpointsDone.ToString();
            Gold.SetActive(true);
            if (sortedWalkingDogs.Count > 1)
            {
                Silver.transform.Find("Image").GetComponent<Image>().sprite = sortedWalkingDogs[1].thumbnail;
                Silver.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = sortedWalkingDogs[1].checkpointsDone.ToString();
                Silver.SetActive(true);
                if (sortedWalkingDogs.Count > 2)
                {
                    Bronze.transform.Find("Image").GetComponent<Image>().sprite = sortedWalkingDogs[2].thumbnail;
                    Bronze.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = sortedWalkingDogs[2].checkpointsDone.ToString();
                    Bronze.SetActive(true);

                    if (sortedWalkingDogs.Count > 3)
                    {
                        GameObject dogPrefab = Instantiate(CheckpointPrefab, CurrentWalkingDogsTransform);
                        dogPrefab.GetComponent<Dog>().SetDog(dog);
                        dogPrefab.transform.Find("Image").GetComponent<Image>().sprite = dog.thumbnail;
                        dogPrefab.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = dog.checkpointsDone.ToString();
                        sortedWalkingDogsGameObjects.Add(dogPrefab);
                    }

                }

            }
        }
    }

    internal void scorchedEarth()
    {
        foreach(GameObject dog in sortedWalkingDogsGameObjects)
        {
            Destroy(dog);

        }
        Gold.SetActive(false);
        Bronze.SetActive(false);
        Silver.SetActive(false);
    }

    internal void sort(List<Dog> walkingDogs)
    {
        sortedWalkingDogs = new List<Dog>();

        foreach(Dog dog in walkingDogs)
        {
            for(int i = 0; i < sortedWalkingDogs.Count; i++)
            {
                if(dog.checkpointsDone > sortedWalkingDogs[i].checkpointsDone)
                {
                    sortedWalkingDogs.Insert(i, dog);
                    break;
                }
            }
        }




    }

    internal void updateDog(Dog dog)
    {
        sortedWalkingDogs.Remove(dog);

        if (dog.leaderboards_opt_in)
        {
            int i;
            bool added = false;
            for (i = 0; i < sortedWalkingDogs.Count; i++)
            {
                if (dog.checkpointsDone > sortedWalkingDogs[i].checkpointsDone)
                {
                    sortedWalkingDogs.Insert(i, dog);
                    added = true;
                    break;
                }
            }
            if (!added)
                sortedWalkingDogs.Add(dog);
        }


        // 3 2 1 
        // 
        // index = 2 

        if (sortedWalkingDogs.Count > 0)
        {
            Gold.transform.Find("Image").GetComponent<Image>().sprite = sortedWalkingDogs[0].thumbnail;
            Gold.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = sortedWalkingDogs[0].checkpointsDone.ToString();
            Gold.SetActive(true);
            if (sortedWalkingDogs.Count > 1)
            {
                Silver.transform.Find("Image").GetComponent<Image>().sprite = sortedWalkingDogs[1].thumbnail;
                Silver.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = sortedWalkingDogs[1].checkpointsDone.ToString();
                Silver.SetActive(true);
                if (sortedWalkingDogs.Count > 2)
                {
                    Bronze.transform.Find("Image").GetComponent<Image>().sprite = sortedWalkingDogs[2].thumbnail;
                    Bronze.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = sortedWalkingDogs[2].checkpointsDone.ToString();
                    Bronze.SetActive(true);

                    if (sortedWalkingDogs.Count > 3)
                    {
                        foreach (GameObject dogGO in sortedWalkingDogsGameObjects)
                        {
                            Destroy(dogGO);

                        }
                        for (int j = 3; j < sortedWalkingDogs.Count; j++)
                        {
                            GameObject dogPrefab = Instantiate(CheckpointPrefab, CurrentWalkingDogsTransform);
                            dogPrefab.GetComponent<Dog>().SetDog(sortedWalkingDogs[j]);
                            dogPrefab.transform.Find("Image").GetComponent<Image>().sprite = sortedWalkingDogs[j].thumbnail;
                            dogPrefab.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = sortedWalkingDogs[j].checkpointsDone.ToString();
                            sortedWalkingDogsGameObjects.Add(dogPrefab);
                        }
                    }
                    else
                    {
                        foreach (GameObject dogGO in sortedWalkingDogsGameObjects)
                        {
                            Destroy(dogGO);

                        }
                    }

                }
                else
                {
                    Bronze.SetActive(false);
                    foreach (GameObject dogGO in sortedWalkingDogsGameObjects)
                    {
                        Destroy(dogGO);

                    }
                }

            }
            else
            {
                Silver.SetActive(false);
                Bronze.SetActive(false);
                foreach (GameObject dogGO in sortedWalkingDogsGameObjects)
                {
                    Destroy(dogGO);

                }
            }
        }
    }

    public void openLeaderboardCanvas()
    {
        if(sortedWalkingDogs.Count == 0)
        {
            TopThreePanel.SetActive(false);
            CurrentWalkingDogsTransform.gameObject.SetActive(false);
            NoDogsDialogue.SetActive(true);
        }
        else
        {
            TopThreePanel.SetActive(true);
            CurrentWalkingDogsTransform.gameObject.SetActive(true);
            NoDogsDialogue.SetActive(false);
        }

        gameObject.SetActive(true);
    }
}
