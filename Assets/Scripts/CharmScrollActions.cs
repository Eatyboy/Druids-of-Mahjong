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

    // Switch the suit of the given tile objects to the new suit.
    // Only affects tiles that are currently a numbered suit (Bamboo, Dot, Character); honors (Wind, Dragon) are unchanged.
    public static void SwitchTileSuit(HandBase hand, IEnumerable<PlayerTileObject> tileObjects, TileSuit newSuit)
    {
        if (hand == null) return;

        foreach (PlayerTileObject obj in tileObjects ?? Array.Empty<PlayerTileObject>())
        {
            if (obj?.tileData == null) continue;
            if (!IsNumberedSuit(obj.tileData.suit)) continue;

            obj.tileData.suit = newSuit;
            obj.RefreshDisplay();
        }
    }

    private static bool IsNumberedSuit(TileSuit suit)
    {
        return suit == TileSuit.Bamboo || suit == TileSuit.Dot || suit == TileSuit.Character;
    }

    // Increase the player's max HP and current HP by the given amount. Does not use selected tiles.
    // Updates the health bar UI if Player.instance is available.
    public static void IncreaseMaxHealth(int amount)
    {
        if (GameManager.playerData == null || amount <= 0) return;

        GameManager.playerData.maxHealth += amount;
        GameManager.playerData.health += amount;

        if (Player.instance != null)
            Player.instance.RefreshHealthBar();
    }

    // Doubles up to a certain amount of qi
    public static void DoubleQi(int maxThreshold)
    {
        if (Player.instance == null || GameManager.playerData == null || maxThreshold < 0) return;

        int current = GameManager.playerData.qi;
        int newQi = current <= maxThreshold ? current * 2 : current + maxThreshold;
        int delta = newQi - current;
        if (delta <= 0) return;

        Player.instance.AddQi(delta);
    }
}
