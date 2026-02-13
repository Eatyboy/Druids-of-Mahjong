using UnityEngine;

public class PopupTester : MonoBehaviour
{
    public PopupPreset damagePreset;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PopupSystem.Instance.OpenPopup(
                damagePreset,
                Random.Range(10, 99).ToString(),
                transform.position,
                transform
            );
        }
    }
}
