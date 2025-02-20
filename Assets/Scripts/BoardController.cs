using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class BoardController : MonoBehaviour
{
    public float rotationSpeed; // set in inspector
    public float maxRotation; // set in inspector

    private AttitudeSensor attitudeSensor;
    // private Quaternion initialRotation;

    void Start()
    {   
        // Force landscape mode
        // Screen.orientation = ScreenOrientation.LandscapeLeft;

        // Get the AttitudeSensor
        attitudeSensor = AttitudeSensor.current;
        if (attitudeSensor != null)
        {
            InputSystem.EnableDevice(attitudeSensor);
            // I confirmed this part works, so it is enabled
            Debug.Log("Attitude Sensor enabled: " + attitudeSensor.enabled);
        }
        else
        {
            Debug.LogWarning("AttitudeSensor not found");
        }
    }


    void Update()
    {   
        // editor: use WASD to rotate maze
        // phone: rotate physically

        /******* using editor!!!! *******/
        #if UNITY_EDITOR

        float rotationX = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;
        float rotationZ = -Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        Vector3 currentRotation = transform.localEulerAngles;
        
        // Convert the range to [-180, 180]
        if (currentRotation.x > 180f) currentRotation.x -= 360f;
        if (currentRotation.z > 180f) currentRotation.z -= 360f;

        float newRotationX = Mathf.Clamp(currentRotation.x + rotationX, -maxRotation, maxRotation);
        float newRotationZ = Mathf.Clamp(currentRotation.z + rotationZ, -maxRotation, maxRotation);

        // note: currentRotation.y will just be 0 always
        transform.localEulerAngles = new Vector3(newRotationX, currentRotation.y, newRotationZ);
        
        /******* using phone!!!! *******/
        #else

        // Debug.LogWarning("ENTERED THE CORRECT PLACE HEHE!");

        if (attitudeSensor != null)
        {
        
        // Get the phone's orientation
        Quaternion attitude = attitudeSensor.attitude.ReadValue();

        // Convert quaternion to euler angles directly
        Vector3 angles = attitude.eulerAngles;

        // Convert angles from [0,360] to [-180,180] range
        // Same as the manual case in Unity editor
        if (angles.x > 180f) angles.x -= 360f;
        if (angles.z > 180f) angles.z -= 360f;

        // Clamp rotation to our maximum values
        float tiltX = Mathf.Clamp(angles.x, -maxRotation, maxRotation);
        float tiltZ = Mathf.Clamp(angles.z, -maxRotation, maxRotation);

        // Apply the rotation (keeping Y rotation at 0)
        transform.localEulerAngles = new Vector3(tiltX, 0, tiltZ);
        
        // Add this debug line to see the values while testing
        Debug.Log($"Angles - X: {angles.x:F1}°, Z: {angles.z:F1}°");
        }

        #endif
    }
}
