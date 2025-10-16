using System.Collections;
using UnityEngine;

public abstract class BaseBlock : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    [Header("Block Behavior Settings")]
    [SerializeField] protected bool canReleaseCoins = false;
    [SerializeField] protected bool canReleasePowerUp = false;
    [SerializeField] protected bool staysBouncy = true;
    [SerializeField] protected bool hasAnimation = false;

    [Header("Physics Settings")]
    [SerializeField] protected float bounceDuration = 0.7f;
    [SerializeField] protected float bounceForce = 10f;

    [Header("Components")]
    protected SpringJoint2D springJoint;
    protected Rigidbody2D rb;
    protected Collider2D blockCollider;
    protected bool isActive = true;
    protected bool isBouncing = false;


    protected virtual void Awake()
    {
        springJoint = GetComponent<SpringJoint2D>();
        rb = GetComponent<Rigidbody2D>();
        blockCollider = GetComponent<Collider2D>();

        Debug.Assert(springJoint != null, "SpringJoint2D missing from " + gameObject.name);
        Debug.Assert(rb != null, "Rigidbody2D missing from " + gameObject.name);

        gameManager = FindAnyObjectByType<GameManager>();
        GameManager.OnGameRestart += ResetBlock;
    }

    protected virtual void Start()
    {
        MakeKinematic();
    }

    void Update()
    {
        if (!isBouncing)
            MakeKinematic();
    }

    protected virtual void MakeKinematic()
    {
        if (rb.bodyType != RigidbodyType2D.Kinematic)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            springJoint.enabled = false;
            transform.localPosition = Vector3.zero;
        }
    }

    protected virtual void MakeDynamic()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        springJoint.enabled = true;
    }

    protected virtual void OnHitFromBelow()
    {
        if (!isActive) return;

        // Start bouncing
        isBouncing = true;
        MakeDynamic();
        rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);

        // Handle coin release
        if (canReleaseCoins)
        {
            ReleaseCoin();
        }

        if (canReleasePowerUp)
        {
            ReleasePowerUp();
            Debug.Log("Base block: Power-up release logic not implemented");
        }

        // Handle animation
        if (hasAnimation)
        {
            Debug.Log("BaseBlock: Playing animation");
            PlayAnimation();
        }

        // Handle reusability
        if (!staysBouncy)
        {
            isActive = false;
        }

        StartCoroutine(BounceDurationCoroutine());
    }

    protected virtual IEnumerator BounceDurationCoroutine()
    {
        yield return new WaitForSeconds(bounceDuration);
        isBouncing = false;
    }

    protected virtual void ReleaseCoin()
    {
        Debug.Log("Base block: No coin release logic");
    }

    protected virtual void ReleasePowerUp()
    {
        Debug.Log("Base block: No power-up release logic");
    }

    protected virtual void PlayAnimation()
    {
        Debug.Log("Base block: No animation logic");
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 hitDirection = collision.contacts[0].normal;
            if (hitDirection.y > 0f)
            {
                OnHitFromBelow();
            }
        }
    }

    protected virtual void ResetAnimation()
    {
        Debug.Log("Base block: No reset animation logic");
    }

    protected virtual void ResetBlock()
    {
        MakeKinematic();
        // Reset the active state
        isActive = true;

        if (hasAnimation)
        {
            Debug.Log("BaseBlock: Resetting animation");
            ResetAnimation();
        }
    }
}