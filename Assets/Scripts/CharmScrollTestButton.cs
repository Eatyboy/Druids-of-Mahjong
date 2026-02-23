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
}
