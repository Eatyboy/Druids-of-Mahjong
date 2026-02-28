using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using UnityEngine.UI;

public class FlowerTileUI : MonoBehaviour
{
    public static FlowerTileUI instance;

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

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    private void Start()
    {
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
        FlowerTile addedFlowerTile = Instantiate(flowerTilePrefab, flowerTileContainer.transform);
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
            if ((selectedTile.rectTransform.anchoredPosition.x < flowerTileObjects[i].rectTransform.anchoredPosition.x && selectedIndex < i) 
                || (selectedTile.rectTransform.anchoredPosition.x > flowerTileObjects[i].rectTransform.anchoredPosition.x && selectedIndex > i))
            {
                Swap(selectedIndex, i);
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

    private void Swap(int selectedIndex, int index)
    {
        (flowerTileObjects[selectedIndex], flowerTileObjects[index]) = (flowerTileObjects[index], flowerTileObjects[selectedIndex]);

        int swappedIndex = selectedIndex;
        FlowerTile swapped = flowerTileObjects[swappedIndex];
        swapped.canvasGroup.blocksRaycasts = false;
        swapped.StartReturnAnim(doPunch: false);
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
