using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System;

public class EnemyAttackTileObject : MonoBehaviour
{
    public RectTransform rt;

    public Tile tileData;
    public Image tileBackImage;
    public Image tileFaceImage;

    [Header("Procedural Animation")]
    [SerializeField] private int animationFPS;
    public bool isInAnimation;
    public float drawAnimationDuration;
    public float attackAnimationDuration;
    public float parriedAnimationDuration;
    [SerializeField] private Vector2 idleOffset;
    [SerializeField] private Vector2 initialScale;
    [SerializeField] private Vector2 initialPosition;

    private Vector2[] drawBezierPoints;
    private Vector2[] attackBezierPoints;
    private Vector2[] parriedBezierPoints;

    [SerializeField] private TextMeshProUGUI tmpElement;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        isInAnimation = false;
    }

    public void Initialize(Tile tile, Vector2 initPos, Vector2 offset)
    {
        tileData = tile;
        idleOffset = offset;
        initialPosition = initPos;
        rt.position = initialPosition;
        initialScale = new(rt.rect.width, rt.rect.height);
        tmpElement.text = tile.rank.ToString() + " of " + tile.suit.ToString();

        Vector2 drawBezierOffset = new(initPos.x + 100.0f, initPos.y);
        drawBezierPoints = new Vector2[] {initPos, drawBezierOffset, initPos + offset};

        Vector2 attackBezierOffset = new(initPos.x + 200.0f, initPos.y);
        Vector2 attackFinalPoint = new(initPos.x, initPos.y - 100.0f);
        attackBezierPoints = new Vector2[] {drawBezierPoints[2], attackBezierOffset, attackFinalPoint};

        Vector2 parryBezierOffset = new(initPos.x, initPos.y + 250.0f);
        Vector2 parryFinalPoint = new(initPos.x, initPos.y + 200.0f);
        parriedBezierPoints = new Vector2[] {drawBezierPoints[2], attackBezierOffset, attackFinalPoint};
    }

    public IEnumerator PlayDrawAnimation()
    {
        isInAnimation = true;

        rt.sizeDelta = new(0, 0);  

        yield return new WaitForSeconds(0.25f);

        float spf = 1.0f/(float)animationFPS;
        for (float t = 0; t < drawAnimationDuration; t += spf)
        {
            rt.sizeDelta = new(initialScale.x * (t / drawAnimationDuration), initialScale.y * (t / drawAnimationDuration));

            Vector2 tileVel = Utils.SlopeOnQuadraticBezierCurve2D(t, drawBezierPoints[0], drawBezierPoints[1], drawBezierPoints[2], drawAnimationDuration);
            rt.anchoredPosition += tileVel * spf;
            // rt.anchoredPosition = Utils.PointOnQuadraticBezierCurve2D(t, drawBezierPoints[0], drawBezierPoints[1], drawBezierPoints[2], attackAnimationDuration);

            float tileRotationAngle = -90.0f * (float)Math.Exp(-2.2f * t / drawAnimationDuration);
            if (Math.Abs(tileRotationAngle) < 5.0f) tileRotationAngle = 0.0f;
            rt.rotation = Quaternion.Euler(0.0f, 0.0f, tileRotationAngle);

            yield return new WaitForSeconds(spf);
        }

        isInAnimation = false;
        yield return null;
    }

    public IEnumerator PlayAttackAnimation()
    {
        isInAnimation = true;

        float spf = 1.0f/(float)animationFPS;
        for (float t = 0; t < attackAnimationDuration; t += spf)
        {

            Vector2 tileVel = Utils.SlopeOnQuadraticBezierCurve2D(t, attackBezierPoints[0], attackBezierPoints[1], attackBezierPoints[2], attackAnimationDuration);
            rt.anchoredPosition += tileVel * spf;
            // rt.anchoredPosition = Utils.PointOnQuadraticBezierCurve2D(t, drawBezierPoints[0], drawBezierPoints[1], drawBezierPoints[2], attackAnimationDuration);
            
            yield return new WaitForSeconds(spf);
        }

        isInAnimation = false;
        yield return null;
    }

    public IEnumerator PlayParriedAnimation()
    {
        isInAnimation = true;

        float spf = 1.0f/(float)animationFPS;
        for (float t = 0; t < parriedAnimationDuration; t += spf)
        {
            Vector2 tileVel = Utils.SlopeOnQuadraticBezierCurve2D(t, parriedBezierPoints[0], parriedBezierPoints[1], parriedBezierPoints[2], parriedAnimationDuration);
            rt.anchoredPosition += tileVel * spf;
            // rt.anchoredPosition = Utils.PointOnQuadraticBezierCurve2D(t, drawBezierPoints[0], drawBezierPoints[1], drawBezierPoints[2], attackAnimationDuration);

            float tileRotationAngle = -180.0f *(1.0f - (float)Math.Exp(-2.0f * t / parriedAnimationDuration)); 
            if (Math.Abs(tileRotationAngle) > 175.0f) tileRotationAngle = 180.0f;
            rt.rotation = Quaternion.Euler(0.0f, 0.0f, tileRotationAngle);
            
            yield return new WaitForSeconds(spf);
        }

        isInAnimation = false;
        yield return null;
    }
}
