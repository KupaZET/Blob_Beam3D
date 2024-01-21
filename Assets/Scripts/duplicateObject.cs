using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerMovement;

public class duplicateObject : MonoBehaviour
{
    public GameObject objectToPlace;
    public RuntimeAnimatorController controller;
    public int numberOfObjects = 10;
    public Vector3 areaSize = new Vector3(10, 0, 10);
    public bool isFirst = true;

    void Start()
    {
        if (isFirst) 
        {
            for (int i = 0; i < numberOfObjects; i++)
            {
                PlaceRandomObject();
            }
        }

        Vector3[] rayDirections = {
            transform.forward,         // forward
            transform.forward + transform.right,    // forward-right
            transform.right,           // right
            transform.right - transform.forward,   // back-right
            -transform.forward,        // back
            -transform.forward - transform.right, // back-left
            -transform.right,           // left
            -transform.right + transform.forward   // forward-left
        };

        foreach (Vector3 direction in rayDirections)
        {
            Ray ray = new Ray(transform.position, direction);

            // Perform the raycast
            if (Physics.Raycast(ray, 3))
            {
                Destroy(gameObject);
            }
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

        if (newObject.GetComponent<Animator>() == null)
        {
            Animator animator = newObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
        }

        if (newObject.GetComponent<duplicateObject>() == null)
        {
            var newScript = newObject.AddComponent<duplicateObject>();
            newScript.objectToPlace = objectToPlace;
            newScript.areaSize = areaSize;
            newScript.isFirst = false;
        }
    }
}
