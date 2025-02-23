using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    public Canvas victoryCanvas;
    public TextMeshProUGUI completionTimeText;
    public TextMeshProUGUI collectiblesText;
    public Button nextLevelButton;
    public Button restartButton;
    public Button startMenuButton;

    // this is set once each new level starts
    // private float levelStartTime;

    // call this when we start a new level
    // public void StartTimer()
    // {
    //     levelStartTime = Time.time;
    // }

    void Start()
    {   
        victoryCanvas.enabled = false; // start as disabled

        // set up button listeners here in code, rather than unity editor
        restartButton.onClick.AddListener(RestartLevel);
        startMenuButton.onClick.AddListener(StartScreenReturn);
        nextLevelButton.onClick.AddListener(NextLevel);
    }

    // called from: Player (when collides with goal)
    // only need to pass in the collectible count
    public void ShowVictoryScreen(int numCollectibles)
    {   
        victoryCanvas.enabled = true;

        // use timer to measure the completion time
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            timer.StopTimer(); // stop counting time
            float completionTime = timer.GetElapsedTime();
            completionTimeText.text = $"Time: {Mathf.RoundToInt(completionTime)} seconds";
        }

        collectiblesText.text = $"Collectibles: {numCollectibles}";

        // need to make sure we're not on the last level
        int curSceneIdx = SceneManager.GetActiveScene().buildIndex;
        const int totalScenes = 4; // hard coded for now
        
        // not on last level
        if (curSceneIdx < totalScenes - 1) { // works for build idx 0, 1, 2
            nextLevelButton.gameObject.SetActive(true);
            GameData.UnlockedLevels++;
        }
        else {
            nextLevelButton.gameObject.SetActive(false);
        }

        // use level system controller to unlock next level
        // need this for data persistence
        // LevelSystem levelSystem = FindObjectOfType<LevelSystem>();
        
        // if (levelSystem != null)
        // {
        //     levelSystem.UnlockNextLevel();
        // }
    }

    // called upon button click for next level
    private void NextLevel()
    {
        int nextSceneIdx = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIdx);
    }

    private void RestartLevel()
    {   
        // just load the current scene again
        int curBuildIdx = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(curBuildIdx);
    }

    private void StartScreenReturn()
    {
        SceneManager.LoadScene(0);
    }
}
