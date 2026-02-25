using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlowerTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public FlowerTileInstance instance;
    public RectTransform rectTransform;

    [SerializeField] private Image image;

    public FlowerTileInfoController infoController;

    // need this to differentiate between flower tiles already initialized and those not (f key vs bought) to prevent stacking
    public bool initialized;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialized = false;
    }

    public void Initialize(FlowerTileInstance flowerTileInstance, FlowerTileInfoController infoController)
    {
        this.instance = flowerTileInstance;
        this.infoController = infoController;
        UpdateImage();

        // can be null if not in combat scene; will be checked for initialization again in GameManager pre-battle state
        if (initialized || CombatManager.instance == null) return;

        CombatManager.instance.EnqueueAction(() =>
            flowerTileInstance.effect.OnInitialize(
                PlayerHand.instance.GetPlayerHandTileData(),
                PlayerHand.instance.GetSelectedTileData()
            ),
            nameof(flowerTileInstance.effect.OnInitialize)
        );

        initialized = true;
    }

    public void UpdateImage()
    {
        image.sprite = instance.data.sprite;
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
