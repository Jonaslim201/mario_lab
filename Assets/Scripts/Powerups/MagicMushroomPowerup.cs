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
        if (!powerupData.isSpawned)
            return;

        if (col.gameObject.CompareTag("Player"))
        {
            // Apply powerup to the player when colliding
            ApplyPowerup(col.gameObject.GetComponent<MonoBehaviour>());

            // then destroy powerup (optional)
            powerupData.isConsumed = true;
            DestroyPowerup();
        }
        else if (col.gameObject.CompareTag("Pipe")) // else if hitting Pipe, flip travel direction
        {
            goRight = !goRight;
            rigidBody.AddForce(Vector2.right * 3 * (goRight ? 1 : -1), ForceMode2D.Impulse);
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

        if (i != null)
        {
            i.transform.localScale *= 1.5f;
        }
    }
}
