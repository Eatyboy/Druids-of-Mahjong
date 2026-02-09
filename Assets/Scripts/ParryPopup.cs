using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ParryPopup : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI inputKeyText;
    [SerializeField] private string inputKeyName;

    private void Awake()
    {
        inputKeyText.text = inputKeyName.ToUpper();
        gameObject.SetActive(false);
    }

    public void Open(float duration, Vector2 position)
    {
        image.fillAmount = 1.0f;
        gameObject.SetActive(true);
        (transform as RectTransform).position = position;
        StartCoroutine(Animate(duration));
    }

    private IEnumerator Animate(float duration)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.fillAmount = elapsedTime / duration;
            yield return null;
        }
    }

    public void Close()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
