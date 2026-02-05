using TMPro;
using UnityEngine;

public class FlowerTileInfoController : MonoBehaviour
{
    [SerializeField] private FlowerTileInfo flowerTileInfo;

    [SerializeField] private Vector2 offset;
    private int hoverCount = 0;

    public FlowerTile currentFlowerTile;

    private void Awake()
    {
        flowerTileInfo.Close();
    }

    public void PointerEntered()
    {
        hoverCount++;
        flowerTileInfo.Open(currentFlowerTile.data, currentFlowerTile.rectTransform.anchoredPosition + offset);
    }

    public void PointerExited()
    {
        hoverCount--;
        CancelInvoke(nameof(CheckClose));
        Invoke(nameof(CheckClose), 0.01f);
    }
    private void CheckClose()
    {
        if (hoverCount <= 0)
        {
            hoverCount = 0;
            flowerTileInfo.Close();
        }
    }
}
