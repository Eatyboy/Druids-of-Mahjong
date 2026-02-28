using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FlowerTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private static IEnumerator NoOpCoroutine() { yield break; }
    public FlowerTileInstance instance;
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;

    [SerializeField] private Image image;

    public FlowerTileInfoController infoController;
    private RectTransform container;

    [Header("Movement")]
    private Vector2 startPosition;
    private Vector2 targetPosition;
    [SerializeField] private float returnDuration = 1.0f;
    private float returningElapsedTime;
    private bool isReturning = false;

    [Header("Punch Rotation")]
    [SerializeField] private float punchAngle = -45.0f;
    [SerializeField] private float rotationDuration = 1.0f;
    private float startAngle;
    private float targetAngle;
    private float rotatingElapsedTime;
    private bool isRotating = false;

    [Header("Dragging")]
    [HideInInspector] public UnityEvent<FlowerTile> BeginDragEvent;
    [HideInInspector] public UnityEvent<FlowerTile> EndDragEvent;
    [SerializeField] private Canvas tileContainer; // Assign the Tile Container to it
    private Vector3 ogPosition;
    public int ogIndex;
    private Vector3 offset;
    public bool isDraggable = false; // Updated once added to player tiles
    public bool isDragging = false;

    [Header("Hover Scaling")]
    public Vector3 ogScale;
    public float scaleFactor = 1.2f;
    [SerializeField] private float scaleDuration;
    private Vector3 startScale;
    private Vector3 targetScale;
    private float scalingElapsedTime;
    private bool isScaling = false;

    // need this to differentiate between flower tiles already initialized and those not (f key vs bought) to prevent stacking
    public bool initialized;

    private void Start()
    {
        tileContainer = GetComponentInParent<Canvas>();
        ogScale = transform.localScale;
        targetPosition = rectTransform.anchoredPosition;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialized = false;
    }
    
    private Vector2 ScreenPointToPointInContainer(Vector2 point)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            container,
            point,
            null,
            out Vector2 localPoint
        );

        return localPoint;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector2 localMousePos = ScreenPointToPointInContainer(Mouse.current.position.ReadValue());
            Vector2 targetPosition = localMousePos + (Vector2)offset;
            rectTransform.anchoredPosition = targetPosition;
        }

        if (isReturning)
        {
            returningElapsedTime += Time.deltaTime;
            float t = returningElapsedTime / returnDuration;
            t = Utils.ExpEaseIn(t, 4.0f);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            if (returningElapsedTime >= returnDuration)
            {
                isReturning = false;
                rectTransform.anchoredPosition = targetPosition;
                canvasGroup.blocksRaycasts = true;
            }
        }

        if (isRotating)
        {
            rotatingElapsedTime += Time.deltaTime;
            float t = rotatingElapsedTime / returnDuration;
            t = Mathf.Sin(t * Mathf.PI) * Mathf.Sin(t * Mathf.PI);
            float currentAngle = Mathf.Lerp(startAngle, targetAngle, t);
            transform.eulerAngles = new Vector3(0, 0, currentAngle);

            if (rotatingElapsedTime >= returnDuration)
            {
                isRotating = false;
                transform.eulerAngles = new Vector3(0, 0, startAngle);
            }
        }

        if (isScaling)
        {
            scalingElapsedTime += Time.deltaTime;
            float t = scalingElapsedTime + Time.deltaTime / scaleDuration;
            t = Mathf.SmoothStep(0.0f, 1.0f, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            if (scalingElapsedTime >= scaleDuration)
            {
                isScaling = false;
                transform.localScale = targetScale;
            }
        }
    }

    public void Initialize(FlowerTileInstance flowerTileInstance, RectTransform container, FlowerTileInfoController infoController)
    {
        this.instance = flowerTileInstance;
        this.infoController = infoController;
        this.container = container;
        UpdateImage();

        // can be null if not in combat scene; will be checked for initialization again in GameManager pre-battle state
        if (initialized || CombatManager.instance == null) return;

        CombatManager.instance.EnqueueAction(() =>
        {
            if (flowerTileInstance?.effect == null || PlayerHand.instance == null)
                return NoOpCoroutine();
            return flowerTileInstance.effect.OnInitialize(
                PlayerHand.instance.GetPlayerHandTileData(),
                PlayerHand.instance.GetSelectedTileData()
            );
        },
            nameof(flowerTileInstance.effect.OnInitialize)
        );

        initialized = true;
        isDraggable = true;
    }

    public void UpdateImage()
    {
        image.sprite = instance.data.sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoController.currentFlowerTile = this;
        infoController.PointerEntered();

        StartScaleAnim(ogScale * scaleFactor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoController.PointerExited();
        StartScaleAnim(ogScale);
    }

    public void StartScaleAnim(Vector3 endScale)
    {
        startScale = transform.localScale;
        targetScale = endScale;
        scalingElapsedTime = 0.0f;
        isScaling = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDraggable) {
            ogPosition = transform.localPosition;
            ogIndex = FlowerTileUI.instance.flowerTileObjects.IndexOf(this);

            BeginDragEvent.Invoke(this);
            Vector2 localMousePos = ScreenPointToPointInContainer(Mouse.current.position.ReadValue());
            offset = rectTransform.anchoredPosition - localMousePos;
            tileContainer.GetComponent<GraphicRaycaster>().enabled = false;
            image.raycastTarget = false;
            isDragging = true;
            transform.SetAsLastSibling();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent.Invoke(this);
        isDragging = false;
        tileContainer.GetComponent<GraphicRaycaster>().enabled = true;
        image.raycastTarget = true;

        StartReturnAnim(doPunch: true);

        isDragging = false;
        FlowerTileUI.instance.selectedTile = null;
    }

    public void StartReturnAnim(bool doPunch = false)
    {
        int index = FlowerTileUI.instance.flowerTileObjects.IndexOf(this);
        startPosition = rectTransform.anchoredPosition;
        targetPosition = FlowerTileUI.instance.GetSlotPosition(index);
        returningElapsedTime = 0.0f;
        isReturning = true;

        if (doPunch)
        {
            startAngle = transform.eulerAngles.z;
            targetAngle = startAngle + punchAngle;
            rotatingElapsedTime = 0.0f;
            isRotating = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        FlowerTileUI.instance.selectedTile = this;
    }
}
