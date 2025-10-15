using System.Collections;
using UnityEngine;

public class QuestionMarkBlock : BaseBlock
{
    [SerializeField] public Animator animator;
    private CoinReleaser coinReleaser;

    protected override void Awake()
    {
        base.Awake();

        // Configure this block type
        canReleaseCoins = true;
        staysBouncy = false; // Becomes inactive after first hit
        hasAnimation = true;

        coinReleaser = GetComponent<CoinReleaser>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        if (animator)
        {
            animator.SetBool("isActive", true);
        }
    }

    protected override void PlayAnimation()
    {
        if (animator)
        {
            Debug.Log("QuestionMarkBlock: Playing animation");
            animator.SetBool("isActive", false);
        }
    }

    protected override void ReleaseCoin()
    {
        if (coinReleaser != null)
        {
            coinReleaser.ReleaseCoin();
        }
    }

    public override void ResetBlock()
    {
        base.ResetBlock();
        if (animator != null)
        {
            animator.SetBool("isActive", true);
        }
        if (coinReleaser != null)
        {
            coinReleaser.ResetCoin();
        }
    }
}
