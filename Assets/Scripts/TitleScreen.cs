using System.IO;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private GameObject loadGameButton;

    public void Start()
    {
        AudioManager.instance.PlayMusic(AudioManager.instance.titleScreenMusic);

        loadGameButton.SetActive(File.Exists(SaveSystem.savePath));
    }

    public async void NewGameEvent()
    {
        await GameManager.instance.NewGame();
    }

    public async void LoadGameEvent()
    {
        await GameManager.instance.LoadGame();
    }

    public void Quit()
    {
        Utils.QuitGame();
    }
}
