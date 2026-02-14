using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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

    public GameState gameState = GameState.TitleScreen;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    void Start()
    {
    }

    public async void QuitToTileScreen()
    {
        await SceneManager.LoadSceneAsync("TitleScreen", LoadSceneMode.Single);

        gameState = GameState.TitleScreen;
    }

    public async void GoToCombat()
    {
        await SceneManager.LoadSceneAsync("Combat", LoadSceneMode.Single);

        gameState = GameState.InCombat;
        CombatManager.instance.StartCombat();
    }

    public async void GoToTree()
    {
        await SceneManager.LoadSceneAsync("UpgradeTree", LoadSceneMode.Single);
        gameState = GameState.AtTree;
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Utils.QuitGame();
        }
    }
}
