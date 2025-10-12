using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{

    [Header("Game Objects")]
    [SerializeField] public PlayerMovement playerMovement;
    [SerializeField] private GameObject enemies;
    [SerializeField] private Timer Timer;
    [SerializeField] private EnemyPool EnemyPool;

    [Header("Audio Components")]
    [SerializeField] private AudioManager killAudioManager;

    [Header("Video Components")]
    [SerializeField] private VideoManager killVideoManager;

    [Header("UI Components")]
    [SerializeField] private GameUI gameUIComponent;
    [SerializeField] private GameOverUI gameOverComponent;
    public ScoreData scoreData;

    [HideInInspector]
    private int killScore = 0;

    #region Game Events
    public static event Action OnGameStart;
    public static event Action<int> OnGameOver;
    public static event Action OnGameRestart;
    public static event Action<int> OnScoreChange;
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ValidateReferences();
        LoadHighScore();
        TriggerGameStart();
    }

    void OnEnable()
    {
        GoombaEvents.OnGoombaStomped += HandleGoombaStomped;
        GoombaEvents.OnGoombaDeath += HandleGoombaDeath;
    }

    void OnDisable()
    {
        GoombaEvents.OnGoombaStomped -= HandleGoombaStomped;
    }

    private void ValidateReferences()
    {
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement reference is not set in GameManager!");
            return;
        }

        if (enemies == null)
        {
            Debug.LogError("Enemies reference is not set in GameManager!");
            return;
        }

        if (gameUIComponent == null)
        {
            Debug.LogError("gameUI reference is not set in GameManager!");
            return;
        }

        if (gameOverComponent == null)
        {
            Debug.LogError("gameOverComponent reference is not set in GameManager!");
            return;
        }
    }

    public void TriggerGameStart()
    {
        Debug.Log("Game Started");
        OnGameStart?.Invoke();

        // Initialize score
        OnScoreChange?.Invoke(scoreData.currentScore);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void HandleGoombaStomped(int points)
    {
        AddScore(points);
    }

    private void HandleGoombaDeath()
    {
        Debug.Log("Handling Goomba Death, index: " + killScore);
        killAudioManager.PlaySound(killScore);
        killVideoManager.PlayVideo(killScore);
        killScore += 1;
        if (killScore >= 5)
        {
            killScore = 0;
        }
    }

    public void SetGameOver()
    {
        Debug.Log("Game Over Triggered");
        Time.timeScale = 0.0f;
        SaveHighScore();   // <-- Save highscore when game ends
        OnGameOver?.Invoke(scoreData.currentScore);
    }

    public void AddScore(int points)
    {
        scoreData.AddPoints(points);
        Debug.Log($"Score updated: {scoreData.currentScore}");
        OnScoreChange?.Invoke(scoreData.currentScore);
    }

    public void RestartGame()
    {
        Debug.Log("Game Restart Triggered");
        scoreData.ResetCurrentScore();
        Time.timeScale = 1.0f;
        OnGameRestart?.Invoke();
        OnScoreChange?.Invoke(scoreData.currentScore);
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", scoreData.highScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        scoreData.highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void OnApplicationQuit()
    {
        SaveHighScore();
    }
}
