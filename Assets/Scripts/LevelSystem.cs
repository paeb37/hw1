using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSystem : MonoBehaviour
{   

    // assign the public vars in the Inspector
    public Button startButton;
    public GameObject levelSelectPanel;
    public Button[] levelButtons;
    private int unlockedLevels = 1; // start with only one level unlocked

    void Start()
    {   

        // player prefs stores data across scenes
        // as key value pair
        // so we can track how many levels are completed by user
        unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);
        
        startButton.gameObject.SetActive(true);
        levelSelectPanel.SetActive(false);

        startButton.onClick.AddListener(ShowLevelSelect);
        UpdateLevelButtons();
    }

    void ShowLevelSelect()
    {
        startButton.gameObject.SetActive(false);  // Hide start button
        levelSelectPanel.SetActive(true);         // Show level selection
    }

    // use a list of Button objects to keep track
    // makes things easier
    void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {   

            // this loop keeps track of which ones are disabled or not
            int levelIndex = i;
            levelButtons[i].interactable = i < unlockedLevels; // basically if disabled or not
            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex + 1)); // now when clicked, will call the load next level method
        }
    }

    void LoadLevel(int levelNum)
    {
        SceneManager.LoadScene("L" + levelNum); // named L1, L2, L3
    }

    // to unlock levels (call this when a level is cleared)
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
