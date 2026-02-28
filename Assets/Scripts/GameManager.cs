using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState
{
    TitleScreen,
    InMap,
    InCombat,
    AtTree,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //public GameState gameState = GameState.TitleScreen;
    private PlayerData _playerData = null;
    public static PlayerData playerData => instance._playerData;

    // Enemy Data
    public float hpScale = 1.0f; 
    public float hpScaleRate = 1.5f;

    [SerializeField] private float transitionFadeDuration = 1.0f;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
//#if UNITY_EDITOR
//        _playerData = new PlayerData();
//#endif
    }

    public async Task NewGame()
    {
        await SaveSystem.DeleteSave();
        _playerData = new();
        TilesManager.instance.InitializeDeck();
        await SaveSystem.Save(_playerData);
        GoToCombat();
    }

    public async Task LoadGame()
    {
        _playerData = await SaveSystem.Load();
        if (_playerData.gameState == GameState.InCombat)
        {
            GoToCombat();
        }
        else if (_playerData.gameState == GameState.AtTree)
        {
            GoToTree();
        }
        else
        {
            Debug.LogError($"Invalid Player data game state: {_playerData.gameState}");
            QuitToTitleScreen();
        }
    }

    public async void QuitToTitleScreen()
    {
        if (TilesManager.instance != null)
            TilesManager.instance.ReturnScrollHandToDeck();
        AudioManager.instance.StopMusic();

        await CoroutineTask.Run(this, ScreenFader.FadeOut(transitionFadeDuration));
        await SaveSystem.Save(_playerData);
        await SceneManager.LoadSceneAsync(Bootstrapper.titleScreenSceneName, LoadSceneMode.Single);
        await CoroutineTask.Run(this, ScreenFader.FadeIn(transitionFadeDuration));

        _playerData.gameState = GameState.TitleScreen;
    }

    public async void GoToCombat()
    {
        AudioManager.instance.StopMusic();

        await CoroutineTask.Run(this, ScreenFader.FadeOut(transitionFadeDuration));
        await SceneManager.LoadSceneAsync(Bootstrapper.combatScreenSceneName, LoadSceneMode.Single);
        await CoroutineTask.Run(this, ScreenFader.FadeIn(transitionFadeDuration));

        _playerData.gameState = GameState.InCombat;
    }

    public async void GoToTree()
    {
        AudioManager.instance.StopMusic();

        await CoroutineTask.Run(this, ScreenFader.FadeOut(transitionFadeDuration));
        await SceneManager.LoadSceneAsync(Bootstrapper.treeScreenSceneName, LoadSceneMode.Single);
        await CoroutineTask.Run(this, ScreenFader.FadeIn(transitionFadeDuration));

        _playerData.gameState = GameState.AtTree;
    }

    //public void Update()
    //{
    //    if (Keyboard.current.escapeKey.wasReleasedThisFrame)
    //    {
    //        Utils.QuitGame();
    //    }
    //}

    // This is only for testing
#if UNITY_EDITOR
    [ContextMenu("Delete Save Data")]
    private async void DeleteSaveData()
    {
        await SaveSystem.DeleteSave();
    }
#endif
}
