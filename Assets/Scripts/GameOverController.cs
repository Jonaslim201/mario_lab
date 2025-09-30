using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Button restartButton;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemies;
    [SerializeField] private JumpOverGoomba jumpOverGoomba;
    [SerializeField] private GameObject gameUI;

    private PlayerMovement playerMovement;
    private Rigidbody2D playerBody;
    private BoxCollider2D playerCollider;
    private AudioSource playerAudioSource;
    private Vector3 originalScale;
    private Vector3 originalPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is not set in GameOverController!");
            return;
        }

        if (enemies == null)
        {
            Debug.LogError("Enemies reference is not set in GameOverController!");
            return;
        }

        if (jumpOverGoomba == null)
        {
            Debug.LogError("JumpOverGoomba reference is not set in GameOverController!");
            return;
        }

        // Setup restart button
        if (restartButton != null)
        {
            Debug.Log("Restart button found and listener added.");
            restartButton.onClick.AddListener(RestartGame);
        }

        // Hide game over screen
        gameObject.SetActive(false);
        playerBody = player.GetComponent<Rigidbody2D>();
        originalPosition = player.transform.position;
        originalScale = player.transform.localScale;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerCollider = player.GetComponent<BoxCollider2D>();
        playerAudioSource = player.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(int score)
    {
        Time.timeScale = 0.0f;
        gameUI.SetActive(false);
        gameObject.SetActive(true);
        scoreText.text = "Score: " + score.ToString();
    }

    public void RestartButtonCallback()
    {
        Debug.Log("Restart!");
        // reset everything
        RestartGame();
        // resume time
        Time.timeScale = 1.0f;
        gameUI.SetActive(true);
    }

    public void RestartGame()
    {
        // reset position
        playerBody.transform.position = originalPosition;
        // reset sprite direction
        playerMovement.setFaceRightState(true);
        playerBody.transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        playerBody.linearVelocity = Vector2.zero;
        playerCollider.enabled = true;
        playerMovement.marioAnimator.SetTrigger("gameRestart");
        playerAudioSource.Stop();
        playerMovement.alive = true;
        // reset score
        scoreText.text = "Score: 0";
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.transform.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }
        jumpOverGoomba.score = 0;
        gameObject.SetActive(false);
    }
}
