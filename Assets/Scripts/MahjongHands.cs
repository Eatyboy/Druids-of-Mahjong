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
    public static (MahjongHandTypes type, List<TileObject> tiles) GetMahjongHand(List<TileObject> allTiles, List<TileObject> selectedTiles)
    {
        if (selectedTiles.Count == 0 || allTiles.Count == 0) return (MahjongHandTypes.None, null);

        List<TileObject> optimalHand = new();
        MahjongHandTypes optimalHandType = MahjongHandTypes.None;
        List<TileObject> sortedHand = MahjongMergeSort(allTiles, 0, allTiles.Count - 1);
        List<TileObject> sortedSelectedTiles = MahjongMergeSort(selectedTiles, 0, selectedTiles.Count - 1);
        
        UnityEngine.Debug.Log("sorted list");
        foreach(TileObject t in sortedHand)
        {
            UnityEngine.Debug.Log(t.tileData.rank + " of " + t.tileData.suit);
        }

        List<List<TileObject>> pairCombinations = new(); // 2 of a kind
        List<List<TileObject>> setCombinations = new(); // 3 of a kind
        List<List<TileObject>> threeRunCombinations = new(); // 3 in sequence
        List<List<TileObject>> quadCombinations = new(); // 4 of a kind
        List<List<TileObject>> nineRunCombinations = new(); // 9 in sequence\
        // each tile can only be part of one type of combination; e.g. if its in a run, it can't be in a set

        foreach (TileObject st in sortedSelectedTiles)
        {
            List<TileObject> sameKindList = new();
            List<TileObject> sequenceList = new();
            sameKindList.Add(st);

            // check for same kind (pair, set, quad) combinations by iterating through entire sorted list
            foreach (TileObject at in sortedHand)
            {
                // does not check own tile
                if (st.Equals(at) || at.tileData.suit != st.tileData.suit) continue;

                if (at.tileData.rank == st.tileData.rank)
                {
                    sameKindList.Add(at);
                    switch (sameKindList.Count)
                    {
                        case 2:
                            pairCombinations.Add(sameKindList);
                            break;
                        case 3:
                            setCombinations.Add(sameKindList);
                            break;
                        case 4:
                            quadCombinations.Add(sameKindList);
                            break;
                    }
                }
            }

            // check for sequences (left and right check)
            // this implementation sucks tbh
            int i = sortedHand.IndexOf(st);

            // check for 9-run
            // bring to left of suit in sorted list
            while (sortedHand[i].tileData.suit == st.tileData.suit && i > 0)
            {
                i--;
            }
            // check rightward
            while (sortedHand[i].tileData.suit == st.tileData.suit && i < sortedHand.Count - 1)
            {
                // no duplicates
                if (sortedHand[i + 1].tileData.rank != sortedHand[i].tileData.rank)
                {
                    sequenceList.Add(sortedHand[i + 1]);
                }
                i++;
            }

            // should already be sorted
            // check for nine-run and sequence in sequence list (duplicates removed)
            if (sequenceList.Count == 9)
            {
                nineRunCombinations.Add(sequenceList);
            }
            if (sequenceList.Count > 3)
            {
                int pivotIndex = sequenceList.IndexOf(st);
                if (pivotIndex + 2 <= sequenceList.Count - 1)
                {
                    List<TileObject> newCombo = new List<TileObject> {sequenceList[i], sequenceList[i + 1], sequenceList[i + 2]};
                    threeRunCombinations.Add(newCombo);
                }
                if (pivotIndex - 1 > 0 && pivotIndex + 1 <= sequenceList.Count - 1)
                {
                    List<TileObject> newCombo = new List<TileObject> {sequenceList[i - 1], sequenceList[i], sequenceList[i + 1]};
                    threeRunCombinations.Add(newCombo);
                }
                if (pivotIndex - 2 >= 0)
                {
                    List<TileObject> newCombo = new List<TileObject> {sequenceList[i - 2], sequenceList[i - 1], sequenceList[i]};
                    threeRunCombinations.Add(newCombo);
                }
            }

        }

        // return most powerful hand type (full win, all pairs, nine run, three sets, two quads, two sets, two runs, set and run, three pairs, quad, run set, pair, in that order)
        // for full win, go through each set, run, and pair combination; if theres a duplicate, skip that combination and try to get at least 1 pair + 4 sets/runs
        // limited by pair

        // DEBUG: check if this actually works using combinatorics and stats
        // Addendum: brute force it, check every single combination, O(n^2)
        List<TileObject> possiblefullWin = new();
        foreach (List<TileObject> t in pairCombinations)
        {
            possiblefullWin.Clear();
            possiblefullWin.AddRange(t);
            // oh god
            foreach (List<TileObject> setPivot in setCombinations)
            {
                if (CheckForCommonTile(setPivot, possiblefullWin)) continue;

                possiblefullWin.AddRange(setPivot);

                foreach (List<TileObject> sc1 in setCombinations)
                {
                    if (ContainsSameTiles(setPivot, sc1)) continue;

                    if (!CheckForCommonTile(possiblefullWin, setPivot))
                    {
                        possiblefullWin.AddRange(sc1);
                        if (possiblefullWin.Count == 14)
                        {
                            return (MahjongHandTypes.FullWin, possiblefullWin);
                        }
                    }
                }
                foreach (List<TileObject> rc1 in threeRunCombinationsCombinations)
                {
                    if (!CheckForCommonTile(possiblefullWin, rc1))
                    {
                        possiblefullWin.AddRange(rc1);
                        if (possiblefullWin.Count == 14)
                        {
                            return (MahjongHandTypes.FullWin, possiblefullWin);
                        }
                    }
                }
            }

            foreach (List<TileObject> runPivot in threeRunCombinations)
            {
                if (CheckForCommonTile(runPivot, possiblefullWin)) continue;

                possiblefullWin.AddRange(runPivot);

                foreach (List<TileObject> rc2 in threeRunCombinations)
                {
                    if (ContainsSameTiles(runPivot, rc2)) continue;

                    if (!CheckForCommonTile(possiblefullWin, runPivot))
                    {
                        possiblefullWin.AddRange(rc2);
                        if (possiblefullWin.Count == 14)
                        {
                            return (MahjongHandTypes.FullWin, possiblefullWin);
                        }
                    }
                }
                foreach (List<TileObject> sc2 in setCombinations)
                {
                    if (!CheckForCommonTile(possiblefullWin, sc2))
                    {
                        possiblefullWin.AddRange(sc2);
                        if (possiblefullWin.Count == 14)
                        {
                            return (MahjongHandTypes.FullWin, possiblefullWin);
                        }
                    }
                }
            }
        }
        

        // list1.AddRange(list2); to concat lists
        // TODO: Implement hand checking
        return (optimalHandType, optimalHand);
    }

    // implement merge sort for player hand (suit order based on MahjongTile TileSuit enum)
    public static List<TileObject> MahjongMergeSort(List<TileObject> inputList, int leftIndex, int rightIndex)
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
    private static void Merge(List<TileObject> inputList, int leftIndex, int middleIndex, int rightIndex)
    {
        int leftListLength = middleIndex - leftIndex + 1;
        int rightListLength = rightIndex - middleIndex;
        List<TileObject> leftTempList = new();
        List<TileObject> rightTempList = new();
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
            if ((int)leftTempList[i].tileData.suit < (int)rightTempList[j].tileData.suit)
            {
                inputList[k++] = leftTempList[i++];
                continue;
            }
            else if ((int)leftTempList[i].tileData.suit > (int)rightTempList[j].tileData.suit)
            {
                inputList[k++] = rightTempList[j++];
                continue;
            }

            // then rank
            if (leftTempList[i].tileData.rank <= rightTempList[j].tileData.rank)
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

    private static bool CheckForCommonTile(List<TileObject> list1, List<TileObject> list2)
    {
        // Use Any() to check if any element of list1 is present in list2
        return list1.Any(item1 => list2.Contains(item1));
    }

    private static bool ContainsSameTiles(List<TileObject> list1, List<TileObject> list2)
    {
        if (list1.Count != list2.Count) return false;
        for (int i = 0; i < list1.Count; i++)
        {
            if (!list1[i].Equals(list2[i])) return false;
        }
        return true;
    }
}
