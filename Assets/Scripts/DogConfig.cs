using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DogConfig", menuName = "DogConfig/NewDogConfig")]
public class DogConfig : ScriptableObject
{
    public string id;
    public string name;
    public Sprite thumbnail;

}
