using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Vector3 openPosition = new Vector3(0, 3, 0); // edit this
    public float openSpeed = 2f;
    private Vector3 closedPosition;
    private bool shouldOpen = false;

    void Start()
    {
        closedPosition = transform.position;
    }

    void Update()
    {
        if (shouldOpen)
        {
            // Smoothly move the door to open position
            transform.position = Vector3.Lerp(transform.position, closedPosition + openPosition, Time.deltaTime * openSpeed);
        }
        else
        {
            // Smoothly move the door to closed position
            transform.position = Vector3.Lerp(transform.position, closedPosition, Time.deltaTime * openSpeed);
        }
    }

    public void OpenDoor()
    {
        shouldOpen = true;
    }

    public void CloseDoor()
    {
        shouldOpen = false;
    }
}