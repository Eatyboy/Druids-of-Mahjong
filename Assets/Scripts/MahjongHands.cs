using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public enum MahjongHandTypes
{
    None,
    Pair, // 2 of a kind
    Set, // 3 of a kind
    Run, // 3 in a sequence
    Quad, // 4 of a kind
    TwoPairs, // 2 distinct pairs
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
    // elements 0-4: pairs, sets, three runs, quads, nine runs
    public static async Task<List<(MahjongHandTypes type, List<Tile> tiles)>> GetAllHandCombinationsAsync(List<Tile> tiles, Tile required = null)
    {
        return await Task.Run(() =>
        {
            List<(MahjongHandTypes type, List<Tile> tiles)> result = new();
            if (tiles == null || tiles.Count == 0) return result;

            // Bitmask approach to iterate over all possible subsets of tiles
            int subsetCount = 1 << tiles.Count;
            for (int mask = 0; mask < subsetCount; mask++)
            {
                List<Tile> subset = new();
                for (int i = 0; i < tiles.Count; i++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        subset.Add(tiles[i]);
                    }
                }

                if (required != null && !subset.Contains(required)) continue;

                MahjongHandTypes handType = GetMahjongHand(subset);
                if (handType != MahjongHandTypes.None)
                {
                    result.Add((handType, subset));
                }
            }

            return result
                .OrderByDescending(hand => hand.type)
                .ThenByDescending(hand => hand.tiles.Count)
                .ToList();
        });
    }

    public static async Task<(MahjongHandTypes type, List<Tile> tiles)> GetOptimalHandAsync(List<Tile> tiles, Tile required = null)
    {
        if (tiles == null || tiles.Count == 0) return (MahjongHandTypes.None, null);

        var allHands = await GetAllHandCombinationsAsync(tiles, required);
        if (allHands.Count == 0) return (MahjongHandTypes.None, null);

        return allHands.First();
    }

    private static List<Tile> FindSetOfCombinationsFromAll(List<List<Tile>> allCombinationsList, int outputSize)
    {
        List<Tile> output = new();
        if (allCombinationsList == null) return null;

        foreach (List<Tile> tPivot in allCombinationsList)
        {
            output.Clear();
            output.AddRange(tPivot);

            foreach(List<Tile> t in allCombinationsList)
            {
                if (!CheckForCommonTile(output, t))
                {
                    output.AddRange(t);
                    if (output.Count == outputSize)
                    {
                        return output;
                    }
                }
            }
        }

        return null;
    }

    // Ethan: I deprecated in favor of LINQ
    //// implement merge sort for player hand (suit order based on MahjongTile TileSuit enum)
    //public static List<Tile> MahjongMergeSort(List<Tile> inputList, int leftIndex, int rightIndex)
    //{
    //    if (leftIndex < rightIndex)
    //    {
    //        int middleIndex = leftIndex + (rightIndex - leftIndex) / 2;
    //        MahjongMergeSort(inputList, leftIndex, middleIndex);
    //        MahjongMergeSort(inputList, middleIndex + 1, rightIndex);
    //        Merge(inputList, leftIndex, middleIndex, rightIndex);
    //    }

    //    return inputList;
    //}

    //// holy copy paste
    //private static void Merge(List<Tile> inputList, int leftIndex, int middleIndex, int rightIndex)
    //{
    //    int leftListLength = middleIndex - leftIndex + 1;
    //    int rightListLength = rightIndex - middleIndex;
    //    List<Tile> leftTempList = new();
    //    List<Tile> rightTempList = new();
    //    int i, j;
    //    for (i = 0; i < leftListLength; ++i)
    //        leftTempList.Add(inputList[leftIndex + i]);
    //    for (j = 0; j < rightListLength; ++j)
    //        rightTempList.Add(inputList[middleIndex + 1 + j]);
    //    i = 0;
    //    j = 0;
    //    int k = leftIndex;
    //    while (i < leftListLength && j < rightListLength)
    //    {
    //        // check suit first
    //        if ((int)leftTempList[i].suit < (int)rightTempList[j].suit)
    //        {
    //            inputList[k++] = leftTempList[i++];
    //            continue;
    //        }
    //        else if ((int)leftTempList[i].suit > (int)rightTempList[j].suit)
    //        {
    //            inputList[k++] = rightTempList[j++];
    //            continue;
    //        }

    //        // then rank
    //        if (leftTempList[i].rank <= rightTempList[j].rank)
    //        {
    //            inputList[k++] = leftTempList[i++];
    //        }
    //        else
    //        {
    //            inputList[k++] = rightTempList[j++];
    //        }
    //    }
    //    while (i < leftListLength)
    //    {
    //        inputList[k++] = leftTempList[i++];
    //    }
    //    while (j < rightListLength)
    //    {
    //        inputList[k++] = rightTempList[j++];
    //    }
    //} 

    // check if two lists contain the same tile
    private static bool CheckForCommonTile(List<Tile> list1, List<Tile> list2)
    {
        if (list1 == null || list2 == null) return false;
        // Use Any() to check if any element of list1 is present in list2
        return list1.Any(item1 => list2.Contains(item1));
    }

    // check if two lists contain the same tiles, in order
    private static bool ContainsSameTiles(List<Tile> list1, List<Tile> list2)
    {
        if (list1.Count != list2.Count) return false;
        for (int i = 0; i < list1.Count; i++)
        {
            if (!list1[i].Equals(list2[i])) return false;
        }
        return true;
    }

    public static void PrintTilesList(List<Tile> list)
    {
        if (list == null)
        {
            UnityEngine.Debug.Log("Null list");
            return;
        }
        if (list.Count == 0)
        {
            UnityEngine.Debug.Log("Empty list");
            return;
        }

        string msg = "{";
        foreach (Tile t in list)
        {
            msg += t.rank + " of " + t.suit  + ", ";
        }
        msg += "}";
        UnityEngine.Debug.Log(msg);
    }
    
    public static void Print2DTilesList(List<List<Tile>> list)
    {
        if (list == null)
        {
            UnityEngine.Debug.Log("Null list");
            return;
        }
        if (list.Count == 0)
        {
            UnityEngine.Debug.Log("Empty list");
            return;
        }

        string msg = "{";
        foreach (List<Tile> l in list)
        {
            msg += "{";
            foreach (Tile t in l)
            {
                msg += t.rank + " of " + t.suit + ", ";
            }
            msg += "}, ";
        }
        msg += "}";
        UnityEngine.Debug.Log(msg);
    }

    public static readonly IReadOnlyDictionary<MahjongHandTypes, int> HandScores = new Dictionary<MahjongHandTypes, int>
    {
        { MahjongHandTypes.None, 0 },
        { MahjongHandTypes.Pair, 2 },
        { MahjongHandTypes.Set, 4 },
        { MahjongHandTypes.Run, 4 },
        { MahjongHandTypes.Quad, 8 },
        { MahjongHandTypes.TwoPairs, 6 },
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

        if (n == 4)
        {
            if (IsQuad(valid)) return MahjongHandTypes.Quad;
            if (IsTwoPairs(valid)) return MahjongHandTypes.TwoPairs;
        }

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

    // Returns true if all tiles in the list count as "matching" for pair/set/quad.
    // With MixedSets flower tile and all numbered suits, only rank must match; otherwise exact tile (suit + rank) match.
    static bool AllTilesMatchForSet(List<Tile> t)
    {
        if (t == null || t.Count == 0) return false;
        bool useRankOnly = FlowerTileManager.instance != null
            && FlowerTileManager.instance.IsFlowerTileActive(FlowerTileType.MixedSets)
            && t.All(x => IsNumberedSuit(x.suit));
        if (useRankOnly)
            return t.All(x => x.rank == t[0].rank);
        return t.All(x => SameTile(x, t[0]));
    }

    static bool IsPair(List<Tile> t)
    {
        if (t.Count != 2) return false;
        return AllTilesMatchForSet(t);
    }

    static bool IsSet(List<Tile> t)
    {
        if (t.Count != 3) return false;
        return AllTilesMatchForSet(t);
    }

    static bool IsQuad(List<Tile> t)
    {
        if (t.Count != 4) return false;
        return AllTilesMatchForSet(t);
    }

    static bool IsNumberedSuit(TileSuit s) =>
        s == TileSuit.Bamboo || s == TileSuit.Dot || s == TileSuit.Character;

    static bool IsRun(List<Tile> t)
    {
        if (t.Count != 3) return false;
        // WindRuns: 3 winds of different ranks count as a run
        if (FlowerTileManager.instance != null && FlowerTileManager.instance.IsFlowerTileActive(FlowerTileType.WindRuns)
            && t.All(x => x.suit == TileSuit.Wind))
        {
            var windRanks = t.Select(x => x.rank).Distinct().ToList();
            if (windRanks.Count == 3) return true;
        }
        if (!t.All(x => IsNumberedSuit(x.suit))) return false;
        if (t[0].suit != t[1].suit || t[1].suit != t[2].suit) return false;
        var r = t.Select(x => x.rank).OrderBy(v => v).ToList();
        if (FlowerTileManager.instance.IsFlowerTileActive(FlowerTileType.SkipOneInRun))
        {
            return r[0] >= 1 && r[0] <= 7 && (r[0] + 1 == r[1] || r[0] + 2 == r[1]) && (r[1] + 1 == r[2] || r[1] + 2 == r[2]);
        }
        else
        {
            return r[0] >= 1 && r[0] <= 7 && r[0] + 1 == r[1] && r[1] + 1 == r[2];
        }
        
    }

    static bool IsNineRun(List<Tile> t)
    {
        if (t.Count != 9) return false;
        if (!t.All(x => IsNumberedSuit(x.suit))) return false;
        var suit = t[0].suit;
        if (!t.All(x => x.suit == suit)) return false;
        var r = t.Select(x => x.rank).OrderBy(v => v).ToList();
        if (FlowerTileManager.instance.IsFlowerTileActive(FlowerTileType.SkipOneInRun))
        {
            for (int i = 0; i < 9; i++)
                if (r[i] != i + 1 && r[i] != i + 2) return false;
        }
        else
        {
            for (int i = 0; i < 9; i++)
                if (r[i] != i + 1) return false;
        }
        return true;
    }

    static bool IsTwoPairs(List<Tile> t)
    {
        if (t.Count != 4) return false;
        var groups = t.GroupBy(x => (x.suit, x.rank)).ToList();
        return groups.Count == 2 && groups.All(g => g.Count() == 2);
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
