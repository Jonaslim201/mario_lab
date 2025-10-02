public class RedBrickEmpty : BaseBlock
{
    protected override void Awake()
    {
        base.Awake();

        // Configure this block type
        canReleaseCoins = false;
        staysBouncy = true;
        hasAnimation = false;
    }
}