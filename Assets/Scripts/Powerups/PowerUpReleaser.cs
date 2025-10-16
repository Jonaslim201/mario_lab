using System.Collections;
using UnityEngine;

[System.Serializable]
public class PowerUpReleaseSettings
{
    public GameObject powerUpPrefab;
    public PowerupType powerupType;
    public float spawnHeight = 1f;
    public float emergeDuration = 0.5f;
}

public class PowerUpReleaser : MonoBehaviour
{
    [SerializeField] private PowerUpReleaseSettings powerUpSettings;

    private BasePowerup powerUpBehavior;
    private GameObject spawnedPowerUp;
    private bool hasReleased = false;

    void Start()
    {
        Debug.Assert(powerUpSettings.powerUpPrefab != null, "PowerUp prefab is not assigned in " + gameObject.name);
        SetupPowerUp();
    }

    public void ReleasePowerUp()
    {
        Debug.Log("Attempting to release powerup from " + gameObject.name);
        if (hasReleased)
        {
            Debug.Log("PowerUp already released from this block");
            return;
        }

        if (powerUpSettings.powerUpPrefab != null)
        {
            Debug.Log("Releasing powerup...");
            hasReleased = true;
            StartCoroutine(SpawnAndEmergePowerUp());
        }
    }

    private void SetupPowerUp()
    {
        // Instantiate powerup at block position
        spawnedPowerUp = Instantiate(
            powerUpSettings.powerUpPrefab,
            transform.position,
            Quaternion.identity
        );

        // Get the powerup behavior component
        powerUpBehavior = spawnedPowerUp.GetComponent<BasePowerup>();

        if (powerUpBehavior == null)
        {
            Debug.LogError("PowerUp prefab doesn't have BasePowerup component!");
            return;
        }

        // Initially disable physics and hide
        Rigidbody2D rb = spawnedPowerUp.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        SpriteRenderer sprite = spawnedPowerUp.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.enabled = false;
        }

        // Disable collider during emerge animation
        Collider2D collider = spawnedPowerUp.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    private IEnumerator SpawnAndEmergePowerUp()
    {
        if (spawnedPowerUp == null) yield break;

        SpriteRenderer sprite = spawnedPowerUp.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.enabled = true;
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * powerUpSettings.spawnHeight;

        float elapsedTime = 0f;

        if (powerUpBehavior.powerupData.spawnSound != null)
        {
            powerUpBehavior.powerupSource.PlayOneShot(powerUpBehavior.powerupData.spawnSound);
        }

        // Animate powerup emerging from block
        while (elapsedTime < powerUpSettings.emergeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / powerUpSettings.emergeDuration;

            if (spawnedPowerUp != null)
            {
                spawnedPowerUp.transform.position = Vector3.Lerp(startPos, endPos, t);
            }

            yield return null;
        }

        // Ensure final position
        if (spawnedPowerUp != null)
        {
            spawnedPowerUp.transform.position = endPos;

            // Enable physics and collision
            Rigidbody2D rb = spawnedPowerUp.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 1f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            Collider2D collider = spawnedPowerUp.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            // Activate powerup behavior (starts moving)
            if (powerUpBehavior != null)
            {
                powerUpBehavior.SpawnPowerup();
            }
        }
    }

    public void ResetPowerUp()
    {
        hasReleased = false;

        if (spawnedPowerUp != null)
        {
            Destroy(spawnedPowerUp);
            spawnedPowerUp = null;
        }

        powerUpBehavior = null;
        SetupPowerUp();
    }

    void OnEnable()
    {
        GameManager.OnGameRestart += ResetPowerUp;
    }

    void OnDisable()
    {
        GameManager.OnGameRestart -= ResetPowerUp;
    }
}