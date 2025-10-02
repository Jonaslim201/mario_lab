using UnityEngine;

public class GunController : MonoBehaviour
{

    private bool isRecoiling = false;
    private bool gunFacingRight = true;
    private float baseRotation; // The rotation the gun should be at without recoil
    private int currentGunIndex = -1;
    public float slowMotionScale = 0.1f;

    private GunManager gunManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gunManager = GetComponent<GunManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get current gun from GunManager
        GunInstance currentGunInstance = gunManager.GetCurrentGunInstance();

        if (currentGunInstance == null) return;

        Transform gun = currentGunInstance.gunTransform;
        GunData gunData = currentGunInstance.gunData;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;

        // Calculate the target rotation based on mouse position
        baseRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // update rotation if not recoiling
        if (!isRecoiling)
        {
            gun.rotation = Quaternion.Euler(new Vector3(0, 0, baseRotation));
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gun.position = transform.position + Quaternion.Euler(0, 0, angle) * new Vector3(gunData.gunDistance, 0, 0);

        if (Input.GetKeyDown("mouse 0"))
        {
            Shoot(direction, gun, currentGunInstance, gunData);
        }

        if (Input.GetKeyDown("r") && gunData.gunName == "AWP")
        {
            Debug.Log($"Right click - Slow motion activated");
            Time.timeScale = slowMotionScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        if (Input.GetKeyUp("r"))
        {
            Debug.Log($"Normal time restored");
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f;
        }

        GunFlipController(mousePos, gun);
    }

    private void GunFlip(Transform gun)
    {
        gunFacingRight = !gunFacingRight;
        gun.localScale = new Vector3(gun.localScale.x, gun.localScale.y * -1, gun.localScale.z);
    }

    private void GunFlipController(Vector3 mousePos, Transform gun)
    {
        if (mousePos.x < gun.position.x && gunFacingRight)
        {
            GunFlip(gun);
        }
        else if (mousePos.x > gun.position.x && !gunFacingRight)
        {
            GunFlip(gun);
        }
    }

    public void Shoot(Vector3 direction, Transform gun, GunInstance gunInstance, GunData gunData)
    {

        if (gun == null) return;

        StartCoroutine(RecoilEffect(gun, gunData));
        StartCoroutine(MuzzleFlashEffect(gunInstance, gunData));

        GameObject newBullet = Instantiate(gunData.bulletPrefab, gun.position, gun.rotation);
        newBullet.GetComponent<Rigidbody2D>().linearVelocity = direction.normalized * gunData.bulletSpeed;

        Destroy(newBullet, 5f);
    }

    private System.Collections.IEnumerator RecoilEffect(Transform gun, GunData gunData)
    {
        isRecoiling = true;

        float startRotation = baseRotation;
        float recoilDirection = gunFacingRight ? gunData.recoilAngle : -gunData.recoilAngle;
        float recoilRotation = baseRotation + recoilDirection; // Kick upward

        // Kick up quickly
        float elapsed = 0f;
        while (elapsed < gunData.recoilDuration * 0.1f)
        {
            float currentRotation = Mathf.Lerp(startRotation, recoilRotation, elapsed / (gunData.recoilDuration * 0.3f));
            gun.rotation = Quaternion.Euler(new Vector3(0, 0, currentRotation));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return to normal position slowly
        elapsed = 0f;
        while (elapsed < gunData.recoilDuration * 0.7f)
        {
            float currentRotation = Mathf.Lerp(recoilRotation, baseRotation, elapsed / (gunData.recoilDuration * 0.7f));
            gun.rotation = Quaternion.Euler(new Vector3(0, 0, currentRotation));
            elapsed += Time.deltaTime;
            yield return null;
        }

        isRecoiling = false;
    }

    private System.Collections.IEnumerator MuzzleFlashEffect(GunInstance gunInstance, GunData gunData)
    {
        if (gunInstance.muzzleFlash == null)
        {
            Debug.LogError("Muzzle flash is null!");
            yield break;
        }

        SpriteRenderer muzzleFlash = gunInstance.muzzleFlash;
        Color flashColor = muzzleFlash.color;

        Debug.Log($"Starting muzzle flash. Initial alpha: {flashColor.a}");

        float elapsed = 0f;
        while (elapsed < gunData.fadeInTime)
        {
            flashColor.a = Mathf.Lerp(0f, 1f, elapsed / gunData.fadeInTime);
            muzzleFlash.color = flashColor;
            elapsed += Time.deltaTime;
            yield return null;
        }

        flashColor.a = 1f;
        muzzleFlash.color = flashColor;

        Debug.Log($"Muzzle flash at full brightness: {flashColor.a}");

        elapsed = 0f;
        while (elapsed < gunData.fadeOutTime)
        {
            flashColor.a = Mathf.Lerp(1f, 0f, elapsed / gunData.fadeOutTime);
            muzzleFlash.color = flashColor;
            elapsed += Time.deltaTime;
            yield return null;
        }

        flashColor.a = 0f;
        muzzleFlash.color = flashColor;
        Debug.Log($"Muzzle flash effect complete. Final alpha: {flashColor.a}");
    }
}
