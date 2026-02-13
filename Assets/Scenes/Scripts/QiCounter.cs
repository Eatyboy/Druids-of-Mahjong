using TMPro;
using UnityEngine;

public class QiCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI qiText;
    
    public void SetQi(int value)
    {
        qiText.text = value.ToString();
    }
}
