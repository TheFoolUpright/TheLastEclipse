public static class GameProgress
{
    public static bool fleeSoulCollected;
    public static bool attackSoulCollected;

    public static bool BothMainSoulsCollected =>
        fleeSoulCollected && attackSoulCollected;
}