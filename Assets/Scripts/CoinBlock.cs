using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class CoinBlock : MonoBehaviour
{
    [SerializeField] public GameObject coinPrefab; // Assign your coin prefab in inspector
    [SerializeField] public AnimationClip coinFlyAnimation; // Assign your coin fly animation in inspector
    private CoinBehavior coinBehavior;
    private Animator coinAnimator;
    private GameObject childCoin;
    private SpriteRenderer coinSpriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject spawnedCoin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
        childCoin = spawnedCoin.transform.GetChild(0).gameObject;

        coinBehavior = childCoin.GetComponent<CoinBehavior>();
        coinAnimator = childCoin.GetComponent<Animator>();
        coinSpriteRenderer = childCoin.GetComponent<SpriteRenderer>();
        coinSpriteRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReleaseCoin()
    {
        StartCoroutine(SpawnAndAnimateCoin());
    }

    public IEnumerator SpawnAndAnimateCoin()
    {
        coinSpriteRenderer.enabled = true;

        coinAnimator.SetTrigger("PlayerHit");
        Debug.Log("Coin spawned and animation triggered");

        yield return new WaitForSeconds(coinFlyAnimation.length);

        coinSpriteRenderer.enabled = false;

        coinBehavior.PlayCoinSound();
    }
}
