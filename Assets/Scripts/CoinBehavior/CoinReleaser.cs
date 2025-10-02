using System.Collections;
using UnityEngine;

[System.Serializable]
public class CoinReleaseSettings
{
    public GameObject coinPrefab;
    public AnimationClip coinFlyAnimation;
}

public class CoinReleaser : MonoBehaviour
{
    [Header("Coin Release Settings")]
    [SerializeField] public CoinReleaseSettings coinReleaseSettings;

    private CoinBehavior coinBehavior;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (coinReleaseSettings.coinPrefab != null)
        {
            SetupCoin();
        }
        else
        {
            Debug.LogError("Coin prefab not assigned in CoinReleaser on " + gameObject.name);
        }
    }

    private void SetupCoin()
    {
        GameObject spawnedCoin = Instantiate(coinReleaseSettings.coinPrefab, transform.position, Quaternion.identity);
        GameObject childCoin = spawnedCoin.transform.GetChild(0).gameObject;

        coinBehavior = childCoin.GetComponent<CoinBehavior>();
        Debug.Assert(coinBehavior != null, "CoinBehavior component missing from CoinReleaser");

        SpriteRenderer coinSpriteRenderer = childCoin.GetComponent<SpriteRenderer>();
        if (coinSpriteRenderer != null)
        {
            coinSpriteRenderer.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReleaseCoin()
    {
        StartCoroutine(coinBehavior.SpawnAndAnimateCoin());
    }

}