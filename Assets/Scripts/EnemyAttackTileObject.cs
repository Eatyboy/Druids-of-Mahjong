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
    [SerializeField] private TextMeshProUGUI label;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        isInAnimation = false;
    }

    public void Initialize(Tile tile, Vector2 initPos, Vector2 offset)
    {
        tileData = tile;
        if (tileData.baseTileData.faceSprite == null )
        {
            tmpElement.text = tile.rank.ToString() + " of " + tile.suit.ToString();
            tileFaceImage.enabled = false;
            label.enabled = false;
        }
        else
        {
            tmpElement.enabled = false;
            tileFaceImage.sprite = tileData.baseTileData.faceSprite;
            label.text = tile.baseTileData.suit switch
            {
                TileSuit.None => "X",
                TileSuit.Bamboo => tile.rank.ToString(),
                TileSuit.Dot => tile.rank.ToString(),
                TileSuit.Character => tile.rank.ToString(),
                TileSuit.Wind => tile.rank switch
                {
                    1 => "N",
                    2 => "E",
                    3 => "S",
                    4 => "W",
                    _ => "X"
                },
                TileSuit.Dragon => tile.rank switch
                {
                    1 => "G",
                    2 => "R",
                    3 => "W",
                    _ => "X"
                },
                _ => "X"
            };
        }

        idleOffset = offset;
        initialPosition = initPos;
        rt.position = initialPosition;
        initialScale = new(rt.rect.width, rt.rect.height);
        tmpElement.text = tile.rank.ToString() + " of " + tile.suit.ToString();

        Vector2 drawBezierOffset = new(initPos.x + 100.0f, initPos.y - 20.0f);
        drawBezierPoints = new Vector2[] {initPos, drawBezierOffset, initPos + offset};

        Vector2 attackBezierOffset = new(initPos.x + 350.0f, initPos.y + 100.0f);
        Vector2 attackFinalPoint = new(initPos.x + 50.0f, initPos.y - 500.0f);
        attackBezierPoints = new Vector2[] {drawBezierPoints[2], attackBezierOffset, attackFinalPoint};

        Vector2 parryBezierOffset = new(drawBezierPoints[2].x + 150.0f, drawBezierPoints[2].y + 250.0f);
        parriedBezierPoints = new Vector2[] {drawBezierPoints[2], parryBezierOffset, initPos};
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

            float tileRotationAngle = -90.0f * (float)Math.Exp(-3.0f * t / drawAnimationDuration);
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
            rt.sizeDelta = initialScale * (float)Math.Exp(0.35f * t);
            rt.rotation = Quaternion.Euler(0.0f, 0.0f, -180.0f * (1.0f - (float)Math.Exp(-3.0f * t / drawAnimationDuration)));
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

            rt.rotation = Quaternion.Euler(0.0f, 0.0f, 225.0f * (1.0f - (float)Math.Exp(-1.0f * t / drawAnimationDuration)));
            
            yield return new WaitForSeconds(spf);
        }

        isInAnimation = false;
        yield return null;
    }
}
