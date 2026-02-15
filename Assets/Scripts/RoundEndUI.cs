using TMPro;
using UnityEngine;

public class RoundEndUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI qiDroppedText;

    public void Initialize(int qiDropped)
    {
        qiDroppedText.text = qiDropped.ToString();
    }

    public void Continue()
    {
        GameManager.instance.GoToTree();
    }
}
