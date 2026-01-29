using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject enemyPrefabs;

    public Transform enemySpawn;

    Unit enemyUnit;

    void Start()
    {
        SetUpBattle();
    }

    public void SetUpBattle()
    {
        //Enemy's center is at spawn's center
        GameObject enemyGO = Instantiate(enemyPrefabs, enemySpawn);

        enemyUnit = enemyGO.GetComponent<Unit>();
    }
}
