using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

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
    }
}
