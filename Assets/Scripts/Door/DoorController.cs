using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float openAngle = 90f; // Angle to rotate when opening
    public float openSpeed = 2f;
    private Quaternion closedRotation;
    private bool shouldOpen = false;

    void Start()
    {
        closedRotation = transform.rotation;
    }

    void Update()
    {
        if (shouldOpen)
        {
            // Smoothly rotate the door around the Y axis
            Quaternion targetRotation = closedRotation * Quaternion.Euler(0, -openAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
        }
        else
        {
            // Smoothly rotate back to closed position
            transform.rotation = Quaternion.Lerp(transform.rotation, closedRotation, Time.deltaTime * openSpeed);
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