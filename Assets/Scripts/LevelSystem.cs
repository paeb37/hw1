using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSystem : MonoBehaviour
{   

    // using the "singleton" method so there is only one level system instance
    // which persists across all scenes

    // also use level system to keep track of the timer as well

    // public static LevelSystem Instance { get; private set; }

    // assign the public vars in the Inspector
    public Button startButton;
    public GameObject levelSelectPanel;
    public Button[] levelButtons;
    private int unlockedLevels; // track the number of levels unlocked

    // private float levelStartTime; // will update with each new level
    // private bool timerActive = false; // only start when level 1 starts
    // ignoring the paused time

    // the only thing that really need to be persistent is the unlocked levels

    // it's easier to just pass this variable between calls
    // rather than have it persist

    // void Awake()
    // {
    //     if (Instance == null) // if no instance of level system exists yet
    //     {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject); // don't get rid of it when switching scenes
    //         // unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);
    //         unlockedLevels = 1;
    //     }
    //     else
    //     {
    //         Destroy(gameObject); // means there is already one so destroy new one created
    //     }
    // }

    void Start()
    {   
        unlockedLevels = GameData.UnlockedLevels;

        Debug.Log($"Start: UnlockedLevels = {unlockedLevels}, GameData.UnlockedLevels = {GameData.UnlockedLevels}");

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

        // // by default, all level buttons are disabled
        // foreach (Button button in levelButtons)
        // {
        //     button.interactable = false;
        // }

        // this one will start by enabling level 1 and then keep going
        UpdateLevelButtons();
    }

    // just to show the levels after the user clicks start
    void ShowLevelSelect()
    {
        startButton.gameObject.SetActive(false); // hide start
        levelSelectPanel.SetActive(true); // show level select
    }

    // updates the UI and interactability of the level buttons
    // based on what levels are unlocked
    void UpdateLevelButtons()
    {      
        // check if we have valid buttons first, prevent null error
        if (levelButtons == null || levelButtons.Length == 0)
        {
            return;
        }

        // nice way to do with for loop instead of setting manually in the editor

        for (int i = 0; i < levelButtons.Length; i++)
        {   
            // this loop keeps track of which ones are disabled or not
            int levelIndex = i;

            levelButtons[i].interactable = i < unlockedLevels; // basically if disabled or not
            
            // Debug.Log($"Number of unlocked levels: {unlockedLevels}");
            Debug.Log($"Button {i} interactable: {levelButtons[i].interactable}, UnlockedLevels: {GameData.UnlockedLevels}");

            // click handler for button
            levelButtons[i].onClick.RemoveAllListeners();
            
            // this is what handles the scene loading
            levelButtons[i].onClick.AddListener(() => SceneManager.LoadScene(levelIndex + 1)); // now when clicked, will call the load next level method
        }
    }

    // this is called by VictoryScreen when the player beats level
    // public void UnlockNextLevel()
    // {
    //     if (unlockedLevels < levelButtons.Length)
    //     {
    //         unlockedLevels++; // this is what unlocks the next level
    //         GameData.UnlockedLevels = unlockedLevels;

    //         Debug.Log($"After unlock: UnlockedLevels = {unlockedLevels}, GameData.UnlockedLevels = {GameData.UnlockedLevels}");

    //         // have to change the stored dict values as well
    //         // PlayerPrefs.SetInt("UnlockedLevels", unlockedLevels);
    //         // PlayerPrefs.Save();
    //         UpdateLevelButtons();
    //     }

    //     // reset the screen too so the buttons don't overlap in UI
    //     // if (startButton != null && levelSelectPanel != null)
    //     // {
    //     //     startButton.gameObject.SetActive(true);
    //     //     levelSelectPanel.SetActive(false);
    //     // }
    // }
}
