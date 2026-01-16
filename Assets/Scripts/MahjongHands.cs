using System.Collections.Generic;

public enum MahjongHandTypes
{
    None,
    Pair,
    ThreeSet,
    FourSet,
    ThreeRun,
    NineRun,
    FullWin,
}

public class MahjongHands
{
    public static MahjongHandTypes GetMahjongHand(List<Tile> tiles)
    {
        // TODO: Implement hand checking
        return MahjongHandTypes.None;
    }
}
