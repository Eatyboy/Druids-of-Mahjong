using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public void NewGame()
    {
        GameManager.instance.GoToCombat();
    }

    public void Quit()
    {
        Utils.QuitGame();
    }
}
