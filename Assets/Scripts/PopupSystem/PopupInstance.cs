using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopupInstance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image image;
    [SerializeField] private CanvasGroup canvasGroup;

    private PopupPreset preset;
    private float timer;
    private Vector3 startPos;
    private Transform followTarget;

    public void Initialize(PopupPreset preset, string value, Transform follow = null)
    {
        this.preset = preset;
        text.text = value;
        image.sprite = preset.sprite;

        followTarget = follow;
        startPos = transform.position + preset.startOffset;
        timer = 0f;

        transform.localScale = Vector3.one * preset.startScale;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / preset.duration;

        if (t >= 1f)
        {
            PopupSystem.pool.Release(this);
            return;
        }

        Animate(t);
    }

    private void Animate(float t)
    {
        float posT = preset.positionCurve.Evaluate(t);
        float scaleT = preset.scaleCurve.Evaluate(t);
        float alphaT = preset.opacityCurve.Evaluate(t);

        Vector3 basePos = followTarget ? followTarget.position : startPos;

        transform.position = basePos + Vector3.Lerp(preset.startOffset, preset.endOffset, posT);
        transform.localScale = Vector3.one * scaleT * preset.startScale;
        canvasGroup.alpha = alphaT;

        if (preset.colorOverLifetime != null)
            text.color = preset.colorOverLifetime.Evaluate(t);
    }
}
