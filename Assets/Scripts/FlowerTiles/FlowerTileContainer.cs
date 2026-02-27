using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using UnityEngine.UI;

public class FlowerTileContainer : MonoBehaviour
{
    public static FlowerTileContainer instance;

    [Header("References")]
    [SerializeField] private RectTransform flowerTileContainer;
    [SerializeField] private RectTransform flowerTileSlotsContainer;
    public FlowerTileInfoController infoController;
    [SerializeField] private FlowerTile flowerTilePrefab;
    [SerializeField] private GameObject flowerTileSlotPrefab;

    public Transform[] flowerTileSlots;
    public List<FlowerTile> flowerTileObjects = new();
    public FlowerTile selectedTile; // Currently Held Tile

    [Header("Flower Tile Positioning")]
    [SerializeField] private Vector2 containerOffset = Vector2.zero;
    [SerializeField] private float spacing = 0.0f;
    [SerializeField] private float swapDuration = 0.25f;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        flowerTileSlots = new Transform[flowerTileContainer.childCount];
        for (int i = 0; i < flowerTileContainer.childCount; ++i)
        {
            flowerTileSlots[i] = flowerTileContainer.GetChild(i);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(DelayedRefreshPlayerFlowerTiles(0.1f));
    }

    public void AddFlowerTile(FlowerTileInstance flowerTileInstance)
    {
        GameObject tileSlot = Instantiate(flowerTileSlotPrefab, flowerTileContainer);
        FlowerTile addedFlowerTile = Instantiate(flowerTilePrefab, tileSlot.transform);
        int addedTileIndex = flowerTileObjects.Count;
        addedFlowerTile.Initialize(flowerTileInstance, flowerTileContainer, infoController);
        flowerTileObjects.Add(addedFlowerTile);
        addedFlowerTile.rectTransform.anchoredPosition = GetSlotPosition(addedTileIndex);
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
            FlowerTileInstance fti = FlowerTileManager.instance.GetRandomFlowerTile();
            GameManager.playerData.flowerTiles.Add(fti);
            AddFlowerTile(fti);
        }
        if (selectedTile == null) { return; }
        int selectedIndex = flowerTileObjects.IndexOf(selectedTile);
        for (int i = 0; i < flowerTileObjects.Count; i++)
        {
            if (selectedTile.rectTransform.anchoredPosition.x > flowerTileObjects[i].rectTransform.anchoredPosition.x && selectedIndex < i)
            {
                StartCoroutine(Swap(selectedIndex, i));
                break;
            }

            if (selectedTile.rectTransform.anchoredPosition.x < flowerTileObjects[i].rectTransform.anchoredPosition.x && selectedIndex > i)
            {
                StartCoroutine(Swap(selectedIndex, i));
                break;
            }
        }
    }

    public Vector2 GetSlotPosition(int index)
    {
        if (index < 0 || index >= flowerTileObjects.Count)
        {
            Debug.LogError("Tried to get the slot position at an out of bounds index");
            return Vector2.zero;
        }

        float width = flowerTilePrefab.rectTransform.rect.width;
        float x = flowerTileContainer.rect.xMax - (width + spacing) * index - 0.5f * width;
        return new(x + containerOffset.x, containerOffset.y);
    }

    private IEnumerator Swap(int selectedIndex, int index)
    {
        (flowerTileObjects[selectedIndex], flowerTileObjects[index]) = (flowerTileObjects[index], flowerTileObjects[selectedIndex]);

        FlowerTile swapped = flowerTileObjects[index];
        swapped.GetComponent<GraphicRaycaster>().enabled = false;
        Vector2 startPos = swapped.rectTransform.anchoredPosition;
        Vector2 endPos = GetSlotPosition(index);
        float elapsedTime = 0.0f;
        while (elapsedTime < swapDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0.0f, 1.0f, elapsedTime / swapDuration);

            swapped.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        swapped.rectTransform.anchoredPosition = endPos;
        swapped.GetComponent<GraphicRaycaster>().enabled = true;

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
