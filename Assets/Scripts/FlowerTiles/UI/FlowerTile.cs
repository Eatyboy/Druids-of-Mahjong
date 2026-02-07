using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlowerTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public FlowerTileData data;
    public RectTransform rectTransform;
    public FlowerTileEffect effectClass;

    [SerializeField] private Image image;

    public FlowerTileInfoController infoController;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(FlowerTileData data, FlowerTileInfoController infoController)
    {
        this.infoController = infoController;

        this.data = data;
        image.sprite = data.sprite;
        CombatManager.instance.actionQueue.Enqueue(() => effectClass.OnInitialize(PlayerHand.instance.GetPlayerHandTileData(), PlayerHand.instance.GetSelectedTileData()));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoController.currentFlowerTile = this;
        infoController.PointerEntered();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoController.PointerExited();
    }
}
