using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Button restartButton;

    [SerializeField] private GameObject player;
    private PlayerMovement playerMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.Log("PlayerMovement component not found on player GameObject.");
        }

        // Setup restart button
        if (restartButton != null)
        {
            Debug.Log("Restart button found and listener added.");
            restartButton.onClick.AddListener(RestartGame);
        }

        // Hide game over screen
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(int score)
    {
        gameObject.SetActive(true);
        scoreText.text = "Score: " + score.ToString();
    }
    
    public void RestartGame()
    {
        Debug.Log("Restart button clicked.");
        if (playerMovement != null)
        {
            Debug.Log("Calling RestartButtonCallback on PlayerMovement.");
            gameObject.SetActive(false);
            playerMovement.RestartButtonCallback();
        }
    }
}
