using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class FlowerTileContainer : MonoBehaviour
{
    public static FlowerTileContainer instance;

    [Header("References")]
    [SerializeField] private RectTransform flowerTileContainer;
    public FlowerTileInfoController infoController;
    [SerializeField] private FlowerTile flowerTilePrefab;
    [SerializeField] private GameObject flowerTileSlotPrefab;

    public List<FlowerTile> flowerTileObjects = new();
    public FlowerTile selectedTile; // Currently Held Tile

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    private void OnEnable()
    {
        StartCoroutine(DelayedRefreshPlayerFlowerTiles(0.1f));
    }

    public void AddFlowerTile(FlowerTileInstance flowerTileInstance)
    {
        GameObject tileSlot = Instantiate(flowerTileSlotPrefab, flowerTileContainer);
        FlowerTile addedFlowerTile = Instantiate(flowerTilePrefab, tileSlot.transform);
        addedFlowerTile.Initialize(flowerTileInstance, infoController);
        flowerTileObjects.Add(addedFlowerTile);
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
            FlowerTileInstance fti = FlowerTileManager.instance.GetRandomFlowerTile();
            GameManager.playerData.flowerTiles.Add(fti);
            AddFlowerTile(fti);
        }
        if(selectedTile == null) { return; }
        for (int i = 0; i < flowerTileObjects.Count; i++)
        {

            if (selectedTile.transform.position.x > flowerTileObjects[i].transform.position.x)
            {
                if (selectedTile.GetComponentIndex() < flowerTileObjects[i].GetComponentIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if (selectedTile.transform.position.x < flowerTileObjects[i].transform.position.x)
            {
                if (selectedTile.GetComponentIndex() > flowerTileObjects[i].GetComponentIndex())
                {
                    Swap(i);
                    break;
                }
            }
        }
    }

    private void Swap(int index)
    {
        Debug.Log("Swap");
        /*isCrossing = true;

        Transform focusedParent = selectedCard.transform.parent;
        Transform crossedParent = cards[index].transform.parent;

        cards[index].transform.SetParent(focusedParent);
        cards[index].transform.localPosition = cards[index].selected ? new Vector3(0, cards[index].selectionOffset, 0) : Vector3.zero;
        selectedCard.transform.SetParent(crossedParent);

        isCrossing = false;

        if (cards[index].cardVisual == null)
            return;

        bool swapIsRight = cards[index].ParentIndex() > selectedCard.ParentIndex();
        cards[index].cardVisual.Swap(swapIsRight ? -1 : 1);

        //Updated Visual Indexes
        foreach (Card card in cards)
        {
            card.cardVisual.UpdateIndex(transform.childCount);
        }*/
    }

    IEnumerator DelayedRefreshPlayerFlowerTiles(float sec)
    {
        yield return new WaitForSeconds(sec);

        foreach (FlowerTile ft in flowerTileObjects)
        {
            Destroy(ft.gameObject);
        }
        flowerTileObjects.Clear();

        foreach (FlowerTileInstance fti in GameManager.playerData.flowerTiles)
        {
            AddFlowerTile(fti);
        }
    }
}
