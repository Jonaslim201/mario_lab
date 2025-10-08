using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    [SerializeField] public RawImage killImage;
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

    void OnDisable()
    {
        GameManager.OnGameStart -= HandleGameStart;
        GameManager.OnGameOver -= HandleGameOver;
        GameManager.OnGameRestart -= HandleGameRestart;
        GameManager.OnScoreChange -= HandleScoreChange;
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
        Debug.Log("Game restart GameUI: Handling game restart");
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
        scoreText.text = "Score: " + score.ToString();
    }

}
