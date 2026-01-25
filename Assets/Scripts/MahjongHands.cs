using System.Collections.Generic;
using System.Linq;

public enum MahjongHandTypes
{
    None,
    Pair,
    Set,
    Quad,
    Run,
    NineRun,
    FullWin,
    SetAndRun,
    TwoRuns,
    TwoSets,
    TwoQuads,
    ThreeSets,
    SevenPairs
}

public class MahjongHands
{
    // Can be tuned to balance the game
    public static readonly IReadOnlyDictionary<MahjongHandTypes, int> HandScores = new Dictionary<MahjongHandTypes, int>
    {
        { MahjongHandTypes.None, 0 },
        { MahjongHandTypes.Pair, 2 },
        { MahjongHandTypes.Set, 4 },
        { MahjongHandTypes.Quad, 8 },
        { MahjongHandTypes.Run, 4 },
        { MahjongHandTypes.NineRun, 6 },
        { MahjongHandTypes.FullWin, 12 },
        { MahjongHandTypes.SetAndRun, 8 },
        { MahjongHandTypes.TwoRuns, 8 },
        { MahjongHandTypes.TwoSets, 8 },
        { MahjongHandTypes.TwoQuads, 16 },
        { MahjongHandTypes.ThreeSets, 12 },
        { MahjongHandTypes.SevenPairs, 14 }
    };

    public static MahjongHandTypes GetMahjongHand(List<Tile> tiles)
    {
        if (tiles == null || tiles.Count == 0)
            return MahjongHandTypes.None;

        // Remove null tiles
        var validTiles = tiles.Where(t => t != null).ToList();
        if (validTiles.Count == 0)
            return MahjongHandTypes.None;

        int tileCount = validTiles.Count;

        // Check for multi-tile patterns first (most specific)
        if (tileCount == 9)
        {
            if (IsNineRun(validTiles))
                return MahjongHandTypes.NineRun;
            if (IsThreeSets(validTiles))
                return MahjongHandTypes.ThreeSets;
        }

        if (tileCount == 8)
        {
            if (IsTwoQuads(validTiles))
                return MahjongHandTypes.TwoQuads;
        }

        if (tileCount == 6)
        {
            if (IsSetAndRun(validTiles))
                return MahjongHandTypes.SetAndRun;
            if (IsTwoRuns(validTiles))
                return MahjongHandTypes.TwoRuns;
            if (IsTwoSets(validTiles))
                return MahjongHandTypes.TwoSets;
        }

        // Check for single patterns
        if (tileCount == 4)
        {
            if (IsQuad(validTiles))
                return MahjongHandTypes.Quad;
        }

        if (tileCount == 3)
        {
            if (IsSet(validTiles))
                return MahjongHandTypes.Set;
            if (IsRun(validTiles))
                return MahjongHandTypes.Run;
        }

        if (tileCount == 2)
        {
            if (IsPair(validTiles))
                return MahjongHandTypes.Pair;
        }

        return MahjongHandTypes.None;
    }

    static bool IsPair(List<Tile> tiles)
    {
        if (tiles.Count != 2) return false;
        return tiles[0].type == tiles[1].type;
    }

    static bool IsSet(List<Tile> tiles)
    {
        if (tiles.Count != 3) return false;
        return tiles[0].type == tiles[1].type && tiles[1].type == tiles[2].type;
    }

    static bool IsQuad(List<Tile> tiles)
    {
        if (tiles.Count != 4) return false;
        return tiles.All(t => t.type == tiles[0].type);
    }

    static bool IsRun(List<Tile> tiles)
    {
        if (tiles.Count != 3) return false;
        
        // Runs only work with numbered suits (Sticks, Circles, Numbers)
        var numberedTiles = tiles.Where(t => IsNumberedSuit(t.suit)).ToList();
        if (numberedTiles.Count != 3) return false;

        // All must be same suit
        if (numberedTiles[0].suit != numberedTiles[1].suit || numberedTiles[1].suit != numberedTiles[2].suit)
            return false;

        var values = numberedTiles.Select(GetNumericValue).OrderBy(v => v).ToList();
        return values[0] + 1 == values[1] && values[1] + 1 == values[2];
    }

