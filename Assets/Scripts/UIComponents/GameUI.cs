using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    [SerializeField] private ScoreData scoreData;
    [SerializeField] public RawImage killImage;
    [SerializeField] private VideoManager killVideoManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        HideKillVideo();
        Debug.Assert(scoreText != null, "Score Text reference is not set in GameUI!");
        Debug.Assert(killImage != null, "Kill Image reference is not set in GameUI!");
        Debug.Assert(killVideoManager != null, "Kill Video Manager reference is not set i");
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
        GoombaEvents.OnGoombaDeath += HandleGoombaDeath;
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

    private void HandleGoombaDeath()
    {
        ShowKillVideo();
    }

    public void SetGameOver()
    {
        gameObject.SetActive(false);
        HideKillVideo();
    }

    public void SetRestart()
    {
        Debug.Log("GameUI: Setting restart");
        gameObject.SetActive(true);
        killImage.enabled = false;
        HideKillVideo();
        UpdateScore(0);
    }

    public void UpdateScore(int score)
    {

        scoreText.text = "Score: " + scoreData.currentScore.ToString() + "\nHighscore: " + scoreData.highScore.ToString();
    }

    private void HideKillVideo()
    {
        killImage.enabled = false;
        killVideoManager.StopVideo();
    }

    private void ShowKillVideo()
    {
        killVideoManager.StopVideo();
        killImage.enabled = true;
    }
}
