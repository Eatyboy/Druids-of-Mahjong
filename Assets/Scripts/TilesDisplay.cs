using TMPro;
using UnityEngine;


public class TilesDisplay : MonoBehaviour
{
    public MajongTiles majongData;

    public TMP_Text suitText;

    public TMP_Text valueText;

    void Start()
    {
        UpdateTilesDisplay();
    }
    public void UpdateTilesDisplay()
    {
        valueText.text = majongData.value.ToString();

        suitText.text = majongData.suit.ToString();
    }

    public void setTile(MajongTiles tile)
    {
        majongData = tile;
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