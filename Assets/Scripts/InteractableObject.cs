using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    protected Collider2D collider2d;
    protected bool isActive = true;
    protected SpringJoint2D springJoint;
    public Rigidbody2D rb;
    private GameObject interactableObject;

    void Awake()
    {
        interactableObject = gameObject;
        Debug.Assert(interactableObject != null, "InteractableObject: No GameObject found for " + gameObject.name);

        springJoint = GetComponent<SpringJoint2D>();
        Debug.Assert(springJoint != null, "SpringJoint2D component missing from InteractableObject");

        collider2d = GetComponent<Collider2D>();
        if (collider2d == null)
        {
            Debug.LogError("InteractableObject: No Collider2D found on " + gameObject.name);
        }

        rb = GetComponent<Rigidbody2D>();
        Debug.Assert(rb != null, "Rigidbody2D component missing from InteractableObject");
    }
    public abstract void Interact();

    protected void HoldBlockInPlace()
    {
        // Stop motion
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        // Stop physics control
        rb.bodyType = RigidbodyType2D.Kinematic;
        // disable the joint
        springJoint.enabled = false;

        interactableObject.transform.localPosition = Vector3.zero;
    }

    protected void ReleaseBlock()
    {
        // Enable physics control
        rb.bodyType = RigidbodyType2D.Dynamic;
        // enable the joint
        springJoint.enabled = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        interactableObject.transform.localPosition = Vector3.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collided with QuestionMarkBlock");
            Vector2 hitDirection = collision.contacts[0].normal;
            Debug.Log("Hit direction: " + hitDirection);
            if (hitDirection.y > 0f) // Adjust threshold as needed
            {
                Debug.Log("Player hit from below!");
                isActive = false;
                Debug.Log("Spring Joint Frequency: " + springJoint.frequency + ", Damping Ratio: " + springJoint.dampingRatio);
                Interact();
            }
            else // Hit from above or side - disable spring
            {
                Debug.Log("Player hit from above - disabling spring");
                HoldBlockInPlace();
            }

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!isActive) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player exited collision with QuestionMarkBlock");
            ReleaseBlock();
        }
    }
}
