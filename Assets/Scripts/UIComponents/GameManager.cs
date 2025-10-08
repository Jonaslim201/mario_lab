using UnityEngine;
using System;

public class GameManager : MonoBehaviour
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

    [HideInInspector]
    public int score { get; private set; } = 0;
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

        TriggerGameStart();
    }

    void OnEnable()
    {
        GoombaEvents.OnGoombaDeath += HandleGoombaDeath;
    }

    void OnDisable()
    {
        GoombaEvents.OnGoombaDeath -= HandleGoombaDeath;
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
        OnScoreChange?.Invoke(score);
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
        gameUIComponent.gameObject.SetActive(true);
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
        killScore = 0;

        gameUIComponent.gameObject.SetActive(true);
        // Reset time scale first
        Time.timeScale = 1.0f;

        // Trigger restart event for all listeners
        OnGameRestart?.Invoke();

        // Trigger score change to reset score display
        OnScoreChange?.Invoke(score);
    }
}
