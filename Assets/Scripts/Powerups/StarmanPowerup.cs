using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarmanPowerup : BasePowerup
{
    [SerializeField] private float invincibilityDuration = 10f; // Duration of invincibility in seconds
    [SerializeField] private float flashInterval = 0.1f; // Flash speed during invincibility
    private Coroutine invincibilityCoroutine;


    protected override void Start()
    {
        base.Start();
        this.type = PowerupType.StarMan;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && powerupData.isSpawned)
        {
            // Apply invincibility effect to player
            MonoBehaviour player = col.gameObject.GetComponent<MonoBehaviour>();
            if (player != null)
            {
                ApplyPowerup(player);
            }
            powerupData.isConsumed = true;
            // Destroy the star powerup object
            DestroyPowerup();
        }
        else if (col.gameObject.layer == 10) // Hitting a pipe
        {
            if (powerupData.isSpawned)
            {
                goRight = !goRight;
                rigidBody.AddForce(Vector2.right * 3 * (goRight ? 1 : -1), ForceMode2D.Impulse);
            }
        }
    }

    public override void SpawnPowerup()
    {
        powerupData.isSpawned = true;
        rigidBody.AddForce(Vector2.right * 3, ForceMode2D.Impulse); // Move to the right
    }

    public override void ApplyPowerup(MonoBehaviour i)
    {
        PlayerMovement pm = i.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.StartInvincibility(invincibilityDuration);
        }
    }

    public void StopInvincibility(PlayerMovement pm)
    {
        if (invincibilityCoroutine != null)
        {
            pm.StopCoroutine(invincibilityCoroutine);
            invincibilityCoroutine = null;
        }
    }
}
