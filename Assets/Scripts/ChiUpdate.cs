using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChiUpdate : MonoBehaviour
{
    public TMP_Text chiAmt; // References the ChiAmt text
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chiAmt.text = Random.Range(0, 100).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
