using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.uiClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.uiHover);
    }
}
