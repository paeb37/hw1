using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDetector : MonoBehaviour
{
    public Vector3 resetPosition; // set this in the Inspector to the top left

    // for reseting the board as well
    public GameObject board;
    private Quaternion initRotation;

    private void OnTriggerEnter(Collider other)
    {   
        if (board != null)
        {
            initRotation = board.transform.rotation;
        }

        // anything that falls through
        ResetObjectPosition(other.gameObject);
    }

    private void ResetObjectPosition(GameObject fallenObject)
    {   
        // basically reset all fields
        
        fallenObject.transform.position = resetPosition;
        fallenObject.transform.rotation = Quaternion.identity; // this will reset rotation to default

        Rigidbody rb = fallenObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Reset the board to its initial rotation
        if (board != null)
        {
            board.transform.rotation = initRotation; // should just be easy
            
            // if rigidbody exists for board
            Rigidbody boardRb = board.GetComponent<Rigidbody>();
            if (boardRb != null)
            {
                boardRb.velocity = Vector3.zero;
                boardRb.angularVelocity = Vector3.zero;
            }
        }


    }
}