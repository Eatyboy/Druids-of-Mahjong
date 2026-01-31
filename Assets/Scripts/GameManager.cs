using System.Collections;
using UnityEditor;
using UnityEngine;

public enum GameState
{
    TitleScreen,
    InMap,
    InCombat,
    AtTree,
}

public enum CombatState
{
    PreBattle,
    PlayerTurn,
    EnemyTurn,
    PlayerDead,
    EnemyDead,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [System.Serializable]
    public class WeightedEnemyPrefab
    {
        public Enemy enemyPrefab;
        public int spawnWeight = 0; // how likely enemy is to spawn
    }

    public GameState gameState;
    public CombatState combatState;

    // list of possible enemies that can spawn
    public WeightedEnemyPrefab[] spawnableEnemies;

    public Transform enemySpawn;

    public Enemy currentEnemy;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    void Start()
    {
        SetUpBattle();
    }

    public void PlayerVictory()
    {
        combatState = CombatState.EnemyDead;
        Debug.Log("Victory!");

        Destroy(currentEnemy.gameObject);
        currentEnemy = null;

        StartCoroutine(SpawnEnemy());
    }

    public void PlayerDefeat()
    {
        combatState = CombatState.PlayerDead;
        Debug.Log("Defeat!");

        // TEMP: Remove after prototype
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // get a random index for enemyPrefabs using a list of weights for weighted probablity
    Enemy GetWeightedRandomEnemy()
    {
        int weightTotal = 0;
        int numWeights = spawnableEnemies.Length;
        for (int i = 0; i < numWeights; i++)
        {
            weightTotal += spawnableEnemies[i].spawnWeight;
        }

        int randValue = Random.Range(0, weightTotal);
        int total = 0;
        int result = 0;
        for (result = 0; result < numWeights; result++)
        {
            total += spawnableEnemies[result].spawnWeight;
            if (total > randValue) break;
        }

        return spawnableEnemies[result].enemyPrefab;
    }

    public IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(0.6f);

        //Enemy's center is at spawn's center
        Enemy spawnedEnemy = Instantiate(GetWeightedRandomEnemy());
        spawnedEnemy.transform.position = enemySpawn.transform.position;

        currentEnemy = spawnedEnemy;

        yield return new WaitForSeconds(0.2f);

        combatState = CombatState.PlayerTurn;
    }

    // spawn enemy
    public void SetUpBattle()
    {
        gameState = GameState.InCombat;
        combatState = CombatState.PreBattle;

        StartCoroutine(SpawnEnemy());
    }

    public void EndPlayerTurn()
    {
        if (currentEnemy.currentHP <= 0)
        {
            PlayerVictory();
        }
        else
        {
            combatState = CombatState.EnemyTurn;
            StartCoroutine(currentEnemy.Attack());
        }
    }

    public void EndEnemyTurn()
    {
        if (Player.instance.health <= 0)
        {
            PlayerDefeat();
        }
        else
        {
            combatState = CombatState.PlayerTurn;
        }
    }
}
