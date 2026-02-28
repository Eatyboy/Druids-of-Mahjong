using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Connects charm scroll definitions to CharmScrollActions: validation and execution per scroll type.
/// </summary>
public static class CharmScrollRunner
{
    /// <summary>
    /// True if this scroll type does not require any tiles to be selected (e.g. IncreaseMaxHealth, DoubleQi).
    /// </summary>
    public static bool ScrollRequiresTileSelection(CharmScrollDefinition definition)
    {
        if (definition == null) return true;
        switch (definition.type)
        {
            case CharmScrollType.IncreaseMaxHealth:
            case CharmScrollType.DoubleQi:
                return false;
            default:
                return true;
        }
    }

    /// <summary>
    /// Returns true if the current hand selection is valid for the given scroll.
    /// </summary>
    public static bool IsSelectionValid(CharmScrollDefinition definition, HandBase hand)
    {
        if (definition == null || hand == null) return false;

        var selected = hand.selectedTiles;
        int count = selected != null ? selected.Count : 0;

        switch (definition.type)
        {
            case CharmScrollType.RemoveTiles:
            case CharmScrollType.AddCopies:
                return count >= 0 && count <= definition.intParam;
            case CharmScrollType.SwitchToBamboo:
            case CharmScrollType.SwitchToDot:
            case CharmScrollType.SwitchToCharacter:
                return count >= 0 && count <= definition.intParam;
            case CharmScrollType.IncreaseMaxHealth:
                return definition.intParam > 0;
            case CharmScrollType.DoubleQi:
                return definition.intParam >= 0;
            default:
                return false;
        }
    }

    /// <summary>
    /// Execute the charm scroll action for the given definition on the hand (uses selected tiles where applicable).
    /// </summary>
    public static void Execute(CharmScrollDefinition definition, HandBase hand)
    {
        if (definition == null || hand == null) return;

        var selected = hand.selectedTiles ?? new List<PlayerTileObject>();

        switch (definition.type)
        {
            case CharmScrollType.RemoveTiles:
                CharmScrollActions.RemoveTilesFromHand(hand, selected);
                break;
            case CharmScrollType.AddCopies:
                CharmScrollActions.AddCopiesToHand(hand, selected);
                break;
            case CharmScrollType.SwitchToBamboo:
                CharmScrollActions.SwitchTileSuit(hand, selected, TileSuit.Bamboo);
                break;
            case CharmScrollType.SwitchToDot:
                CharmScrollActions.SwitchTileSuit(hand, selected, TileSuit.Dot);
                break;
            case CharmScrollType.SwitchToCharacter:
                CharmScrollActions.SwitchTileSuit(hand, selected, TileSuit.Character);
                break;
            case CharmScrollType.IncreaseMaxHealth:
                CharmScrollActions.IncreaseMaxHealth(definition.intParam);
                break;
            case CharmScrollType.DoubleQi:
                CharmScrollActions.DoubleQi(definition.intParam);
                break;
        }
    }
}
