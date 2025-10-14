using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMushroomPowerup : BasePowerup
{
    // setup this object's type
    // instantiate variables
    protected override void Start()
    {
        base.Start(); // call base class Start()
        this.type = PowerupType.MagicMushroom;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && powerupData.isSpawned)
        {
            // Apply powerup to the player when colliding
            ApplyPowerup(col.gameObject.GetComponent<MonoBehaviour>());

            // then destroy powerup (optional)
            powerupData.isConsumed = true;
            gameObject.SetActive(false);
        }
        else if (col.gameObject.layer == 10) // else if hitting Pipe, flip travel direction
        {
            if (powerupData.isSpawned)
            {
                goRight = !goRight;
                rigidBody.AddForce(Vector2.right * 3 * (goRight ? 1 : -1), ForceMode2D.Impulse);
            }
        }
    }
    // interface implementation
    public override void SpawnPowerup()
    {
        powerupData.isSpawned = true;
        rigidBody.AddForce(Vector2.right * 3, ForceMode2D.Impulse); // move to the right
    }

    // interface implementation
    public override void ApplyPowerup(MonoBehaviour i)
    {
        // Example effect: Increase player's size ("grow" Mario)
        // For a real game, use your own player controller script type instead of MonoBehaviour.

        if (i != null)
        {
            i.transform.localScale *= 1.5f; // Grow the player (works if scale change is desired)

            // If your PlayerScript has a Grow() method:
            // PlayerScript player = i as PlayerScript;
            // if (player != null) { player.Grow(); }
        }
    }
}
