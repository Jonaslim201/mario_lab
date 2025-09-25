using UnityEngine;

public class GunManager : MonoBehaviour
{
    [SerializeField] private GunInstance[] guns;
    private int currentGunIndex = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize all gun instances
        for (int i = 0; i < guns.Length; i++)
        {
            if (guns[i] != null)
            {
                guns[i].gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()) && i <= guns.Length)
            {
                EquipGun(i - 1);
                return;
            }
        }
        
        if (Input.GetKeyDown("0"))
        {
            UnequipGun();
        }
    }

    private void EquipGun(int gunIndex){
        if (gunIndex < 0 || gunIndex >= guns.Length) return;
        if (currentGunIndex == gunIndex) return;
        
        // Unequip current gun
        if (currentGunIndex != -1)
        {
            guns[currentGunIndex].gameObject.SetActive(false);
        }

        // Equip new gun
        currentGunIndex = gunIndex;
        guns[currentGunIndex].gameObject.SetActive(true);

        Debug.Log($"Equipped {guns[currentGunIndex].gunData.gunName}");
    }

    private void UnequipGun(){
        if (currentGunIndex != -1)
        {
            Debug.Log($"Unequipped {guns[currentGunIndex].gunData.gunName}");
            guns[currentGunIndex].gameObject.SetActive(false);
            currentGunIndex = -1;
        }
    }

    public int GetCurrentGunIndex()
    {
        return currentGunIndex;
    }

    public GunInstance GetCurrentGunInstance()
    {
        return currentGunIndex != -1 ? guns[currentGunIndex] : null;
    }
    
    public GunInstance[] GetAllGunInstances()
    {
        return guns;
    }
}
