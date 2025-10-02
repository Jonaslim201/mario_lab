using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    void Start()
    {
        // Debug: Check if components exist
        if (GetComponent<Collider2D>() == null)
            Debug.LogError("Bullet missing Collider2D!");
        if (rb == null)
            Debug.LogError("Bullet missing Rigidbody2D!");
    }

    // Update is called once per frame
    void Update()
    {
        transform.right = rb.linearVelocity;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bullet hit: " + collision.gameObject.name);
        // Destroy bullet on hit
        Destroy(gameObject);
    }
}
