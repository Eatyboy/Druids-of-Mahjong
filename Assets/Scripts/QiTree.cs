using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class QiTree : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI qiText;

    public int qiCost = 100;
    public int flowerTileOptionCount = 3;

    private void Start()
    {
        UpdateQiText(GameManager.playerData.qi);
    }


    public void OnFlowerClick()
    {
        if (GameManager.playerData.qi >= qiCost)
        {
            GameManager.playerData.qi -= qiCost;
            UpdateQiText(GameManager.playerData.qi);

            List<FlowerTileType> flowerTileOptions = new(flowerTileOptionCount);
            for (int i = 0; i < flowerTileOptionCount; i++)
            {
                flowerTileOptions.Add(Utils.GetRandomEnumValue<FlowerTileType>());
            }
            GameManager.playerData.flowerTiles.Add(flowerTileOptions[0]);
        }
    }

    private void UpdateQiText(int qi)
    {
        qiText.text = "Qi: " + qi;
    }
}

