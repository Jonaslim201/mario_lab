using UnityEngine;

[CreateAssetMenu(fileName = "PowerupData", menuName = "Game/Powerup Data")]
public class PowerupData : ScriptableObject
{
    public PowerupType powerupType;
    public bool isSpawned;
    public bool isConsumed;
    public Vector3 initialPosition;
    public AudioClip spawnSound;

    // You can add other fields as needed (e.g., spawn count, duration left, etc.)

    public void ResetState()
    {
        isSpawned = true;
        isConsumed = false;
        // Position reset depends on how you use it in game

    }
}
