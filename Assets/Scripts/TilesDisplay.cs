using TMPro;
using UnityEngine;

//This class might not be needed in the future with images being added
public class TilesDisplay : MonoBehaviour
{
    public MahjongTile mahjongData;

    public TMP_Text suitText;

    public TMP_Text valueText;

    void Start()
    {
        UpdateTilesDisplay();
    }
    public void UpdateTilesDisplay()
    {
        valueText.text = mahjongData.rank.ToString();

        suitText.text = mahjongData.suit.ToString();
    }

    public void setTile(MahjongTile tile)
    {
        mahjongData = tile;
    }

    public void setSuitText(string text)
    {
        suitText.text = text;
    }
    public void SetValueText(string text)
    {
        valueText.text = text;
    }
}