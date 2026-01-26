using System.Collections.Generic;

public enum MahjongHandTypes
{
    None,
    Pair, // 2 of a kind
    Set, // 3 of a kind
    Run, // 3 in a sequence
    Quad, // 4 of a kind
    ThreePairs, // 3 distinct pairs
    SetAndRun, // A distinct set and run
    TwoRuns, // 2 distinct runs
    TwoSets, // 2 distinct sets
    TwoQuads, // 2 distinct quads
    ThreeSets, // 3 distinct sets
    NineRun, // 9 in a sequence
    AllPairs, // Full hand of pairs
    FullWin, // Full hand consisting of 1 pair and a combination of sets and runs
}

public class MahjongHands
{
    public static (MahjongHandTypes type, List<Tile> tiles) GetMahjongHand(List<Tile> tiles)
    {
        // list1.AddRange(list2); to concat lists
        // TODO: Implement hand checking
        return (MahjongHandTypes.None, null);
    }
}
