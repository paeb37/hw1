using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDetector : MonoBehaviour
{
    public Vector3 resetPosition; // set this in the Inspector

    private void OnTriggerEnter(Collider other)
    {
        // anything that falls through
        ResetObjectPosition(other.gameObject);
    }

    private void ResetObjectPosition(GameObject fallenObject)
    {   
        // basically reset all fields
        
        fallenObject.transform.position = resetPosition;
        Rigidbody rb = fallenObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}