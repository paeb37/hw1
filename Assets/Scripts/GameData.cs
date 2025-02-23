using UnityEngine;

// use static class to store persistent data
// like num of unlocked levels

public static class GameData
{
    public static int UnlockedLevels { get; set; } = 1;

    // hold other constants across classes
    public const float collectDistance = 2f;
}