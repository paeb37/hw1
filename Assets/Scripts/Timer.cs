using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    
    private float levelStartTime;
    private bool timerActive = false;
    private float elapsedTime;

    void Start()
    {
        levelStartTime = Time.time;
        timerActive = true;
    }

    void Update()
    {
        if (timerActive)
        {
            elapsedTime = Time.time - levelStartTime;
            
            // update timer text display
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void StopTimer()
    {
        timerActive = false;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
