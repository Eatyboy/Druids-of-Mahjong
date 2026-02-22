using TMPro;
using UnityEngine;

public class FlowerTileInfoController : MonoBehaviour
{
    [SerializeField] protected FlowerTileInfo flowerTileInfo;

    [SerializeField] protected Vector2 offset;
    protected int hoverCount = 0;

    public FlowerTile currentFlowerTile;

    protected void Awake()
    {
        flowerTileInfo.Close();
    }

    public virtual void PointerEntered()
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
    protected void CheckClose()
    {
        if (hoverCount <= 0)
        {
            hoverCount = 0;
            flowerTileInfo.Close();
        }
    }
}
