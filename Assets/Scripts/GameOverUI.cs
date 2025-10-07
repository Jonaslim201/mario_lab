using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Hide game over screen
        gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        Debug.Log("GameOverUI: OnEnable called");
        GameManager.OnGameOver += HandleGameOver;
        GameManager.OnGameRestart += HandleGameRestart;
    }

    private void HandleGameOver(int score)
    {
        Debug.Log("GameOverUI: Handling game over");
        SetGameOver(score);
    }

    private void HandleGameRestart()
    {
        Debug.Log("GameOverUI: Handling game restart");
        SetRestart();
    }

    public void SetGameOver(int score)
    {
        Debug.Log("GameOverUI: Setting game over with score " + score);
        gameObject.SetActive(true);
        UpdateScore(score);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
        else
        {
            Debug.LogWarning("Score Text reference is not set in GameOverController!");
        }
    }

    public void SetRestart()
    {
        gameObject.SetActive(false);
    }

}
