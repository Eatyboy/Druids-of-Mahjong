using System.Collections.Generic;
using System.Linq;

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
    public static readonly IReadOnlyDictionary<MahjongHandTypes, int> HandScores = new Dictionary<MahjongHandTypes, int>
    {
        { MahjongHandTypes.None, 0 },
        { MahjongHandTypes.Pair, 2 },
        { MahjongHandTypes.Set, 4 },
        { MahjongHandTypes.Run, 4 },
        { MahjongHandTypes.Quad, 8 },
        { MahjongHandTypes.ThreePairs, 8 },
        { MahjongHandTypes.SetAndRun, 8 },
        { MahjongHandTypes.TwoRuns, 8 },
        { MahjongHandTypes.TwoSets, 8 },
        { MahjongHandTypes.TwoQuads, 16 },
        { MahjongHandTypes.ThreeSets, 12 },
        { MahjongHandTypes.NineRun, 6 },
        { MahjongHandTypes.AllPairs, 14 },
        { MahjongHandTypes.FullWin, 12 }
    };

    public static MahjongHandTypes GetMahjongHand(List<Tile> tiles)
    {
        if (tiles == null || tiles.Count == 0)
            return MahjongHandTypes.None;

        var valid = tiles.Where(t => t != null).ToList();
        if (valid.Count == 0)
            return MahjongHandTypes.None;

        int n = valid.Count;

        if (n == 14)
        {
            if (IsAllPairs(valid)) return MahjongHandTypes.AllPairs;
            if (IsFullWin(valid)) return MahjongHandTypes.FullWin;
        }

        if (n == 9)
        {
            if (IsNineRun(valid)) return MahjongHandTypes.NineRun;
            if (IsThreeSets(valid)) return MahjongHandTypes.ThreeSets;
        }

        if (n == 8 && IsTwoQuads(valid))
            return MahjongHandTypes.TwoQuads;

        if (n == 6)
        {
            if (IsSetAndRun(valid)) return MahjongHandTypes.SetAndRun;
            if (IsTwoRuns(valid)) return MahjongHandTypes.TwoRuns;
            if (IsTwoSets(valid)) return MahjongHandTypes.TwoSets;
            if (IsThreePairs(valid)) return MahjongHandTypes.ThreePairs;
        }

        if (n == 4 && IsQuad(valid))
            return MahjongHandTypes.Quad;

        if (n == 3)
        {
            if (IsSet(valid)) return MahjongHandTypes.Set;
            if (IsRun(valid)) return MahjongHandTypes.Run;
        }

        if (n == 2 && IsPair(valid))
            return MahjongHandTypes.Pair;

        return MahjongHandTypes.None;
    }

    public static int GetScoreForHand(MahjongHandTypes handType)
    {
        return HandScores.TryGetValue(handType, out int score) ? score : 0;
    }

    static bool SameTile(Tile a, Tile b) => a.suit == b.suit && a.rank == b.rank;

    static bool IsPair(List<Tile> t)
    {
        if (t.Count != 2) return false;
        return SameTile(t[0], t[1]);
    }

    static bool IsSet(List<Tile> t)
    {
        if (t.Count != 3) return false;
        return SameTile(t[0], t[1]) && SameTile(t[1], t[2]);
    }

    static bool IsQuad(List<Tile> t)
    {
        if (t.Count != 4) return false;
        return t.All(x => SameTile(x, t[0]));
    }

    static bool IsNumberedSuit(TileSuit s) =>
        s == TileSuit.Bamboo || s == TileSuit.Dot || s == TileSuit.Character;

    static bool IsRun(List<Tile> t)
    {
        if (t.Count != 3) return false;
        if (!t.All(x => IsNumberedSuit(x.suit))) return false;
        if (t[0].suit != t[1].suit || t[1].suit != t[2].suit) return false;
        var r = t.Select(x => x.rank).OrderBy(v => v).ToList();
        return r[0] >= 1 && r[0] <= 7 && r[0] + 1 == r[1] && r[1] + 1 == r[2];
    }

    static bool IsNineRun(List<Tile> t)
    {
        if (t.Count != 9) return false;
        if (!t.All(x => IsNumberedSuit(x.suit))) return false;
        var suit = t[0].suit;
        if (!t.All(x => x.suit == suit)) return false;
        var r = t.Select(x => x.rank).OrderBy(v => v).ToList();
        for (int i = 0; i < 9; i++)
            if (r[i] != i + 1) return false;
        return true;
    }

    static bool IsThreePairs(List<Tile> t)
    {
        if (t.Count != 6) return false;
        var groups = t.GroupBy(x => (x.suit, x.rank)).ToList();
        return groups.Count == 3 && groups.All(g => g.Count() == 2);
    }

    static bool IsSetAndRun(List<Tile> t)
    {
        if (t.Count != 6) return false;
        for (int i = 0; i < t.Count; i++)
            for (int j = i + 1; j < t.Count; j++)
                for (int k = j + 1; k < t.Count; k++)
                {
                    var set = new List<Tile> { t[i], t[j], t[k] };
                    var run = t.Except(set).ToList();
                    if (IsSet(set) && IsRun(run)) return true;
                }
        return false;
    }

    static bool IsTwoRuns(List<Tile> t)
    {
        if (t.Count != 6) return false;
        for (int i = 0; i < t.Count; i++)
            for (int j = i + 1; j < t.Count; j++)
                for (int k = j + 1; k < t.Count; k++)
                {
                    var r1 = new List<Tile> { t[i], t[j], t[k] };
                    var r2 = t.Except(r1).ToList();
                    if (IsRun(r1) && IsRun(r2)) return true;
                }
        return false;
    }

    static bool IsTwoSets(List<Tile> t)
    {
        if (t.Count != 6) return false;
        var groups = t.GroupBy(x => (x.suit, x.rank)).ToList();
        return groups.Count == 2 && groups.All(g => g.Count() == 3);
    }

    static bool IsTwoQuads(List<Tile> t)
    {
        if (t.Count != 8) return false;
        var groups = t.GroupBy(x => (x.suit, x.rank)).ToList();
        return groups.Count == 2 && groups.All(g => g.Count() == 4);
    }

    static bool IsThreeSets(List<Tile> t)
    {
        if (t.Count != 9) return false;
        var groups = t.GroupBy(x => (x.suit, x.rank)).ToList();
        return groups.Count == 3 && groups.All(g => g.Count() == 3);
    }

    static bool IsAllPairs(List<Tile> t)
    {
        if (t.Count != 14) return false;
        var groups = t.GroupBy(x => (x.suit, x.rank)).ToList();
        return groups.Count == 7 && groups.All(g => g.Count() == 2);
    }

    static bool IsFullWin(List<Tile> t)
    {
        if (t.Count != 14) return false;
        for (int i = 0; i < t.Count; i++)
            for (int j = i + 1; j < t.Count; j++)
            {
                if (!SameTile(t[i], t[j])) continue;
                var rest = t.Where((x, idx) => idx != i && idx != j).ToList();
                if (CanFormKMelds(rest, 4)) return true;
            }
        return false;
    }

    static bool CanFormKMelds(List<Tile> t, int k)
    {
        if (k == 0) return t.Count == 0;
        if (t.Count != k * 3) return false;
        for (int i = 0; i < t.Count; i++)
            for (int j = i + 1; j < t.Count; j++)
                for (int m = j + 1; m < t.Count; m++)
                {
                    var meld = new List<Tile> { t[i], t[j], t[m] };
                    if (!IsSet(meld) && !IsRun(meld)) continue;
                    var rest = t.Except(meld).ToList();
                    if (CanFormKMelds(rest, k - 1)) return true;
                }
        return false;
    }
}
