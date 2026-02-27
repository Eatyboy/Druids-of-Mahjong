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

    [SerializeField] private Image image;

    public FlowerTileInfoController infoController;
    private RectTransform container;

    // Dragging Variables
    [HideInInspector] public UnityEvent<FlowerTile> BeginDragEvent;
    [HideInInspector] public UnityEvent<FlowerTile> EndDragEvent;
    [SerializeField] private Canvas tileContainer; // Assign the Tile Container to it
    private Vector3 ogPosition;
    public int ogIndex;
    private Vector3 offset;
    public bool isDraggable = false; // Updated once added to player tiles
    public bool isDragging = false;
    [HideInInspector] public bool wasDragged;
    [SerializeField] private float returnDuration = 0.25f;

    // Hover Scale
    public Vector3 ogScale;
    public float scaleFactor = 1.2f;

    // need this to differentiate between flower tiles already initialized and those not (f key vs bought) to prevent stacking
    public bool initialized;

    private void Start()
    {
        tileContainer = GetComponentInParent<Canvas>();
        ogScale = transform.localScale;
        Debug.Log("Local: " + transform.localPosition);
        Debug.Log("World: " + transform.position);
        Debug.Log("Anchored: "+ rectTransform.anchoredPosition);
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
            null, // null if Screen Space Overlay
            out Vector2 localPoint
        );

        return localPoint;
    }

    void Update()
    {
        Debug.Log("Screen: "+ Mouse.current.position.ReadValue());
        Debug.Log("World: " + Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        Debug.Log("Local: " + ScreenPointToPointInContainer(Mouse.current.position.ReadValue()));

        if (isDragging)
        {
            Vector2 localMousePos = ScreenPointToPointInContainer(Mouse.current.position.ReadValue());
            Vector2 targetPosition = localMousePos + (Vector2)offset;
            rectTransform.anchoredPosition = targetPosition;
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
        StartCoroutine(ScaleAnim(ogScale, ogScale * scaleFactor));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoController.PointerExited();
        StartCoroutine(ScaleAnim(transform.localScale, ogScale));
    }

    public IEnumerator ScaleAnim(Vector3 startScale, Vector3 endScale)
    {
        float elapsedTime = 0f;
        float playDuration = 0.01f;

        while (elapsedTime < playDuration)
        {
            transform.localScale = Vector3.Lerp(ogScale, endScale, elapsedTime / playDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDraggable) {
            ogPosition = transform.localPosition;
            ogIndex = FlowerTileContainer.instance.flowerTileObjects.IndexOf(this);

            BeginDragEvent.Invoke(this);
            Vector2 localMousePos = ScreenPointToPointInContainer(Mouse.current.position.ReadValue());
            offset = rectTransform.anchoredPosition - localMousePos;
            tileContainer.GetComponent<GraphicRaycaster>().enabled = false;
            image.raycastTarget = false;
            isDragging = true;
            wasDragged = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent.Invoke(this);
        isDragging = false;
        tileContainer.GetComponent<GraphicRaycaster>().enabled = true;
        image.raycastTarget = true;

        StartCoroutine(ReturnAnim());

        isDragging = false;
        FlowerTileContainer.instance.selectedTile = null;
    }

    public IEnumerator ReturnAnim(float punchAngle = -45f)
    {
        float startRotation = transform.eulerAngles.z;
        float targetRotation = startRotation + punchAngle;

        Vector3 startPos = rectTransform.anchoredPosition;
        int index = FlowerTileContainer.instance.flowerTileObjects.IndexOf(this);
        Vector3 endPos = FlowerTileContainer.instance.GetSlotPosition(index);

        float elapsedTime = 0f;
        float durDecrement = 0.05f;
        float minDuration = 0.1f;

        while (elapsedTime < returnDuration)
        {
            float t = elapsedTime / returnDuration;

            // Punch curve: goes up then back down
            float punchStrength = Mathf.Sin(t * Mathf.PI); // 0 -> 1 -> 0

            float currentAngle = Mathf.Lerp(startRotation, targetRotation, punchStrength);
            transform.eulerAngles = new Vector3(0, 0, currentAngle);   // Punch Rotation Effect
            rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.eulerAngles = new Vector3(0, 0, startRotation);
        rectTransform.anchoredPosition = endPos;
        returnDuration = Mathf.Max(minDuration, returnDuration - durDecrement);
        wasDragged = false;
        yield return new WaitForEndOfFrame();
    }

    public void OnDrag(PointerEventData eventData)
    {
        FlowerTileContainer.instance.selectedTile = this;
    }
}
