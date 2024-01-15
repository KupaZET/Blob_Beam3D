using System;
using System.Diagnostics;
using UnityEngine;

public static class ScoreHandler
{
    static DateTime TimeStarted;
    static int Deaths;

    public static void Start()
    {
        Deaths = 0;
        TimeStarted = DateTime.UtcNow;
    }

    public static TimeSpan GetTime() => (DateTime.UtcNow - TimeStarted);

    public static string GetDeaths() => Deaths.ToString();

    public static int CalculateScore()
    {
        int timeScore = GetTime().Seconds * 10;
        int deathScore = Deaths * 100;
        int totalScore = 1000 - deathScore - timeScore;

        return totalScore < 0 ? 0 : totalScore;
    }
}