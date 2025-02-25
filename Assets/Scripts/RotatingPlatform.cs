using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public Transform orbitingPlatform;
    public float orbitRadius = 2f;
    
    private float currentAngle;
    private Quaternion initialPlatformRotation;

    void Start()
    {
        // init setup
        initialPlatformRotation = orbitingPlatform.rotation;
        
        currentAngle = Vector3.SignedAngle(
            Vector3.forward,
            orbitingPlatform.position - transform.position,
            Vector3.up
        );
    }

    void Update()
    {
        // rotate central disc first
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        
        if (orbitingPlatform != null)
        {
            // new pos
            currentAngle += rotationSpeed * Time.deltaTime;
            float rad = currentAngle * Mathf.Deg2Rad;
            
            Vector3 offset = new Vector3(
                Mathf.Sin(rad), 
                0, 
                Mathf.Cos(rad)
            ) * orbitRadius;
            
            orbitingPlatform.position = transform.position + offset;
            
            // keep original rot
            orbitingPlatform.rotation = initialPlatformRotation;
        }
    }
}