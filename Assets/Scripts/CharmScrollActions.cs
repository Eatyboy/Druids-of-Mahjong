using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Charm scroll actions that operate on a given hand (HandBase).
// Pass PlayerHand.instance, ScrollHand.instance, or any HandBase.
public static class CharmScrollActions
{
    // Permanently remove the given tile objects from the hand.
    // Removes from hand's currentHand and selectedTiles, then destroys the GameObjects.
    public static void RemoveTilesFromHand(HandBase hand, IEnumerable<PlayerTileObject> tileObjects)
    {
        if (hand == null) return;

        foreach (PlayerTileObject obj in (tileObjects ?? Array.Empty<PlayerTileObject>()).ToList())
        {
            hand.currentHand.Remove(obj);
            hand.selectedTiles.Remove(obj);
            if (obj != null && obj.gameObject != null)
                UnityEngine.Object.Destroy(obj.gameObject);
        }
    }

    // Add a copy of each given tile object to the hand.
    public static void AddCopiesToHand(HandBase hand, IEnumerable<PlayerTileObject> tileObjects)
    {
        if (hand == null) return;

        foreach (PlayerTileObject obj in tileObjects ?? Array.Empty<PlayerTileObject>())
        {
            if (obj?.tileData?.baseTileData == null) continue;
            Tile copy = CopyTile(obj);
            hand.AddTile(copy);
        }
    }

    // Create a new Tile instance with the same base data as the given tile object.
    public static Tile CopyTile(PlayerTileObject source)
    {
        if (source?.tileData?.baseTileData == null) return null;
        return new Tile(source.tileData.baseTileData);
    }

    public static void SwitchTileSuit(HandBase hand, IEnumerable<PlayerTileObject> tileObjects, TileSuit newSuit)
    {
        if (hand == null) return;

        foreach (PlayerTileObject obj in tileObjects ?? Array.Empty<PlayerTileObject>())
        {
            if (obj?.tileData == null) continue;
            obj.tileData.suit = newSuit;
        }
    }
}
