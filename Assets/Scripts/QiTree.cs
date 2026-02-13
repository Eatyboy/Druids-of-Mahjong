using UnityEngine;
using TMPro;

public class QiTree : MonoBehaviour
{
    public TextMeshProUGUI qiText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        qiText.text = "Qi: " + Player.instance.qi;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onFlowerClick()
    {
        Player.instance.qi -= 1;
        qiText.text = "Qi: " + Player.instance.qi;
    }
}

