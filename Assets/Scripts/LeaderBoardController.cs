using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LeaderBoardController : MonoBehaviour
{
    public TextMeshProUGUI leaderboard;
    public static LeaderBoardController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        DisplayLeaderBoard();
    }

    public static void UpdateLeaderBoard()
    {
        float timeCurrent = ((float)ScoreHandler.GetTime().TotalMilliseconds);
        float[] times = new float[3];

        for (int i = 0; i < 3; i++)
        {
            times[i] = PlayerPrefs.GetFloat("Time" + i, 0.0f);
        }

        for(int i = 0; i < 3; ++i)
        {
            if(timeCurrent < times[i] || times[i] == 0.0)
            {
                for (int j = 2; j > i; j--)
                {
                    times[j] = times[j - 1];
                }

                times[i] = timeCurrent;
                
                break;
            }
        }

        for (int i = 0; i < 3; ++i)
        {
            PlayerPrefs.SetFloat("Time" + i, times[i]);
        }
        PlayerPrefs.Save();

        Instance.DisplayLeaderBoard();
    }

    public void DisplayLeaderBoard()
    {
        leaderboard.text = "";
        for (int i = 0; i < 3; i++)
        {
            var time = PlayerPrefs.GetFloat("Time" + i, 0.0f);
            leaderboard.text += $"{i + 1}. {TimeSpan.FromMilliseconds(time).Minutes,0}m {TimeSpan.FromMilliseconds(time).Seconds,0}s \n";
        }
    }

    public void DeleteRecords()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        DisplayLeaderBoard();
    }
}
