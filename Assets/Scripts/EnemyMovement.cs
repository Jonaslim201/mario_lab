using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    public Vector3 originalPos;
    public float moveSpeed = 1.5f;
    private float originalSpeed;
    public float decayTime = 1.5f;
    private bool alive = true;
    private Vector2 velocity;
    private Rigidbody2D enemyBody;
    public SpriteRenderer goombaSprite;
    private bool hitWall = false;

    public Animator goombaAnimator;

    void Awake() {
        enemyBody = GetComponent<Rigidbody2D>();
        // get starting position
        originalPos = transform.localPosition;
        // get original movement params
        originalSpeed = moveSpeed;
        goombaAnimator.SetBool("onDeath", false);
    }

    public void Reset() {
        alive = true;
        goombaSprite.enabled = true;
        SetColliders(true); 
        enemyBody.bodyType = RigidbodyType2D.Dynamic;
        enemyBody.transform.localPosition = originalPos;
        moveSpeed = originalSpeed;
        goombaAnimator.SetBool("onDeath", false);
    }

    void Movegoomba() {
        enemyBody.linearVelocityX = velocity.x;
    }

    void SetColliders(bool state) {
        Collider2D[] allCol = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in allCol) {
            col.enabled = state;
        }
    }
    //When dead
    IEnumerator GoombaStomped() {
        alive = false;
        goombaAnimator.SetBool("onDeath", true);
        enemyBody.bodyType = RigidbodyType2D.Static;
        SetColliders(false);
        yield return new WaitForSeconds(decayTime);
        transform.position += 20 * Vector3.up;
        goombaSprite.enabled = false;
        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (!col.gameObject.CompareTag("Player")) {
            foreach (ContactPoint2D contact in col.contacts) {
                if (contact.normal == Vector2.left || contact.normal == Vector2.right) {
                    hitWall = true;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.CompareTag("Bullet")) {
            StartCoroutine(GoombaStomped());
        }
    }

    void Update() {
        if (hitWall) {
            // change direction
            moveSpeed *= -1;
            hitWall = false;
        }
        velocity = new Vector2(moveSpeed, 0);
        if (alive) {
            Movegoomba();
        }
        goombaAnimator.SetFloat("xSpeed", Mathf.Abs(moveSpeed));
    }
}