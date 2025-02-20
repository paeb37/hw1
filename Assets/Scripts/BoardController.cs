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

    // use courotine in case it takes a bit to activate
    IEnumerator EnableSensorCoroutine()
    {
        float timeout = 10f; // Maximum seconds to wait
        float elapsed = 0f;
        
        while (elapsed < timeout)
        {
            // Try to get the sensor if we don't have it yet
            if (attitudeSensor == null)
            {
                attitudeSensor = AttitudeSensor.current;
                if (attitudeSensor == null)
                {
                    Debug.Log("Waiting for attitude sensor to become available...");
                    yield return new WaitForSeconds(0.5f);
                    elapsed += 0.5f;
                    continue;
                }
            }

            // Try to enable the sensor
            InputSystem.EnableDevice(attitudeSensor);
            yield return new WaitForSeconds(0.1f);
            
            if (attitudeSensor.enabled)
            {
                Debug.Log("Attitude Sensor successfully enabled!");
                yield break;
            }
            
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.6f;
        }
        
        Debug.LogError("Failed to enable Attitude Sensor - timeout reached");
    }

    // try to activate the attitude sensor repeatedly
    // for unity remote testing purposes
    void tryToActivate() {
        if (attitudeSensor == null || !attitudeSensor.enabled) {

            attitudeSensor = AttitudeSensor.current;

            if (attitudeSensor != null) {
                InputSystem.EnableDevice(attitudeSensor);
                
                // I confirmed this part works, so it is enabled
                Debug.Log("Attitude Sensor enabled: " + attitudeSensor.enabled);
            }

        }
    }

    void Start()
    {   
        // force landscape mode for the maze
        // so that phone doesn't rotate and make it portrait
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        // Force landscape mode
        // Screen.orientation = ScreenOrientation.LandscapeLeft;

        // Start the coroutine that will keep trying to enable the sensor
        // StartCoroutine(EnableSensorCoroutine());

        tryToActivate();
        
        // if (attitudeSensor != null)
        // {
        //     InputSystem.EnableDevice(attitudeSensor);
        //     // I confirmed this part works, so it is enabled
        //     Debug.Log("Attitude Sensor enabled: " + attitudeSensor.enabled);
        // }
        // else
        // {
        //     Debug.LogWarning("AttitudeSensor not found");
        // }
    }


    void Update()
    {   
        // editor: use WASD to rotate maze
        // phone: rotate physically

        /******* using editor!!!! *******/
        // #if UNITY_EDITOR

        // Debug.LogWarning("DEVICE: Unity Editor! Bad");

        // float rotationX = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;
        // float rotationZ = -Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        // Vector3 currentRotation = transform.localEulerAngles;
        
        // // Convert the range to [-180, 180]
        // if (currentRotation.x > 180f) currentRotation.x -= 360f;
        // if (currentRotation.z > 180f) currentRotation.z -= 360f;

        // float newRotationX = Mathf.Clamp(currentRotation.x + rotationX, -maxRotation, maxRotation);
        // float newRotationZ = Mathf.Clamp(currentRotation.z + rotationZ, -maxRotation, maxRotation);

        // // note: currentRotation.y will just be 0 always
        // transform.localEulerAngles = new Vector3(newRotationX, currentRotation.y, newRotationZ);
        
        // /******* using phone!!!! *******/
        // #else

        // Debug.LogWarning("DEVICE: IPHONE! Good");

        // try to activate attitude sensor
        tryToActivate();

        // if (attitudeSensor != null)
        // {
        //     // Debug sensor state
        //     Debug.Log($"Sensor enabled: {attitudeSensor.enabled}");
        //     // Rest of your code...
        // }

        if (attitudeSensor != null && attitudeSensor.enabled)
        {

            // // Get the phone's orientation
            // Quaternion attitude = attitudeSensor.attitude.ReadValue();
            
            // // Apply iOS-specific rotation compensation
            // attitude = new Quaternion(attitude.x, attitude.y, -attitude.z, -attitude.w);
            
            // // Convert to euler angles
            // Vector3 angles = attitude.eulerAngles;
            
            // // Convert angles from [0,360] to [-180,180] range
            // if (angles.x > 180f) angles.x -= 360f;
            // if (angles.z > 180f) angles.z -= 360f;

            // // angles.z -= 90f; // for some reason it starts with Z = 90

            // // make sure it does not exceed 30
            // float tiltX = Mathf.Clamp(angles.x, -maxRotation, maxRotation);
            // float tiltZ = Mathf.Clamp(angles.z, -maxRotation, maxRotation);
            
            // // Apply the rotation (keeping Y rotation at 0)
            // transform.localEulerAngles = new Vector3(tiltX, 0, tiltZ);
            
            // // Debug output
            // Debug.Log($"Raw attitude: {attitude.x:F3}, {attitude.y:F3}, {attitude.z:F3}, {attitude.w:F3}");
            // Debug.Log($"Angles - X: {angles.x:F1}°, Y: {angles.y:F1}°, Z: {angles.z:F1}°");

        // Debug sensor state
        // Debug.Log($"Sensor enabled: {attitudeSensor.enabled}");
        
        // Get the phone's orientation
        Quaternion attitude = attitudeSensor.attitude.ReadValue();

        Debug.Log($"Raw attitude: {attitude.x:F3}, {attitude.y:F3}, {attitude.z:F3}, {attitude.w:F3}");

        // Convert quaternion to euler angles directly
        Vector3 angles = attitude.eulerAngles;
    
        // Convert angles from [0,360] to [-180,180] range
        // Same as the manual case in Unity editor
        if (angles.x > 180f) angles.x -= 360f;
        if (angles.y > 180f) angles.y -= 360f;
        if (angles.z > 180f) angles.z -= 360f;

        angles.z -= 90f; // by default, Z is 90 for some reason

        // Clamp rotation to our maximum values

        // X: forward/back in Unity frame of ref
        // Z: left/right
        float tiltX = Mathf.Clamp(-angles.y, -maxRotation, maxRotation); // angles.x
        float tiltZ = Mathf.Clamp(angles.x, -maxRotation, maxRotation); // angles.z
        
        // Apply the rotation (keeping Y rotation at 0)
        transform.localEulerAngles = new Vector3(tiltX, 0, tiltZ);
        
        // Add this debug line to see the values while testing
        Debug.Log($"Angles - X: {angles.x:F1}°, Y: {angles.y:F1}°, Z: {angles.z:F1}°");

        }

        // #endif
    }
}
