using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public void Start()
    {
        AudioManager.instance.PlayMusic(AudioManager.instance.titleScreenMusic);
    }

    public void NewGame()
    {
        GameManager.instance.GoToCombat();
    }

    public void Quit()
    {
        Utils.QuitGame();
    }
}
