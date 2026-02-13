using UnityEngine;
using System.Collections.Generic;

public class PopupSystem : MonoBehaviour
{
    public static PopupSystem Instance;

    [SerializeField] private PopupInstance popupPrefab;
    [SerializeField] private int poolSize = 20;

    private Queue<PopupInstance> pool = new Queue<PopupInstance>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            var popup = Instantiate(popupPrefab, transform);
            popup.gameObject.SetActive(false);
            pool.Enqueue(popup);
        }
    }

    public void OpenPopup(PopupPreset preset, string value, Vector3 worldPos, Transform follow = null)
    {
        var popup = Get();

        popup.transform.position = worldPos;
        popup.gameObject.SetActive(true);
        popup.Initialize(preset, value, follow);
    }

    private PopupInstance Get()
    {
        if (pool.Count > 0)
            return pool.Dequeue();

        return Instantiate(popupPrefab, transform);
    }

    public void Release(PopupInstance popup)
    {
        popup.gameObject.SetActive(false);
        pool.Enqueue(popup);
    }
}
