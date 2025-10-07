using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
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