using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GoombaEvents
{
    public static event Action<int> OnGoombaStomped;
    public static event Action OnGoombaDeath;

    public static void TriggerGoombaStomped(int scorePoints)
    {
        Debug.Log("Triggering OnGoombaStomped event");
        OnGoombaStomped?.Invoke(scorePoints);
    }

    public static void TriggerGoombaDeath()
    {
        Debug.Log("Triggering GoombaDeath event");
        OnGoombaDeath?.Invoke();
    }
}

public class EnemyMovement : MonoBehaviour
{
    public GameManager gameManager;

    [Header("Positioning")]
    private Vector3 originalPos;
    private Vector2 velocity;

    [Header("Movement Parameters")]
    public float moveSpeed = 1.5f;
    private float originalSpeed;
    public float decayTime = 1.5f;

    [Header("State")]
    private bool alive = true;
    private bool hitWall = false;

    [Header("Components")]
    private Rigidbody2D enemyBody;
    public SpriteRenderer goombaSprite;
    public Animator goombaAnimator;

    [Header("Stomping")]
    [SerializeField] public float stompForce = 15f;
    [SerializeField] private int stompScorePoints = 2;

    void Awake()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        // get starting position
        originalPos = transform.localPosition;
        // get original movement params
        originalSpeed = moveSpeed;
        goombaAnimator.SetBool("onDeath", false);
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void OnEnable()
    {
        GameManager.OnGameRestart += HandleGameRestart;
    }

    void OnDisable()
    {
        GameManager.OnGameRestart -= HandleGameRestart;
    }

    private void HandleGameRestart()
    {
        Debug.Log($"EnemyMovement: {gameObject.name} handling restart");
        SetRestart();
    }

    public void SetRestart()
    {
        alive = true;
        gameObject.SetActive(true);
        goombaSprite.enabled = true;
        SetColliders(true);
        enemyBody.bodyType = RigidbodyType2D.Dynamic;
        transform.localPosition = originalPos;
        moveSpeed = originalSpeed;
        goombaAnimator.SetBool("onDeath", false);
    }

    void Movegoomba()
    {
        enemyBody.linearVelocityX = velocity.x;
    }

    void SetColliders(bool state)
    {
        Collider2D[] allCol = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in allCol)
        {
            col.enabled = state;
        }
    }
    //When dead
    IEnumerator GoombaStomped()
    {
        Debug.Log("Goomba Stomped");
        alive = false;
        goombaAnimator.SetBool("onDeath", true);
        enemyBody.bodyType = RigidbodyType2D.Static;
        SetColliders(false);
        yield return new WaitForSeconds(decayTime);
        transform.position += 20 * Vector3.up;
        goombaSprite.enabled = false;
        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                if (contact.normal == Vector2.left || contact.normal == Vector2.right)
                {
                    hitWall = true;
                }
            }
        }
        else
        {
            Debug.Log("Collided with Player");
            foreach (ContactPoint2D contact in col.contacts)
            {
                Debug.Log($"Contact normal: {contact.normal}");
                Debug.Log(contact.normal == Vector2.down);
                if (contact.normal == Vector2.down && alive)
                {
                    Debug.Log("Player stomped Goomba");
                    gameManager.AddScore(stompScorePoints);
                    GoombaEvents.TriggerGoombaDeath();
                    StartCoroutine(GoombaStomped());
                    Rigidbody2D playerBody = col.gameObject.GetComponent<Rigidbody2D>();
                    if (playerBody != null)
                    {
                        playerBody.linearVelocity = new Vector2(playerBody.linearVelocity.x, 0); // Reset vertical velocity
                        playerBody.AddForce(Vector2.up * stompForce, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            GoombaEvents.TriggerGoombaDeath();
            StartCoroutine(GoombaStomped());
            gameManager.AddScore(1);
        }
    }

    void Update()
    {
        if (hitWall)
        {
            // change direction
            moveSpeed *= -1;
            hitWall = false;
        }
        velocity = new Vector2(moveSpeed, 0);
        if (alive)
        {
            Movegoomba();
        }
        goombaAnimator.SetFloat("xSpeed", Mathf.Abs(moveSpeed));
    }
}