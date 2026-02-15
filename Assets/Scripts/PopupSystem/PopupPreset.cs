using UnityEngine;

[CreateAssetMenu(menuName = "Popup System/Popup Preset")]
public class PopupPreset : ScriptableObject
{
    [Header("Timing")]
    public float duration = 1f;

    [Header("Motion")]
    public Vector3 startOffset = Vector3.zero;
    public Vector3 endOffset = new Vector3(0, 2f, 0);

    [Header("Curves")]
    public AnimationCurve positionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 1);
    public AnimationCurve opacityCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Visual")]
    public Sprite sprite;
    public Gradient colorOverLifetime;
    public float startScale = 1f;
}
