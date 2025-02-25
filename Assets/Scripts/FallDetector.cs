using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FallDetector : MonoBehaviour
{

    // just reload the entire level
    private void OnTriggerEnter(Collider other)
    {   
        resetLevel();
    }

    // making it public so other things like reset button can access it too
    public void resetLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartScreenReturn()
    {
        SceneManager.LoadScene(0);
    }
}