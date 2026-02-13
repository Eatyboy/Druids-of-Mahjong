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

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    void Start()
    {
        CombatManager.instance.StartCombat();
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Utils.QuitGame();
        }
    }
}
