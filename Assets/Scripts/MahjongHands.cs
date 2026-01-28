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
        
        // UnityEngine.Debug.Log("sorted list");
        // foreach(TileObject t in sortedHand)
        // {
        //     UnityEngine.Debug.Log(t.tileData.rank + " of " + t.tileData.suit);
        // }

        List<List<TileObject>> pairCombinations = new(); // 2 of a kind
        List<List<TileObject>> setCombinations = new(); // 3 of a kind
        List<List<TileObject>> threeRunCombinations = new(); // 3 in sequence
        List<List<TileObject>> quadCombinations = new(); // 4 of a kind
        List<List<TileObject>> nineRunCombinations = new(); // 9 in sequence\
        // each tile can only be part of one type of combination; e.g. if its in a run, it can't be in a set

        foreach (TileObject st in selectedTiles)
        {
            List<TileObject> sameKindList = new();
            List<TileObject> sequenceList = new();
            sameKindList.Add(st);
            sequenceList.Add(st);

            // check for same kind (pair, set, quad) combinations by iterating through entire sorted list
            foreach (TileObject at in allTiles)
            {
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
            int uniqueRanks = 0;
            int i = sortedHand.IndexOf(st);

            // case-by-case checking for sequence (this sucks vro)
            if (i <= sortedHand.Count - 3)
            {
                if (sortedHand[i + 2].tileData.suit == sortedHand[i].tileData.suit)
                {
                    List<TileObject> newCombo = new List<TileObject> {sortedHand[i], sortedHand[i + 1], sortedHand[i + 2]};
                    threeRunCombinations.Add(newCombo);
                }   
            }
            if (i >= 2)
            {
                if (sortedHand[i - 2].tileData.suit == sortedHand[i].tileData.suit)
                {
                    List<TileObject> newCombo = new List<TileObject> {sortedHand[i - 2], sortedHand[i - 1], sortedHand[i]};
                    threeRunCombinations.Add(newCombo);
                }
            }
            if (i > 0 && i < sortedHand.Count - 1)
            {
                if (sortedHand[i + 1].tileData.suit == sortedHand[i - 1].tileData.suit)
                {
                    List<TileObject> newCombo = new List<TileObject> {sortedHand[i - 1], sortedHand[i], sortedHand[i + 1]};
                    threeRunCombinations.Add(newCombo);
                }
            }

            // check for 9-run
            // bring to left of suit in sorted list
            while (sortedHand[i].tileData.suit == st.tileData.suit && i > 0)
            {
                i--;
            }
            // check rightward
            while (sortedHand[i].tileData.suit == st.tileData.suit && i < sortedHand.Count - 1)
            {
                if (sortedHand[i + 1].tileData.rank != sortedHand[i].tileData.rank)
                {
                    uniqueRanks++;
                    sequenceList.Add(sortedHand[i + 1]);
                }
                i++;
            }

            if (uniqueRanks == 9)
            {
                nineRunCombinations.Add(sequenceList);
            }

        }

        // return most powerful hand type (full win, all pairs, nine run, three sets, two quads, two sets, two runs, set and run, three pairs, quad, run set, pair, in that order)
        // for full win, go through each set, run, and pair combination; if theres a duplicate, skip that combination and try to get at least 1 pair + 4 sets/runs
        // limited by pair

        // DEBUG: check if this actually works using combinatorics and stats
        foreach (List<TileObject> t in pairCombinations)
        {
            List<TileObject> possiblefullWin = new();
            possiblefullWin.AddRange(t);
            foreach (List<TileObject> setCombo in setCombinations)
            {
                if (!CheckForCommonTile(possiblefullWin, setCombo))
                {
                    possiblefullWin.AddRange(setCombo);
                    if (possiblefullWin.Count == 14)
                    {
                        return (MahjongHandTypes.FullWin, possiblefullWin);
                    }
                }
            }
            foreach (List<TileObject> runCombo in threeRunCombinations)
            {
                if (!CheckForCommonTile(possiblefullWin, runCombo))
                {
                    possiblefullWin.AddRange(runCombo);
                    if (possiblefullWin.Count == 14)
                    {
                        return (MahjongHandTypes.FullWin, possiblefullWin);
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
}
