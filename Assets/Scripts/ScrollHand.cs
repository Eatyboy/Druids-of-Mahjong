using UnityEngine;

/// <summary>
/// Hand used for charm scrolls: draw tiles, select them, apply scroll actions.
/// No combat-state checks; selection is always allowed. Use with CharmScrollActions.
/// </summary>
public class ScrollHand : HandBase
{
    public static ScrollHand instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    // SelectTile / DeselectTile: use base implementation (no combat guard)
}
