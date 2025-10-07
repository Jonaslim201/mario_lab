using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        Debug.Log("GameUI: OnEnable called");
        // Subscribe to events
        GameManager.OnGameStart += HandleGameStart;
        GameManager.OnGameOver += HandleGameOver;
        GameManager.OnGameRestart += HandleGameRestart;
        GameManager.OnScoreChange += HandleScoreChange;
    }

    private void HandleGameStart()
    {
        Debug.Log("GameUI: Handling game start");
        // Initialize UI for game start
        SetRestart();
    }

    private void HandleGameOver(int score)
    {
        Debug.Log("GameUI: Handling game over");
        SetGameOver();
    }

    private void HandleGameRestart()
    {
        Debug.Log("GameUI: Handling game restart");
        SetRestart();
    }

    private void HandleScoreChange(int newScore)
    {
        Debug.Log($"GameUI: Score changed to {newScore}");
        UpdateScore(newScore);
    }

    public void SetGameOver()
    {
        gameObject.SetActive(false);
    }

    public void SetRestart()
    {
        Debug.Log("GameUI: Setting restart");
        gameObject.SetActive(true);
        UpdateScore(0);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
        else
        {
            Debug.LogWarning("Score Text reference is not set in GameUI!");
        }
    }
}
