using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{

    [Header("Game Objects")]
    [SerializeField] public PlayerMovement playerMovement;
    [SerializeField] private GameObject enemies;
    [SerializeField] private JumpOverGoomba jumpOverGoomba;
    [SerializeField] private Timer Timer;
    [SerializeField] private EnemyPool EnemyPool;

    [Header("UI Components")]
    [SerializeField] private GameUI gameUIComponent;
    [SerializeField] private GameOverUI gameOverComponent;

    [HideInInspector]
    public int score { get; private set; } = 0;

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

        TriggerGameStart();
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

        if (jumpOverGoomba == null)
        {
            Debug.LogError("JumpOverGoomba reference is not set in GameManager!");
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
        OnScoreChange?.Invoke(score);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetGameOver()
    {
        Debug.Log("Game Over Triggered");
        Time.timeScale = 0.0f;
        OnGameOver?.Invoke(score);
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"Score updated: {score}");
        OnScoreChange?.Invoke(score);
    }

    public void RestartGame()
    {
        Debug.Log("Game Restart Triggered");

        // Reset score
        score = 0;

        // Reset time scale first
        Time.timeScale = 1.0f;

        // Trigger restart event for all listeners
        OnGameRestart?.Invoke();

        // Trigger score change to reset score display
        OnScoreChange?.Invoke(score);
    }
}
