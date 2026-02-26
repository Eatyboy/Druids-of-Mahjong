using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Hand used for charm scrolls: draw tiles, select them, apply scroll actions.
/// No combat-state checks; selection is always allowed. Set current scroll via SetCurrentCharmScroll; validation and execution go through CharmScrollRunner.
/// </summary>
public class ScrollHand : HandBase
{
    public static ScrollHand instance { get; private set; }

    [Header("Current scroll")]
    [SerializeField] private CharmScrollDefinition currentCharmScroll;

    [Header("Scroll UI")]
    [SerializeField] private GameObject selectButton;
    [SerializeField] private TextMeshProUGUI selectButtonText;

    [Header("After apply")]
    [Tooltip("Invoked after the charm scroll action runs (e.g. exit menu, finish charm scroll usage).")]
    [SerializeField] private UnityEvent onCharmScrollFinished;

    /// <summary>Add a one-shot listener that is removed when the scroll is applied. Use from QiTreeManager to close the tab.</summary>
    public void AddCharmScrollFinishedListener(UnityAction callback)
    {
        if (onCharmScrollFinished == null || callback == null) return;
        UnityAction wrapped = null;
        wrapped = () =>
        {
            onCharmScrollFinished.RemoveListener(wrapped);
            callback?.Invoke();
        };
        onCharmScrollFinished.AddListener(wrapped);
    }

    /// <summary>
    /// Set the currently selected charm scroll (e.g. when the player picks a scroll in the menu).
    /// Call this before showing the scroll hand so validation and execution use the right scroll.
    /// </summary>
    public void SetCurrentCharmScroll(CharmScrollDefinition definition)
    {
        currentCharmScroll = definition;
        UpdateSelectButtonState();
    }

    public CharmScrollDefinition GetCurrentCharmScroll() => currentCharmScroll;

    protected override void Awake()
    {
        base.Awake();

        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        if (selectButton != null) selectButton.SetActive(false);
    }

    public override void SelectTile(PlayerTileObject tile)
    {
        base.SelectTile(tile);
        UpdateSelectButtonState();
    }

    public override void DeselectTile(PlayerTileObject tile)
    {
        base.DeselectTile(tile);
        UpdateSelectButtonState();
    }

    /// <summary>
    /// Show/hide the select button and set it enabled only when the current selection is valid for the pre-selected scroll.
    /// </summary>
    private void UpdateSelectButtonState()
    {
        if (selectButton == null) return;
        var button = selectButton.GetComponent<Button>();

        if (currentCharmScroll == null)
        {
            selectButton.SetActive(true);
            if (button != null) button.interactable = false;
            return;
        }

        bool hasSelection = selectedTiles != null && selectedTiles.Count > 0;
        bool requiresTiles = CharmScrollRunner.ScrollRequiresTileSelection(currentCharmScroll);

        if (!hasSelection && requiresTiles)
        {
            selectButton.SetActive(true);
            if (button != null) button.interactable = false;
            return;
        }

        selectButton.SetActive(true);
        if (button != null) button.interactable = IsSelectionValidForScroll();
    }

    /// <summary>
    /// Uses CharmScrollRunner validation for the current scroll. Override to add extra conditions.
    /// </summary>
    protected virtual bool IsSelectionValidForScroll()
    {
        return CharmScrollRunner.IsSelectionValid(currentCharmScroll, this);
    }

    public override IEnumerator SortTilesInHand()
    {
        yield return base.SortTilesInHand();
        if (selectButtonText != null) selectButtonText.text = "Apply";
        UpdateSelectButtonState();
    }

    /// <summary>
    /// Wire this to the select button's OnClick. Runs the current charm scroll action, then invokes onCharmScrollFinished (e.g. exit menu).
    /// </summary>
    public void OnSelectButtonClicked()
    {
        if (currentCharmScroll == null) return;
        if (!IsSelectionValidForScroll()) return;

        CharmScrollRunner.Execute(currentCharmScroll, this);
        onCharmScrollFinished?.Invoke();
    }
}
