using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    public float maxSpeed = 20;
    private Rigidbody2D marioBody;
    public float upSpeed = 10;
    private bool onGroundState = true;
    // global variables
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    private Vector3 originalScale;
    public TextMeshProUGUI scoreText;
    public GameOverController gameOverController;
    public GameObject enemies;
    private Vector3 originalPosition;
    public JumpOverGoomba jumpOverGoomba;

    [Header("UI References")]
    [SerializeField] private GameObject gameUI;

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate =  30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate is called 50 times a second
    void  FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(moveHorizontal) > 0){

            Vector2 movement = new Vector2(moveHorizontal, 0);
            // check if it doesn't go beyond maxSpeed
            if (marioBody.linearVelocity.magnitude < maxSpeed)
            {
                marioBody.AddForce(movement * speed);
            }
            // Handle flipping based on movement direction
            if (moveHorizontal < 0 && faceRightState)
            {
                faceRightState = false;
                transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
            }
            else if (moveHorizontal > 0 && !faceRightState)
            {
                faceRightState = true;
                transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            }
    }

        // stop
        if (Input.GetKeyUp("a") || Input.GetKeyUp("d")){
            // stop
            marioBody.linearVelocity = Vector2.zero;
        }

        if (Input.GetKeyDown("space") && onGroundState){
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
        }
    }

    // Flip the character to face the mouse cursor (for future use with shooting)
    private void FlipController()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x < transform.position.x && faceRightState)
        {
            Flip();
        }
        else if (mousePos.x > transform.position.x && !faceRightState)
        {
            Flip();
        }
    }

    private void Flip(){
        faceRightState = !faceRightState;
        transform.Rotate(0,180,0);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground")) onGroundState = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Time.timeScale = 0.0f;
            gameOverController.Setup(jumpOverGoomba.score);
            gameUI.SetActive(false);
            Debug.Log("Game Over!");
        }
    }

    public void RestartButtonCallback()
    {
        Debug.Log("Restart!");
        // reset everything
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;
    }

    private void ResetGame()
    {
        // reset position
        marioBody.transform.position = originalPosition;
        // reset sprite direction
        faceRightState = true;
        transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        marioBody.linearVelocity = Vector2.zero;
        // reset score
        scoreText.text = "Score: 0";
        gameUI.SetActive(true);
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.transform.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }
        jumpOverGoomba.score = 0;
    }
}