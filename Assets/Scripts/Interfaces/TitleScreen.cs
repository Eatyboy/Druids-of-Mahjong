using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onNewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void onQuit()
    {
        Application.Quit();
    }
}