    static bool IsNineRun(List<Tile> tiles)
    {
        if (tiles.Count != 9) return false;

        // All must be same numbered suit
        var numberedTiles = tiles.Where(t => IsNumberedSuit(t.suit)).ToList();
        if (numberedTiles.Count != 9) return false;

        var suit = numberedTiles[0].suit;
        if (!numberedTiles.All(t => t.suit == suit)) return false;

        var values = numberedTiles.Select(GetNumericValue).OrderBy(v => v).ToList();
        // Must be 1-9
        for (int i = 0; i < 9; i++)
        {
            if (values[i] != i + 1) return false;
        }

        return true;
    }

    static bool IsSetAndRun(List<Tile> tiles)
    {
        if (tiles.Count != 6) return false;

        // Try all combinations: 3 tiles for set, 3 for run
        for (int i = 0; i < tiles.Count; i++)
        {
            for (int j = i + 1; j < tiles.Count; j++)
            {
                for (int k = j + 1; k < tiles.Count; k++)
                {
                    var setCandidates = new List<Tile> { tiles[i], tiles[j], tiles[k] };
                    var runCandidates = tiles.Except(setCandidates).ToList();

                    if (IsSet(setCandidates) && IsRun(runCandidates))
                        return true;
                }
            }
        }

        return false;
    }

    static bool IsTwoRuns(List<Tile> tiles)
    {
        if (tiles.Count != 6) return false;

        // Try all combinations: 3 tiles for first run, 3 for second run
        for (int i = 0; i < tiles.Count; i++)
        {
            for (int j = i + 1; j < tiles.Count; j++)
            {
                for (int k = j + 1; k < tiles.Count; k++)
                {
                    var run1Candidates = new List<Tile> { tiles[i], tiles[j], tiles[k] };
                    var run2Candidates = tiles.Except(run1Candidates).ToList();

                    if (IsRun(run1Candidates) && IsRun(run2Candidates))
                        return true;
                }
            }
        }

        return false;
    }

    static bool IsTwoSets(List<Tile> tiles)
    {
        if (tiles.Count != 6) return false;

        // Group by type and count
        var typeGroups = tiles.GroupBy(t => t.type).ToList();
        
        // Need exactly 2 groups, each with 3 tiles
        if (typeGroups.Count != 2) return false;
        return typeGroups.All(g => g.Count() == 3);
    }

    static bool IsTwoQuads(List<Tile> tiles)
    {
        if (tiles.Count != 8) return false;

        // Group by type and count
        var typeGroups = tiles.GroupBy(t => t.type).ToList();
        
        // Need exactly 2 groups, each with 4 tiles
        if (typeGroups.Count != 2) return false;
        return typeGroups.All(g => g.Count() == 4);
    }

    static bool IsThreeSets(List<Tile> tiles)
    {
        if (tiles.Count != 9) return false;

        // Group by type and count
        var typeGroups = tiles.GroupBy(t => t.type).ToList();
        
        // Need exactly 3 groups, each with 3 tiles
        if (typeGroups.Count != 3) return false;
        return typeGroups.All(g => g.Count() == 3);
    }

    static bool IsNumberedSuit(TileSuit suit)
    {
        return suit == TileSuit.Sticks || suit == TileSuit.Circles || suit == TileSuit.Numbers;
    }

    static int GetNumericValue(Tile tile)
    {
        return tile.type switch
        {
            TileType.OneSticks or TileType.OneCircles or TileType.OneNumbers => 1,
            TileType.TwoSticks or TileType.TwoCircles or TileType.TwoNumbers => 2,
            TileType.ThreeSticks or TileType.ThreeCircles or TileType.ThreeNumbers => 3,
            TileType.FourSticks or TileType.FourCircles or TileType.FourNumbers => 4,
            TileType.FiveSticks or TileType.FiveCircles or TileType.FiveNumbers => 5,
            TileType.SixSticks or TileType.SixCircles or TileType.SixNumbers => 6,
            TileType.SevenSticks or TileType.SevenCircles or TileType.SevenNumbers => 7,
            TileType.EightSticks or TileType.EightCircles or TileType.EightNumbers => 8,
            TileType.NineSticks or TileType.NineCircles or TileType.NineNumbers => 9,
            _ => -1 // Honor tiles
        };
    }

    public static int GetScoreForHand(MahjongHandTypes handType)
    {
        return HandScores.TryGetValue(handType, out int score) ? score : 0;
    }
}
