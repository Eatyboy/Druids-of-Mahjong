using System.ComponentModel;
using UnityEngine;

public class ShopTileInfoController : FlowerTileInfoController
{
    [SerializeField] private Canvas canvas;
    public override void PointerEntered()
    {
        hoverCount++;
        Vector2 pos = RectTransformUtility.CalculateRelativeRectTransformBounds(canvas.transform, currentFlowerTile.transform).center;
        Vector2 screenOffset = new(0.5f * Screen.width, -0.5f * Screen.height);
        flowerTileInfo.Open(currentFlowerTile.instance.data, pos + screenOffset + offset);
    }
}
