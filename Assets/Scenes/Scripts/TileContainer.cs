using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileContainer : UIBehaviour
{
    [SerializeField] private float spacing = 10.0f;
    [SerializeField] private float animationSpeed = 10.0f;


    private readonly List<RectTransform> children = new();
    private readonly Dictionary<RectTransform, float> targetX = new();

    protected override void Awake()
    {
        RefreshChildren();
    }

    private void OnTransformChildrenChanged()
    {
        RefreshChildren();
    }

    public void RefreshChildren()
    {
        children.Clear();

        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf) continue;

            RectTransform rt = child as RectTransform;
            children.Add(rt);
        }

        RecalculateTargets();
    }

    public void RecalculateTargets()
    {
        if (children.Count == 0) return;

        float totalWidth = 0.0f;
        foreach (var child in children) totalWidth += child.rect.width;
        totalWidth += spacing * (children.Count - 1);

        float x = -totalWidth * 0.5f;

        foreach (var child in children)
        {
            float w = child.rect.width;
            targetX[child] = x + w * 0.5f;
            x += w + spacing;
        }
    }

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;

        foreach (var child in children)
        {
            float t = 1.0f - Mathf.Exp(-animationSpeed * dt);
            float newX = Mathf.Lerp(child.anchoredPosition.x, targetX[child], t);

            child.anchoredPosition = new(newX, child.anchoredPosition.y);
        }
    }
}
