using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<Dog> walkingDogs;
    public List<Dog> availableDogs;

    public List<GameObject> regions;

    private void Awake()
    {
        Instance = this;
    }

}

