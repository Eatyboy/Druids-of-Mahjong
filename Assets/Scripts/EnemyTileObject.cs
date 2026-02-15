using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System;
using Unity.VisualScripting;
using UnityEngine.Splines;
using Unity.Mathematics;

public class EnemyTileObject : TileObject
{
    [Header("Procedural Animation")]
    public bool isInAnimation;
    public float drawAnimationDuration;
    public float attackAnimationDuration;
    public float parriedAnimationDuration;

    private Vector2 initialPosition;
    private Vector2 initialScale;
    private Spline drawSpline;
    private Spline attackSpline;
    private Spline parrySpline;

    protected override void Awake()
    {
        base.Awake();

        isInAnimation = false;
    }

    public void Initialize(Tile tile, Vector2 initPos, 
        Spline drawSpline, Spline attackSpline, Spline parrySpline)
    {
        base.Initialize(tile);

        initialPosition = initPos;
        initialScale = new(rt.rect.width, rt.rect.height);

        this.drawSpline = drawSpline;
        this.attackSpline = attackSpline;
        this.parrySpline = parrySpline;

        rt.position = initPos;
    }

    public IEnumerator PlayDrawAnimation()
    {
        isInAnimation = true;

        rt.sizeDelta = new(0, 0);

        float elapsedTime = 0.0f;
        while (elapsedTime < drawAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / drawAnimationDuration);
            t = Mathf.SmoothStep(0.0f, 1.0f, t);

            rt.sizeDelta = new(initialScale.x * t, initialScale.y * t);

            Vector3 pos = drawSpline.EvaluatePosition(t);
            rt.anchoredPosition = (Vector2)pos;

            float tileRotationAngle = -90.0f * (float)Math.Exp(-3.0f * t);
            if (Math.Abs(tileRotationAngle) < 5.0f) tileRotationAngle = 0.0f;
            rt.rotation = Quaternion.Euler(0.0f, 0.0f, tileRotationAngle);

            yield return null;
        }

        Vector3 finalPos = drawSpline.EvaluatePosition(1.0f);
        rt.anchoredPosition = (Vector2)finalPos;
        isInAnimation = false;
        yield break;
    }

    public IEnumerator PlayAttackAnimation()
    {
        isInAnimation = true;

        float elapsedTime = 0.0f;
        while (elapsedTime < attackAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / attackAnimationDuration);
            t = Utils.ExpEaseIn(t);

            rt.sizeDelta = initialScale * (float)Math.Exp(0.35f * t);
            rt.rotation = Quaternion.Euler(0.0f, 0.0f, -180.0f * (1.0f - (float)Math.Exp(-3.0f * t)));
            Vector3 pos = attackSpline.EvaluatePosition(t);
            rt.anchoredPosition = (Vector2)pos;

            yield return null;
        }

        rt.sizeDelta = initialScale * (float)Math.Exp(0.35f);
        rt.rotation = Quaternion.Euler(0.0f, 0.0f, -180.0f * (1.0f - (float)Math.Exp(-3.0f)));
        Vector3 finalPos = attackSpline.EvaluatePosition(1.0f);
        rt.anchoredPosition = (Vector2)finalPos;
        isInAnimation = false;
        yield break;
    }

    public IEnumerator PlayParriedAnimation()
    {
        isInAnimation = true;

        float elapsedTime = 0.0f;
        while (elapsedTime < parriedAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / parriedAnimationDuration);
            t = Utils.ExpEaseIn(t);

            Vector3 pos = parrySpline.EvaluatePosition(t);
            rt.anchoredPosition = (Vector2)pos;

            rt.rotation = Quaternion.Euler(0.0f, 0.0f, 225.0f * (1.0f - (float)Math.Exp(-1.0f * t)));

            yield return null;
        }

        Vector3 finalPos = parrySpline.EvaluatePosition(1.0f);
        rt.anchoredPosition = (Vector2)finalPos;
        rt.rotation = Quaternion.Euler(0.0f, 0.0f, 225.0f * (1.0f - (float)Math.Exp(-1.0f)));
        isInAnimation = false;
        yield break;
    }
}
