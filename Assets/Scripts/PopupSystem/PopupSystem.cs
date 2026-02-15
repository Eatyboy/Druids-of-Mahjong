using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public class PopupSystem : MonoBehaviour
{
    public static PopupSystem instance;

    [SerializeField] private PopupInstance popupPrefab;

    private ObjectPool<PopupInstance> _pool;
    public static ObjectPool<PopupInstance> pool => instance._pool;

    void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        _pool = new(
            createFunc: CreatePopup,
            actionOnGet: OnGet,
            actionOnDestroy: OnDestroyItem,
            actionOnRelease: OnRelease,
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 50 
        );
    }

    public void OpenPopup(PopupPreset preset, string value, Sprite sprite, Vector3 worldPos, Transform follow = null)
    {
        var popup = _pool.Get();

        popup.transform.position = worldPos;
        popup.Initialize(preset, value, sprite, follow);
    }

    private PopupInstance CreatePopup()
    {
        PopupInstance popup = Instantiate(popupPrefab, transform);
        return popup;
    }

    private void OnGet(PopupInstance popup)
    {
        popup.gameObject.SetActive(true);
    }

    public void OnRelease(PopupInstance popup)
    {
        popup.gameObject.SetActive(false);
    }

    public void OnDestroyItem(PopupInstance popup)
    {
        Destroy(popup.gameObject);
    }
}
