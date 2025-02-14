using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{

    // use this script to manage scenes / levels as well



    public GameObject player;
    private Vector3 offset;
 
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }

    public void PlayGame() {
        SceneManager.LoadSceneAsync(1); // for later
    }
}
