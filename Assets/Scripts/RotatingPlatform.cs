using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public Transform orbitingPlatform;
    public float orbitRadius = 2f; // Add this in inspector
    
    private float currentAngle;
    private Quaternion initialPlatformRotation;

    void Start()
    {
        // Store initial configuration
        initialPlatformRotation = orbitingPlatform.rotation;
        currentAngle = Vector3.SignedAngle(
            Vector3.forward,
            orbitingPlatform.position - transform.position,
            Vector3.up
        );
    }

    void Update()
    {
        // Rotate central disc
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        
        // Update orbiting platform
        if (orbitingPlatform != null)
        {
            // Calculate new position
            currentAngle += rotationSpeed * Time.deltaTime;
            float rad = currentAngle * Mathf.Deg2Rad;
            
            Vector3 offset = new Vector3(
                Mathf.Sin(rad), 
                0, 
                Mathf.Cos(rad)
            ) * orbitRadius;
            
            orbitingPlatform.position = transform.position + offset;
            
            // Maintain original rotation
            orbitingPlatform.rotation = initialPlatformRotation;
        }
    }
}


// public class RotatingPlatform : MonoBehaviour
// {
//     public float rotationSpeed = 30f; // deg / sec
//     public Transform orbitingPlatform; // this is the platform
    
//     void Update()
//     {
//         // rotates the central disc
//         transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        
//         // make sure to keep orbiting platform's orientation fixed, while rotating position only
//         if (orbitingPlatform != null)
//         {
//             orbitingPlatform.RotateAround(
//                 transform.position, // this is center disc
//                 Vector3.up, // y axis
//                 rotationSpeed * Time.deltaTime // rotation amount
//             );
            
//             // the orientation is fixed (relative to world space)
//             orbitingPlatform.rotation = Quaternion.Euler(0, 0, 0);
//         }
//     }
// }