using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }
}
