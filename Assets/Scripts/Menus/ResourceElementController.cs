using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceElementController : MonoBehaviour
{
    enum TypeOfResource {Food, Stone, Wood};

    [SerializeField] private TypeOfResource typeOfResource;

    public void UpdateElement()
    {
        // TODO: Add junction with where is store the number of resource of each typeOfResource
    }
}