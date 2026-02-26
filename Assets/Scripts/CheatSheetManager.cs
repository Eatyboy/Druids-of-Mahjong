using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class CheatSheetManager : MonoBehaviour
{
    public static CheatSheetManager instance;

    [SerializeField] private GameObject cheatSheet;

    [SerializeField] private TextMeshProUGUI handTypeText;
    [SerializeField] private TextMeshProUGUI damageNumText;

    [SerializeField] private GameObject handTypesObj;
    private List<GameObject> handTypesObjList;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    private void Start()
    {
        handTypesObjList = new();
        foreach (Transform child in handTypesObj.transform)
        {
            handTypesObjList.Add(child.gameObject);   
        }

        HideAllHandTypes();
    }

    private void HideAllHandTypes()
    {
        foreach (GameObject obj in handTypesObjList)
        {
            obj.SetActive(false);
        }
    }

    public void SetHandTypeData(int index)
    {
        HideAllHandTypes();
        if (index >= handTypesObjList.Count)
        {
            UnityEngine.Debug.LogWarning("Cheat sheet button index out of range. Check button index and make sure hand types objects was initialized properly");
        }
        handTypesObjList[index].SetActive(true);

        MahjongHandTypes type = (MahjongHandTypes)(index + 1);
        handTypeText.text = ModifiedHandNames[type];
        damageNumText.text = MahjongHands.HandScores[type].ToString();
    }

    public void ShowCheatSheet()
    {
        cheatSheet.SetActive(true);
    }

    public void HideCheatSheet()
    {
        cheatSheet.SetActive(false);
    }

    public static readonly IReadOnlyDictionary<MahjongHandTypes, string> ModifiedHandNames = new Dictionary<MahjongHandTypes, string>
    {
        { MahjongHandTypes.None, "None"},
        { MahjongHandTypes.Pair, "Pair"},
        { MahjongHandTypes.Set, "Set"},
        { MahjongHandTypes.Run, "Run"},
        { MahjongHandTypes.Quad, "Quad"},
        { MahjongHandTypes.TwoPairs, "Two Pairs"},
        { MahjongHandTypes.ThreePairs, "Three Pairs"},
        { MahjongHandTypes.SetAndRun, "Set And Run"},
        { MahjongHandTypes.TwoRuns, "Two Runs"},
        { MahjongHandTypes.TwoSets, "Two Sets"},
        { MahjongHandTypes.TwoQuads, "Two Quads"},
        { MahjongHandTypes.ThreeSets, "Three Sets"},
        { MahjongHandTypes.NineRun, "Nine Run"},
        { MahjongHandTypes.AllPairs, "All Pairs"},
        { MahjongHandTypes.FullWin, "Full Win"}
    };
}
