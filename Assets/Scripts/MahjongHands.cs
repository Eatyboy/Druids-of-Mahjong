using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
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
    // elements 0-4: pairs, sets, three runs, quads, nine runs
    public static List<List<Tile>> GetAllHandCombinations(List<Tile> selectedTiles)
    {
        List<Tile> sortedSelectedTiles = MahjongMergeSort(selectedTiles, 0, selectedTiles.Count - 1);

        List<List<Tile>> pairCombinations = new(); // 2 of a kind
        List<List<Tile>> setCombinations = new(); // 3 of a kind
        List<List<Tile>> threeRunCombinations = new(); // 3 in sequence
        List<List<Tile>> quadCombinations = new(); // 4 of a kind
        List<List<Tile>> nineRunCombinations = new(); // 9 in sequence\
        // each tile can only be part of one type of combination; e.g. if its in a run, it can't be in a set

        foreach (Tile st in sortedSelectedTiles)
        {
            List<Tile> sameKindList = new();
            List<Tile> sequenceList = new();
            
            // check for same kind (pair, set, quad) combinations by iterating through entire sorted list
            foreach (Tile at in sortedSelectedTiles)
            {
                if (at.suit != st.suit) continue;

                if (at.rank == st.rank)
                {
                    sameKindList.Add(at);
                    switch (sameKindList.Count)
                    {
                        case 2: pairCombinations.Add(sameKindList.GetRange(0, 2)); break;
                        case 3: setCombinations.Add(sameKindList.GetRange(0, 3)); break;
                        case 4: quadCombinations.Add(sameKindList.GetRange(0, 4)); break;
                    }
                }
            }

            // check for sequences (left and right check)
            // this implementation sucks tbh
            int i = sortedSelectedTiles.IndexOf(st);

            // check for 9-run
            // bring to left of suit in sorted list
            while (sortedSelectedTiles[i].suit == st.suit && i > 0)
            {
                i--;
            }
            if (i > 0) i++;
            // check rightward
            if (sortedSelectedTiles[i].suit == st.suit)
            {
                sequenceList.Add(sortedSelectedTiles[i]);
            }
            while (sortedSelectedTiles[i].suit == st.suit && i < sortedSelectedTiles.Count - 1)
            {
                // no duplicates
                if (sortedSelectedTiles[i + 1].rank != sortedSelectedTiles[i].rank)
                {
                    sequenceList.Add(sortedSelectedTiles[i + 1]);
                }
                i++;
            }

            // UnityEngine.Debug.Log("sequence list");
            // PrintTilesList(sequenceList);

            // should already be sorted
            // check for nine-run and sequence in sequence list (duplicates removed)
            if (sequenceList.Count == 9)
            {
                nineRunCombinations.Add(sequenceList);
            }
            if (sequenceList.Count >= 3)
            {
                int pI = sequenceList.IndexOf(st);
                if (pI - 2 >= 0)
                {
                    if (((float)sequenceList[pI - 2].rank + (float)sequenceList[pI - 1].rank + (float)sequenceList[pI].rank) / 3 
                        == (float)st.rank)
                    {
                        List<Tile> newCombo = new List<Tile> {sequenceList[pI - 2], sequenceList[pI - 1], sequenceList[pI]};
                        threeRunCombinations.Add(newCombo);
                    }
                }
                if (pI - 1 >= 0 && pI + 1 <= sequenceList.Count - 1)
                {
                    if (((float)sequenceList[pI - 1].rank + (float)sequenceList[pI].rank + (float)sequenceList[pI + 1].rank) / 3 
                        == (float)st.rank)
                    {
                        List<Tile> newCombo = new List<Tile> {sequenceList[pI - 1], sequenceList[pI], sequenceList[pI + 1]};
                        threeRunCombinations.Add(newCombo);
                    }
                }
                if (pI + 2 <= sequenceList.Count - 1)
                {
                    if (((float)sequenceList[pI].rank + (float)sequenceList[pI + 1].rank + (float)sequenceList[pI + 2].rank) / 3 == 
                        (float)st.rank)
                    {
                        List<Tile> newCombo = new List<Tile> {sequenceList[pI], sequenceList[pI + 1], sequenceList[pI + 2]};
                        threeRunCombinations.Add(newCombo);
                    }
                }
            }

        }

        List<List<Tile>> result = new();
        result.AddRange(pairCombinations);
        result.AddRange(setCombinations);
        result.AddRange(threeRunCombinations);
        result.AddRange(quadCombinations);
        result.AddRange(nineRunCombinations);

        return result;
    }

    public static (MahjongHandTypes type, List<Tile> tiles) GetOptimalHand(List<Tile> selectedTiles)
    {
        if (selectedTiles.Count == 0) return (MahjongHandTypes.None, null);

        List<Tile> optimalHand = new();
        List<Tile> sortedSelectedTiles = MahjongMergeSort(selectedTiles, 0, selectedTiles.Count - 1);
        
        // PrintTilesList(sortedSelectedTiles);

        List<List<Tile>> pairCombinations = new(); // 2 of a kind
        List<List<Tile>> setCombinations = new(); // 3 of a kind
        List<List<Tile>> threeRunCombinations = new(); // 3 in sequence
        List<List<Tile>> quadCombinations = new(); // 4 of a kind
        List<List<Tile>> nineRunCombinations = new(); // 9 in sequence\
        // each tile can only be part of one type of combination; e.g. if its in a run, it can't be in a set

        foreach (Tile st in sortedSelectedTiles)
        {
            List<Tile> sameKindList = new();
            List<Tile> sequenceList = new();
            
            // check for same kind (pair, set, quad) combinations by iterating through entire sorted list
            foreach (Tile at in sortedSelectedTiles)
            {
                if (at.suit != st.suit) continue;

                if (at.rank == st.rank)
                {
                    sameKindList.Add(at);
                    switch (sameKindList.Count)
                    {
                        case 2: pairCombinations.Add(sameKindList.GetRange(0, 2)); break;
                        case 3: setCombinations.Add(sameKindList.GetRange(0, 3)); break;
                        case 4: quadCombinations.Add(sameKindList.GetRange(0, 4)); break;
                    }
                }
            }

            // check for sequences (left and right check)
            // this implementation sucks tbh
            int i = sortedSelectedTiles.IndexOf(st);

            // check for 9-run
            // bring to left of suit in sorted list
            while (sortedSelectedTiles[i].suit == st.suit && i > 0)
            {
                i--;
            }
            if (i > 0) i++;
            // check rightward
            if (sortedSelectedTiles[i].suit == st.suit)
            {
                sequenceList.Add(sortedSelectedTiles[i]);
            }
            while (sortedSelectedTiles[i].suit == st.suit && i < sortedSelectedTiles.Count - 1)
            {
                // no duplicates
                if (sortedSelectedTiles[i + 1].rank != sortedSelectedTiles[i].rank)
                {
                    sequenceList.Add(sortedSelectedTiles[i + 1]);
                }
                i++;
            }

            // UnityEngine.Debug.Log("sequence list");
            // PrintTilesList(sequenceList);

            // should already be sorted
            // check for nine-run and sequence in sequence list (duplicates removed)
            if (sequenceList.Count == 9)
            {
                nineRunCombinations.Add(sequenceList);
            }
            if (sequenceList.Count >= 3)
            {
                int pI = sequenceList.IndexOf(st);
                if (pI - 2 >= 0)
                {
                    if (((float)sequenceList[pI - 2].rank + (float)sequenceList[pI - 1].rank + (float)sequenceList[pI].rank) / 3 
                        == (float)st.rank)
                    {
                        List<Tile> newCombo = new List<Tile> {sequenceList[pI - 2], sequenceList[pI - 1], sequenceList[pI]};
                        threeRunCombinations.Add(newCombo);
                    }
                }
                if (pI - 1 >= 0 && pI + 1 <= sequenceList.Count - 1)
                {
                    if (((float)sequenceList[pI - 1].rank + (float)sequenceList[pI].rank + (float)sequenceList[pI + 1].rank) / 3 
                        == (float)st.rank)
                    {
                        List<Tile> newCombo = new List<Tile> {sequenceList[pI - 1], sequenceList[pI], sequenceList[pI + 1]};
                        threeRunCombinations.Add(newCombo);
                    }
                }
                if (pI + 2 <= sequenceList.Count - 1)
                {
                    if (((float)sequenceList[pI].rank + (float)sequenceList[pI + 1].rank + (float)sequenceList[pI + 2].rank) / 3 == 
                        (float)st.rank)
                    {
                        List<Tile> newCombo = new List<Tile> {sequenceList[pI], sequenceList[pI + 1], sequenceList[pI + 2]};
                        threeRunCombinations.Add(newCombo);
                    }
                }
            }

        }

        // UnityEngine.Debug.Log("Printing pairs, sets, runs, quads, and nine runs");
        // Print2DTilesList(pairCombinations);
        // Print2DTilesList(setCombinations);
        // Print2DTilesList(threeRunCombinations);
        // Print2DTilesList(quadCombinations);
        // Print2DTilesList(nineRunCombinations);

        // return most powerful hand type (full win, all pairs, nine run, three sets, two quads, two sets, two runs, set and run, three pairs, quad, run set, pair, in that order)
        // for full win, go through each set, run, and pair combination; if theres a duplicate, skip that combination and try to get at least 1 pair + 4 sets/runs
        // limited by pair

        // DEBUG: check if this actually works using combinatorics and stats
        // Addendum: brute force it, check every single combination, O(n^2)
        foreach (List<Tile> p in pairCombinations)
        {
            optimalHand.Clear();
            optimalHand.AddRange(p);
            // oh god
            foreach (List<Tile> setPivot in setCombinations)
            {
                if (CheckForCommonTile(setPivot, optimalHand)) continue;

                optimalHand.AddRange(setPivot);

                foreach (List<Tile> sc1 in setCombinations)
                {
                    if (ContainsSameTiles(setPivot, sc1)) continue;

                    if (!CheckForCommonTile(optimalHand, setPivot))
                    {
                        optimalHand.AddRange(sc1);
                        if (optimalHand.Count == 14)
                        {
                            return (MahjongHandTypes.FullWin, optimalHand);
                        }
                    }
                }
                foreach (List<Tile> rc1 in threeRunCombinations)
                {
                    if (!CheckForCommonTile(optimalHand, rc1))
                    {
                        optimalHand.AddRange(rc1);
                        if (optimalHand.Count == 14)
                        {
                            return (MahjongHandTypes.FullWin, optimalHand);
                        }
                    }
                }
            }

            optimalHand.Clear();
            optimalHand.AddRange(p);

            foreach (List<Tile> runPivot in threeRunCombinations)
            {
                if (CheckForCommonTile(runPivot, optimalHand)) continue;

                optimalHand.AddRange(runPivot);

                foreach (List<Tile> rc2 in threeRunCombinations)
                {
                    if (ContainsSameTiles(runPivot, rc2)) continue;

                    if (!CheckForCommonTile(optimalHand, runPivot))
                    {
                        optimalHand.AddRange(rc2);
                        if (optimalHand.Count == 14)
                        {
                            return (MahjongHandTypes.FullWin, optimalHand);
                        }
                    }
                }
                foreach (List<Tile> sc2 in setCombinations)
                {
                    if (!CheckForCommonTile(optimalHand, sc2))
                    {
                        optimalHand.AddRange(sc2);
                        if (optimalHand.Count == 14)
                        {
                            return (MahjongHandTypes.FullWin, optimalHand);
                        }
                    }
                }
            }
        }

        // full pair (O(n^2))
        // DEBUG: might not check for all pairs, if so, new algorithm will likely by O(n!) or O(n^n)
        // right now, since max = 14, this is fine
        optimalHand = FindSetOfCombinationsFromAll(pairCombinations, 14);
        if (optimalHand != null)
        {
            return (MahjongHandTypes.AllPairs, optimalHand);
        }

        // nine run
        if (nineRunCombinations.Count > 0)
        {
            return (MahjongHandTypes.NineRun, nineRunCombinations[nineRunCombinations.Count - 1]);
        }

        // three sets
        optimalHand = FindSetOfCombinationsFromAll(setCombinations, 9);
        if (optimalHand != null)
        {
            return (MahjongHandTypes.ThreeSets, optimalHand);
        }

        // two quads
        optimalHand = FindSetOfCombinationsFromAll(quadCombinations, 8);
        if (optimalHand != null)
        {
            return (MahjongHandTypes.TwoQuads, optimalHand);
        }

        // two sets (works)
        optimalHand = FindSetOfCombinationsFromAll(setCombinations, 6);
        if (optimalHand != null)
        {
            return (MahjongHandTypes.TwoSets, optimalHand);
        }

        // two runs (works)
        optimalHand = FindSetOfCombinationsFromAll(threeRunCombinations, 6);
        if (optimalHand != null)
        {
            return (MahjongHandTypes.TwoRuns, optimalHand);
        }

        // set and run (should include all combos)
        // again, brute force, dinky
        optimalHand = new();
        if (setCombinations != null)
        {
            foreach (List<Tile> setPivot in setCombinations)
            {
                if (optimalHand != null)
                {
                    optimalHand.Clear(); 
                    optimalHand.AddRange(setPivot);  
                }

                foreach (List<Tile> r in threeRunCombinations)
                {
                    if (ContainsSameTiles(setPivot, r)) continue;

                    if (!CheckForCommonTile(optimalHand, r))
                    {
                        optimalHand.AddRange(r);
                        if (optimalHand.Count == 6)
                        {
                            return (MahjongHandTypes.SetAndRun, optimalHand);
                        }
                    }
                }

                foreach (List<Tile> s in setCombinations)
                {
                    if (ContainsSameTiles(setPivot, s)) continue;

                    if (!CheckForCommonTile(optimalHand, s))
                    {
                        optimalHand.AddRange(s);
                        if (optimalHand.Count == 6)
                        {
                            return (MahjongHandTypes.SetAndRun, optimalHand);
                        }
                    }
                }

                if (optimalHand != null)
                {
                    optimalHand.Clear(); 
                    optimalHand.AddRange(setPivot);  
                }

                foreach (List<Tile> s in setCombinations)
                {
                    if (ContainsSameTiles(setPivot, s)) continue;

                    if (!CheckForCommonTile(optimalHand, s))
                    {
                        optimalHand.AddRange(s);
                        if (optimalHand.Count == 6)
                        {
                            return (MahjongHandTypes.SetAndRun, optimalHand);
                        }
                    }
                }

                foreach (List<Tile> r in threeRunCombinations)
                {
                    if (ContainsSameTiles(setPivot, r)) continue;

                    if (!CheckForCommonTile(optimalHand, r))
                    {
                        optimalHand.AddRange(r);
                        if (optimalHand.Count == 6)
                        {
                            return (MahjongHandTypes.SetAndRun, optimalHand);
                        }
                    }
                }
            }   
        }


        // three pairs
        optimalHand = FindSetOfCombinationsFromAll(pairCombinations, 6);
        if (optimalHand != null)
        {
            return (MahjongHandTypes.ThreePairs, optimalHand);
        }

        // quad (everything below should work)
        if (quadCombinations.Count > 0)
        {
            return (MahjongHandTypes.Quad, quadCombinations[quadCombinations.Count - 1]);
        }

        // run
        if (threeRunCombinations.Count > 0)
        {
            return (MahjongHandTypes.Run, threeRunCombinations[threeRunCombinations.Count - 1]);
        }

        // set
        if (setCombinations.Count > 0)
        {
            return (MahjongHandTypes.Set, setCombinations[setCombinations.Count - 1]);
        }

        // pair
        if (pairCombinations.Count > 0)
        {
            return (MahjongHandTypes.Pair, pairCombinations[pairCombinations.Count - 1]);
        }

        return (MahjongHandTypes.None, null);
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

    // implement merge sort for player hand (suit order based on MahjongTile TileSuit enum)
    public static List<Tile> MahjongMergeSort(List<Tile> inputList, int leftIndex, int rightIndex)
    {
        if (leftIndex < rightIndex)
        {
            int middleIndex = leftIndex + (rightIndex - leftIndex) / 2;
            MahjongMergeSort(inputList, leftIndex, middleIndex);
            MahjongMergeSort(inputList, middleIndex + 1, rightIndex);
            Merge(inputList, leftIndex, middleIndex, rightIndex);
        }

        return inputList;
    }

    // holy copy paste
    private static void Merge(List<Tile> inputList, int leftIndex, int middleIndex, int rightIndex)
    {
        int leftListLength = middleIndex - leftIndex + 1;
        int rightListLength = rightIndex - middleIndex;
        List<Tile> leftTempList = new();
        List<Tile> rightTempList = new();
        int i, j;
        for (i = 0; i < leftListLength; ++i)
            leftTempList.Add(inputList[leftIndex + i]);
        for (j = 0; j < rightListLength; ++j)
            rightTempList.Add(inputList[middleIndex + 1 + j]);
        i = 0;
        j = 0;
        int k = leftIndex;
        while (i < leftListLength && j < rightListLength)
        {
            // check suit first
            if ((int)leftTempList[i].suit < (int)rightTempList[j].suit)
            {
                inputList[k++] = leftTempList[i++];
                continue;
            }
            else if ((int)leftTempList[i].suit > (int)rightTempList[j].suit)
            {
                inputList[k++] = rightTempList[j++];
                continue;
            }

            // then rank
            if (leftTempList[i].rank <= rightTempList[j].rank)
            {
                inputList[k++] = leftTempList[i++];
            }
            else
            {
                inputList[k++] = rightTempList[j++];
            }
        }
        while (i < leftListLength)
        {
            inputList[k++] = leftTempList[i++];
        }
        while (j < rightListLength)
        {
            inputList[k++] = rightTempList[j++];
        }
    } 

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
