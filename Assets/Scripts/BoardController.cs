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
                // Debug.Log("Attitude Sensor enabled: " + attitudeSensor.enabled);
            }

        }
    }

    // when using attitude sensor (either with Unity Remote or full build on phone)
    void attitudeSensorControl() {

        // try to activate attitude sensor first
        tryToActivate();

        if (attitudeSensor != null && attitudeSensor.enabled)
        {

            // get rotation from attitude
            Quaternion attitude = attitudeSensor.attitude.ReadValue();
            Vector3 angles = attitude.eulerAngles;
            
            // convert to [-180, 180] range per standard
            if (angles.x > 180f) angles.x -= 360f;
            if (angles.y > 180f) angles.y -= 360f;
            if (angles.z > 180f) angles.z -= 360f;

            angles.z -= 90f; // Z rot is 90 by default since phone is flat

            // make sure user cannot tilt too far
            float tiltX = Mathf.Clamp(-angles.x, -maxRotation, maxRotation);
            float tiltZ = Mathf.Clamp(-angles.y, -maxRotation, maxRotation);

            // now convert tilt angles to gravity dir
            Vector3 gravityDirection = new Vector3(
                Mathf.Sin(Mathf.Deg2Rad * tiltX),
                -1f,
                Mathf.Sin(Mathf.Deg2Rad * tiltZ)
            ).normalized;

            Physics.gravity = gravityDirection * gravityStrength;
        }
    }

    void Start()
    {   
        // attitude sensor
        tryToActivate();
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
