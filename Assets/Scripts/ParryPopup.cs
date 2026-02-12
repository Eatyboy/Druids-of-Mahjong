using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ParryPopup : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI inputKeyText;
    [SerializeField] private string inputKeyName;
    [SerializeField] private Vector2 offsetFromEnemyAttackTile;

    private void Start()
    {
        inputKeyText.text = inputKeyName.ToUpper();
    }

    public void Open(Vector2 position)
    {
        image.fillAmount = 1.0f;
        (transform as RectTransform).position = position + offsetFromEnemyAttackTile;
    }
}
