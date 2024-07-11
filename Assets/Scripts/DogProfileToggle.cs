using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DogProfileToggle : MonoBehaviour
{
    public Dog dog;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetDog(Dog dog)
    {
        this.dog = dog;
        gameObject.name = dog.name;
        gameObject.GetComponent<Image>().sprite = dog.thumbnail;
        
    }


}
