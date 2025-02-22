using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSystem : MonoBehaviour
{   

    // using the "singleton" method so there is only one level system instance
    // which persists across all scenes

    // also use level system to keep track of the timer as well

    public static LevelSystem Instance { get; private set; }

    // assign the public vars in the Inspector
    public Button startButton;
    public GameObject levelSelectPanel;
    public Button[] levelButtons;
    private int unlockedLevels = 1; // start with only one level unlocked

    // private float levelStartTime; // will update with each new level
    // private bool timerActive = false; // only start when level 1 starts
    // ignoring the paused time

    void Awake()
    {
        if (Instance == null) // if no instance of level system exists yet
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // don't get rid of it when switching scenes
            unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);
        }
        else
        {
            Destroy(gameObject); // means there is already one so destroy new one created
        }
    }

    void Start()
    {   
        // only need to do this in the start scene (build idx 0)
        int curBuildIdx = SceneManager.GetActiveScene().buildIndex;
        
        if (curBuildIdx == 0)
        {
            // note: player prefs stores data across scenes, as key value pair
            unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);
            
            startButton.gameObject.SetActive(true);
            levelSelectPanel.SetActive(false);

            // add null checks in case
            if (startButton != null)
            {
                startButton.onClick.AddListener(ShowLevelSelect);
            }
            else
            {
                Debug.LogError("Start Button not assigned in inspector!");
            }

            UpdateLevelButtons();
        }
    }

    // just to show the levels after the user clicks start
    void ShowLevelSelect()
    {
        startButton.gameObject.SetActive(false); // hide start
        levelSelectPanel.SetActive(true); // show level select
    }

    // use a list of Button objects to keep track
    // makes things easier
    void UpdateLevelButtons()
    {   
        // nice way to do with for loop instead of setting manually in the editor

        for (int i = 0; i < levelButtons.Length; i++)
        {   

            // this loop keeps track of which ones are disabled or not
            int levelIndex = i;
            levelButtons[i].interactable = i < unlockedLevels; // basically if disabled or not
            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => SceneManager.LoadScene(levelIndex + 1)); // now when clicked, will call the load next level method
        }
    }

    // to unlock levels (call this when a level is cleared)
    // this is more of a numeric method
    public void UnlockNextLevel()
    {
        if (unlockedLevels < levelButtons.Length)
        {
            unlockedLevels++;

            // have to change the stored dict values as well
            PlayerPrefs.SetInt("UnlockedLevels", unlockedLevels);
            PlayerPrefs.Save();
            UpdateLevelButtons();
        }
    }
}
