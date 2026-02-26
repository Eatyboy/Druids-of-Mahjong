using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//public enum GameState
//{
//    TitleScreen,
//    InMap,
//    InCombat,
//    AtTree,
//}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //public GameState gameState = GameState.TitleScreen;
    private PlayerData _playerData = null;
    public static PlayerData playerData => instance._playerData;

    // Enemy Data
    public float hpScale = 1.0f; 
    public float hpScaleRate = 1.5f;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
#if UNITY_EDITOR
        _playerData = new PlayerData();
#else
        _playerData = SaveSystem.LoadData();
#endif
    }

    public async void QuitToTitleScreen()
    {
        await SceneManager.LoadSceneAsync(Bootstrapper.titleScreenSceneName, LoadSceneMode.Single);

        //gameState = GameState.TitleScreen;
    }

    public async void GoToCombat()
    {
        AudioManager.instance.StopMusic();

        await SceneManager.LoadSceneAsync(Bootstrapper.combatScreenSceneName, LoadSceneMode.Single);

        AudioManager.instance.PlayMusic(AudioManager.instance.combatMusic);

        //gameState = GameState.InCombat;
    }

    public async void GoToTree()
    {
        AudioManager.instance.StopMusic();

        await SceneManager.LoadSceneAsync(Bootstrapper.treeScreenSceneName, LoadSceneMode.Single);

        AudioManager.instance.PlayMusic(AudioManager.instance.treeMusic);
        //gameState = GameState.AtTree;
    }

    public void Update()
    {
        if (Keyboard.current.escapeKey.wasReleasedThisFrame)
        {
            Utils.QuitGame();
        }
    }
}
