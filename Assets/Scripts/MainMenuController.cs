using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public TextMeshProUGUI Score;

    private void Start()
    {
        if (Score != null)
        {
            LeaderBoardController.UpdateLeaderBoard();
            var time = ScoreHandler.GetTime();
            Score.text =
                $"Time: {time.Minutes,0}m {time.Seconds,0}s\n" +
                $"Deaths: {ScoreHandler.GetDeaths()}\n" +
                $"Score: {ScoreHandler.CalculateScore()}";
        }
    }

    public void PlayGame()
    {
        ScoreHandler.Start();
        SceneManager.LoadScene("MainScene");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LeaderBoard()
    {
        SceneManager.LoadScene("LeaderBoardScene");
    }
}
