using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Basic charm scroll functionality
public static class CharmScrollActions
{
    // Permanently remove the given tile objects from the player hand
    public static void RemoveTilesFromHand(IEnumerable<PlayerTileObject> tileObjects)
    {
        if (PlayerHand.instance == null) return;

        foreach (PlayerTileObject obj in tileObjects.ToList())
        {
            PlayerHand.instance.currentHand.Remove(obj);
            PlayerHand.instance.selectedTiles.Remove(obj);
            if (obj != null && obj.gameObject != null)
                UnityEngine.Object.Destroy(obj.gameObject);
        }
    }

    // Add a copy of each given tile object to the player hand
    public static void AddCopiesToHand(IEnumerable<PlayerTileObject> tileObjects)
    {
        if (PlayerHand.instance == null) return;

        foreach (PlayerTileObject obj in tileObjects ?? Array.Empty<PlayerTileObject>())
        {
            if (obj?.tileData?.baseTileData == null) continue;
            Tile copy = CopyTile(obj);
            PlayerHand.instance.AddTile(copy);
        }
    }

    // Create a new Tile instance with the same base data as the given tile object
    public static Tile CopyTile(PlayerTileObject source)
    {
        if (source?.tileData?.baseTileData == null) return null;
        return new Tile(source.tileData.baseTileData);
    }
}
