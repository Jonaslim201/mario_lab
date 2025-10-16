using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private ScoreData scoreData;

    public void PlayGame()
    {
        SceneManager.LoadScene("LoadingScreen"); // Replace with your scene name
    }

    public void SetHighScore(int score)
    {
        Debug.Log("Setting high score to: " + score);
        if (highScoreText != null)
        {
            highScoreText.text = "Highscore: " + score.ToString();
        }
        else
        {
            Debug.LogWarning("HighScoreText reference is not set in MainMenuUI!");
        }
    }

    private void Start()
    {
        audioManager.PlayAudioClip(mainMenuMusic);
        ResetHighScoreButton.OnHighScoreReset += SetHighScore;
        SetHighScore(scoreData.highScore);
    }
}
