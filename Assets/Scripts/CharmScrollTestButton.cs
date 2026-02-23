using UnityEngine;

/// <summary>
/// Temporary test script. Attach to a Button and assign RemoveSelectedTiles to OnClick.
/// During battle, select one or more tiles, then press the button to remove them from the hand.
/// </summary>
public class CharmScrollTestButton : MonoBehaviour
{
    public void RemoveSelectedTiles()
    {
        if (PlayerHand.instance == null) return;

        CharmScrollActions.RemoveTilesFromHand(PlayerHand.instance, PlayerHand.instance.selectedTiles);
    }

    public void CopySelectedTiles()
    {
        if (PlayerHand.instance == null) return;

        CharmScrollActions.AddCopiesToHand(PlayerHand.instance, PlayerHand.instance.selectedTiles);
    }

    public void SwitchTileSuit()
    {
        if (PlayerHand.instance == null) return;

        CharmScrollActions.SwitchTileSuit(PlayerHand.instance, PlayerHand.instance.selectedTiles, TileSuit.Bamboo);
    }

    public void IncreaseMaxHealth()
    {
        if (Player.instance == null) return;

        CharmScrollActions.IncreaseMaxHealth(5);
    }
}
