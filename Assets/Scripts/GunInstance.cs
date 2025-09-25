using UnityEngine;

public class GunInstance : MonoBehaviour
{
    [Header("Gun References")]
    public GunData gunData;

    [System.NonSerialized]
    public Transform gunTransform;

    [System.NonSerialized]
    public SpriteRenderer muzzleFlash;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gunData.gunPrefab != null && gunTransform == null)
        {
            GameObject gunObj = Instantiate(gunData.gunPrefab, transform);
            gunTransform = gunObj.transform;


            GameObject muzzleFlashObj = null;

            foreach (Transform child in gunTransform)
            {
                if (child.CompareTag("MuzzleFlash"))
                {
                    muzzleFlashObj = child.gameObject;
                    break;
                }
            }
            
            if (muzzleFlashObj != null)
            {
                muzzleFlash = muzzleFlashObj.GetComponent<SpriteRenderer>();
                Debug.Log($"Found muzzle flash: {muzzleFlash.name}");
            }

        } else {
            // throw error
            Debug.LogError("GunInstance: gunPrefab or gunTransform is not set!");
        }

        // Initialize muzzle flash
        if (muzzleFlash != null)
        {
            Color flashColor = muzzleFlash.color;
            flashColor.a = 0f;
            muzzleFlash.color = flashColor;
            Debug.Log($"Muzzle flash initialized with alpha: {muzzleFlash.color.a}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        if (muzzleFlash != null)
        {
            Color flashColor = muzzleFlash.color;
            flashColor.a = 0f;
            muzzleFlash.color = flashColor;
            Debug.Log($"Muzzle flash reset on enable: {muzzleFlash.color.a}");
        }
    }
}
