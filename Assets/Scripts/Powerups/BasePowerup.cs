using UnityEngine;


public abstract class BasePowerup : MonoBehaviour, IPowerup
{
    public PowerupType type;
    protected bool goRight = true;
    public PowerupData powerupData; // Reference to SO

    protected Rigidbody2D rigidBody;

    // base methods
    protected virtual void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        if (powerupData != null)
        {
            powerupData.initialPosition = transform.position;
        }
    }

    public virtual void ResetPowerup()
    {
        if (powerupData != null)
        {
            powerupData.ResetState();
            transform.position = powerupData.initialPosition;
        }
        // Reset local states as well, if any
        Debug.Log("Setting this to trueeeee");
        gameObject.SetActive(true);
        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.angularVelocity = 0;
        // Other resets
    }

    // interface methods
    // 1. concrete methods
    public PowerupType powerupType
    {
        get // getter
        {
            return type;
        }
    }

    public bool hasSpawned
    {
        get // getter
        {
            return powerupData.isSpawned;
        }
    }

    public void DestroyPowerup()
    {
        Destroy(this.gameObject);
    }

    // 2. abstract methods, must be implemented by derived classes
    public abstract void SpawnPowerup();
    public abstract void ApplyPowerup(MonoBehaviour i);
}
