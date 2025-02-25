using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSystem : MonoBehaviour
{   
    public Button startButton;
    public GameObject levelSelectPanel;
    public Button[] levelButtons;
    private int unlockedLevels; // track the number of levels unlocked

    void Start()
    {   
        unlockedLevels = GameData.UnlockedLevels;
        // Debug.Log($"Start: UnlockedLevels = {unlockedLevels}, GameData.UnlockedLevels = {GameData.UnlockedLevels}");

        startButton.gameObject.SetActive(true);
        levelSelectPanel.SetActive(false);

        // add null checks in case
        if (startButton != null)
        {
            startButton.onClick.AddListener(ShowLevelSelect);
        }
        else
        {
            Debug.LogError("Start Button not assigned in inspector this is problem!");
        }

        // this will start by enabling level 1 and then keep going
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
            // Debug.Log($"Button {i} interactable: {levelButtons[i].interactable}, UnlockedLevels: {GameData.UnlockedLevels}");

            // click handler for button
            levelButtons[i].onClick.RemoveAllListeners();
            
            // this is what handles the scene loading
            levelButtons[i].onClick.AddListener(() => SceneManager.LoadScene(levelIndex + 1)); // now when clicked, will call the load next level method
        }
    }
}
