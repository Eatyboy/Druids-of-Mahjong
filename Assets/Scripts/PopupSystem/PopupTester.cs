using UnityEngine;
using UnityEngine.InputSystem;

public class PopupTester : MonoBehaviour
{
    public PopupPreset testPreset;
    [SerializeField] private Sprite testSprite;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            PopupSystem.instance.OpenPopup(
                testPreset,
                transform.position,
                Random.Range(10, 99).ToString(),
                transform
            );
        }
    }
}
