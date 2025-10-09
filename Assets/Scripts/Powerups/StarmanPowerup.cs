using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarmanPowerup : BasePowerup
{
    [SerializeField] private float invincibilityDuration = 10f; // Duration of invincibility in seconds
    [SerializeField] private float flashInterval = 0.1f; // Flash speed during invincibility

    protected override void Start()
    {
        base.Start();
        this.type = PowerupType.StarMan;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && spawned)
        {
            // Apply invincibility effect to player
            MonoBehaviour player = col.gameObject.GetComponent<MonoBehaviour>();
            if (player != null)
            {
                ApplyPowerup(player);
            }

            // Destroy the star powerup object
            DestroyPowerup();
        }
        else if (col.gameObject.layer == 10) // Hitting a pipe
        {
            if (spawned)
            {
                goRight = !goRight;
                rigidBody.AddForce(Vector2.right * 3 * (goRight ? 1 : -1), ForceMode2D.Impulse);
            }
        }
    }

    public override void SpawnPowerup()
    {
        spawned = true;
        rigidBody.AddForce(Vector2.right * 3, ForceMode2D.Impulse); // Move to the right
    }

    public override void ApplyPowerup(MonoBehaviour i)
    {
        PlayerMovement pm = i.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.StartCoroutine(pm.InvincibilityCoroutine(invincibilityDuration));
        }
    }
}
