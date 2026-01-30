using UnityEngine;

public class GameManager : MonoBehaviour
{
    // list of possible enemies that can spawn
    public GameObject[] enemyPrefabs;
    // list of numbers that determines how likely enemy at same index is to spawn
    public int[] enemySpawnWeights;
    public GameObject enemyPrefab;

    public Transform enemySpawn;

    Enemy enemyUnit;

    void Start()
    {
        SetUpBattle();
    }

    // get an weighted random index for enemyPrefabs using a list of weights
    int GetWeightedRandom(int[] weights)
    {
        int weightTotal = 0;
        int numWeights = weights.Length;
        for (int i = 0; i < numWeights; i++)
        {
            weightTotal += weights[i];
        }

        int randValue = Random.Range(0, weightTotal);
        int total = 0;
        int result = 0;
        for (result = 0; result < numWeights; result++)
        {
            total += weights[result];
            if (total > randValue) break;
        }
        return result;
    }

    public void SetUpBattle()
    {
        //Enemy's center is at spawn's center
        int enemyIndex = GetWeightedRandom(enemySpawnWeights);
        GameObject enemyGO = Instantiate(enemyPrefabs[enemyIndex]);

        enemyUnit = enemyGO.GetComponent<Enemy>();
    }
}
