using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Abstract base for a hand of tiles: container, selection, add/draw, sort.
// PlayerHand (battle) and ScrollHand (charm scrolls) inherit from this.
public abstract class HandBase : MonoBehaviour
{
    [Header("Hand References")]
    [SerializeField] protected Transform tileContainer;
    [SerializeField] protected PlayerTileObject tileObjectPrefab;
    [SerializeField] protected RectTransform tileDrawOrigin;

    [Header("Hand/Tiles")]
    [SerializeField] protected int defaultHandSize = 14;
    [SerializeField] protected float drawDuration = 0.03f;
    [SerializeField] protected float sortDelay = 0.4f;

    public List<PlayerTileObject> currentHand;
    public List<PlayerTileObject> selectedTiles;
    public int currentHandSize = 14;

    protected virtual void Awake()
    {
        currentHandSize = defaultHandSize;
        if (currentHand == null) currentHand = new List<PlayerTileObject>();
        if (selectedTiles == null) selectedTiles = new List<PlayerTileObject>();
        currentHand.Clear();
        selectedTiles.Clear();

        if (tileContainer != null)
        {
            foreach (Transform tileObj in tileContainer)
                Destroy(tileObj.gameObject);
        }
    }

    public virtual void AddTile(Tile tile)
    {
        if (tile == null || tileObjectPrefab == null || tileContainer == null) return;

        PlayerTileObject newTileObj = Instantiate(tileObjectPrefab, tileContainer);
        newTileObj.rt.position = tileDrawOrigin != null ? tileDrawOrigin.position : tileContainer.position;
        newTileObj.Initialize(tile);
        newTileObj.name = $"{newTileObj.tileData.rank} of {newTileObj.tileData.suit}";
        newTileObj.transform.SetAsFirstSibling();
        currentHand.Add(newTileObj);
    }

    public virtual IEnumerator DrawTile()
    {
        yield return new WaitForSeconds(drawDuration);
        if (TilesManager.instance != null && GameManager.playerData?.deck != null && GameManager.playerData.deck.Count > 0)
            AddTile(TilesManager.instance.DrawFromDeck());
    }

    public virtual IEnumerator DrawUntilFullHand()
    {
        while (currentHand.Count < currentHandSize)
        {
            if (GameManager.playerData?.deck == null || GameManager.playerData.deck.Count == 0) yield break;
            if (currentHand.Count >= currentHandSize) yield break;
            yield return DrawTile();
        }
        yield return SortTilesInHand();
    }

    public virtual void SelectTile(PlayerTileObject tile)
    {
        if (tile == null) return;
        selectedTiles.Add(tile);
    }

    public virtual void DeselectTile(PlayerTileObject tile)
    {
        if (tile == null) return;
        selectedTiles.Remove(tile);
    }

    public List<Tile> GetSelectedTileData()
    {
        var list = new List<Tile>();
        if (selectedTiles == null) return list;
        foreach (PlayerTileObject t in selectedTiles)
            list.Add(t.tileData);
        return list;
    }

    public List<Tile> GetHandTileData()
    {
        var list = new List<Tile>();
        if (currentHand == null) return list;
        foreach (PlayerTileObject t in currentHand)
            list.Add(t.tileData);
        return list;
    }

    public void ClearTiles()
    {
        if (currentHand == null) return;
        foreach (PlayerTileObject t in currentHand)
        {
            if (t != null && t.gameObject != null)
                Destroy(t.gameObject);
        }
        currentHand.Clear();
        if (selectedTiles != null) selectedTiles.Clear();
    }

    public virtual IEnumerator SortTilesInHand()
    {
        if (selectedTiles != null)
        {
            foreach (PlayerTileObject tile in selectedTiles)
            {
                if (tile != null)
                {
                    tile.isSelected = false;
                    tile.ResetToInitialPosition();
                }
            }
            selectedTiles.Clear();
        }

        if (currentHand == null || currentHand.Count == 0) yield break;

        PlayerTileObject[] sortedTiles = currentHand
            .OrderBy(t => t.tileData.suit)
            .ThenBy(t => t.tileData.rank)
            .ToArray();

        yield return new WaitForSeconds(sortDelay);

        for (int i = 0; i < sortedTiles.Length; i++)
            sortedTiles[i].transform.SetSiblingIndex(i);
    }
}
