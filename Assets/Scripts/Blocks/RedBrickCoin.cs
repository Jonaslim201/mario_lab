public class RedBrickCoin : BaseBlock
{
    private CoinReleaser coinReleaser;

    protected override void Awake()
    {
        base.Awake();

        // Configure this block type
        canReleaseCoins = true;
        staysBouncy = true;
        hasAnimation = false;

        coinReleaser = GetComponent<CoinReleaser>();
    }

    protected override void ReleaseCoin()
    {
        if (coinReleaser != null)
        {
            coinReleaser.ReleaseCoin();
            canReleaseCoins = false;
        }
    }

    public override void ResetBlock()
    {
        base.ResetBlock();
        if (coinReleaser != null)
        {
            coinReleaser.ResetCoin();
        }
    }

}