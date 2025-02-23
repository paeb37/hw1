using UnityEngine;

// use static class to store persistent data
// like num of unlocked levels

public static class GameData
{
    public static int UnlockedLevels { get; set; } = 1;
}