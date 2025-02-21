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

    private float gravityStrength = 9.81f;

    // in case we are using unityRemote
    public bool usingPhone = false;

    // try to activate the attitude sensor
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

    // when using attitude sensor (either with Unity Remote or full build on phone)
    void attitudeSensorControl() {

        // try to activate attitude sensor first
        tryToActivate();

        if (attitudeSensor != null && attitudeSensor.enabled)
        {
        
            // // Get the phone's orientation
            // Quaternion attitude = attitudeSensor.attitude.ReadValue();

            // Debug.Log($"Raw attitude: {attitude.x:F3}, {attitude.y:F3}, {attitude.z:F3}, {attitude.w:F3}");

            // // Convert quaternion to euler angles directly
            // Vector3 angles = attitude.eulerAngles;
        
            // // Convert angles from [0,360] to [-180,180] range
            // // Same as the manual case in Unity editor
            // if (angles.x > 180f) angles.x -= 360f;
            // if (angles.y > 180f) angles.y -= 360f;
            // if (angles.z > 180f) angles.z -= 360f;

            // angles.z -= 90f; // by default, Z is 90 for some reason

            // // Clamp rotation to our maximum values

            // // X: forward/back in Unity frame of ref
            // // Z: left/right
            // float tiltX = Mathf.Clamp(-angles.y, -maxRotation, maxRotation); // angles.x
            // float tiltZ = Mathf.Clamp(angles.x, -maxRotation, maxRotation); // angles.z
            
            // // Apply the rotation (keeping Y rotation at 0)
            // transform.localEulerAngles = new Vector3(tiltX, 0, tiltZ);
            
            // // Add this debug line to see the values while testing
            // Debug.Log($"Angles - X: {angles.x:F1}°, Y: {angles.y:F1}°, Z: {angles.z:F1}°");

            Quaternion attitude = attitudeSensor.attitude.ReadValue();
            Vector3 angles = attitude.eulerAngles;
            
            // Convert to [-180, 180] range
            if (angles.x > 180f) angles.x -= 360f;
            if (angles.y > 180f) angles.y -= 360f;
            if (angles.z > 180f) angles.z -= 360f;

            angles.z -= 90f; // Z rot is 90 by default since phone is flat

            // Clamp the angles
            float tiltX = Mathf.Clamp(-angles.x, -maxRotation, maxRotation); // -angles.y
            float tiltZ = Mathf.Clamp(-angles.y, -maxRotation, maxRotation);

            // Convert tilt angles to gravity direction
            Vector3 gravityDirection = new Vector3(
                Mathf.Sin(Mathf.Deg2Rad * tiltX),
                -1f,
                Mathf.Sin(Mathf.Deg2Rad * tiltZ)
            ).normalized;

            // Apply gravity
            Physics.gravity = gravityDirection * gravityStrength;
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
        #if UNITY_EDITOR

        // using WASD on laptop
        if (!usingPhone) {
            float tiltZ = Input.GetAxis("Vertical") * maxRotation;
            float tiltX = Input.GetAxis("Horizontal") * maxRotation;

            // tilt angle -> gravity dir
            Vector3 gravityDirection = new Vector3(
                Mathf.Sin(Mathf.Deg2Rad * tiltX),
                -1f,
                Mathf.Sin(Mathf.Deg2Rad * tiltZ)
            ).normalized; // normalize it so we can scale by gravity mag

            Physics.gravity = gravityDirection * gravityStrength;
        }

        // using UnityRemote and attitude sensor
        else {
            attitudeSensorControl();
        }

        /******* built on phone!!!! *******/
        #else
            attitudeSensorControl();
        #endif


    }
}
