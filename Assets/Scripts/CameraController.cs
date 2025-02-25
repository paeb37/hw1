using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{   
    // should follow the player only on level 3

    public GameObject player;
    private Vector3 offset;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    
    // Start is called before the first frame update
    void Start()
    {
        // need to store init camera position and rot for L1, L2
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        // for l3
        offset = transform.position - player.transform.position;
    }

    // does this last, once per frame
    void LateUpdate()
    {
        int curSceneIdx = SceneManager.GetActiveScene().buildIndex;

        // Add debug log to check the scene index
        // Debug.Log("Current Scene: " + SceneManager.GetActiveScene().name + ", Index: " + curSceneIdx);
        
        if (curSceneIdx == 3)
        {
            // follow player in level 3 only
            transform.position = player.transform.position + offset;
        }
        else
        {
            // stays in fixed position for levels 1 and 2
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }
    }
}
