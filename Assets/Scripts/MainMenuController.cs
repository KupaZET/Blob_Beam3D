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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
