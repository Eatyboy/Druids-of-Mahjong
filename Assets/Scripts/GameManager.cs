using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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
        await SceneManager.LoadSceneAsync(Bootstrapper.combatScreenSceneName, LoadSceneMode.Single);

        //gameState = GameState.InCombat;
    }

    public async void GoToTree()
    {
        await SceneManager.LoadSceneAsync(Bootstrapper.treeScreenSceneName, LoadSceneMode.Single);
        //gameState = GameState.AtTree;
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Utils.QuitGame();
        }
    }
}
