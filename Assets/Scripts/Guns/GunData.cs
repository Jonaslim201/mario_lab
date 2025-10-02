using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Weapons/Gun")]
public class GunData : ScriptableObject
{
    [Header("Gun Identity")]
    public string gunName;
    public Sprite gunIcon;

    [Header("Gun Prefabs")]
    public GameObject gunPrefab;
    public GameObject bulletPrefab;

    [Header("Gun Properties")]
    public float bulletSpeed = 50f;
    public float gunDistance = 2f;

    [Header("Recoil Settings")]
    public float recoilAngle = 90f;
    public float recoilDuration = 0.8f;

    [Header("Muzzle Flash Settings")]
    public float fadeInTime = 0.02f;
    public float fadeOutTime = 0.08f;
}
