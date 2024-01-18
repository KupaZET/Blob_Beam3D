using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class duplicateObject : MonoBehaviour
{
    public GameObject objectToPlace;
    public int numberOfObjects = 10;
    public Vector3 areaSize = new Vector3(10, 0, 10);

    void Start()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            PlaceRandomObject();
        }
    }

    void PlaceRandomObject()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-areaSize.x / 2, areaSize.x / 2),
            0,
            Random.Range(-areaSize.z / 2, areaSize.z / 2)
        ) + transform.position;

        Quaternion randomRotation = Quaternion.Euler(
            0,
            Random.Range(0, 360),
            0
        );

        GameObject newObject = Instantiate(objectToPlace, randomPosition, randomRotation);

        // Add a Box Collider to the new object if it doesn't already have one
        if (newObject.GetComponent<BoxCollider>() == null)
        {
            newObject.AddComponent<BoxCollider>();
        }
    }
}
