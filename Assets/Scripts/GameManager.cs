using UnityEngine;

public enum GameState
{
    TitleScreen,
    InMap,
    InCombat,
    AtTree,
    PlayerDead,
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

    // spawn enemy
    public void SetUpBattle()
    {
        //Enemy's center is at spawn's center
        Enemy spawnedEnemy = Instantiate(GetWeightedRandomEnemy());
        spawnedEnemy.transform.position = enemySpawn.transform.position;

        currentEnemy = spawnedEnemy;
    }
}
