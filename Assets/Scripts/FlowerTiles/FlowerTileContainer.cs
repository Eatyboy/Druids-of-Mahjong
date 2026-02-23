using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class FlowerTileContainer : MonoBehaviour
{
    public FlowerTileContainer instance;

    [Header("References")]
    [SerializeField] private RectTransform flowerTileContainer;
    public FlowerTileInfoController infoController;
    [SerializeField] private FlowerTile flowerTilePrefab;

    public List<FlowerTile> flowerTileObjects = new();

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
        FlowerTile addedFlowerTile = Instantiate(flowerTilePrefab, flowerTileContainer);
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
