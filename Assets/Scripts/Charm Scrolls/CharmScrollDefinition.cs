using UnityEngine;

/// <summary>
/// Defines a single charm scroll: its type and any parameters.
/// For Remove/Copy/Switch: intParam = max tiles that can be selected (min 0). For IncreaseMaxHealth/DoubleQi: intParam = HP amount / qi threshold.
/// </summary>
[CreateAssetMenu(fileName = "CharmScroll_", menuName = "Charm Scroll Definition")]
public class CharmScrollDefinition : ScriptableObject
{
    public string scrollName;
    [TextArea(2, 4)]
    public string description;

    public CharmScrollType type;

    [Tooltip("Remove/Copy/Switch: max tiles selectable (0 = no limit in practice). IncreaseMaxHealth: HP amount. DoubleQi: max threshold.")]
    public int intParam = 5;
}
