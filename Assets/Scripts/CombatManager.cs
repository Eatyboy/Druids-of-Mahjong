using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatState
{
    PreBattle,
    PlayerTurn,
    EnemyTurn,
    TurnResolution,
    Defeat,
    Victory,
}

/// <summary>
/// Represents any combat action. Enemies and the player should enqueue ICombatActions instead of manually executing routines.
/// </summary>
public interface ICombatAction
{
    IEnumerator Execute(); 
}

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    public GameState gameState;
    public CombatState combatState;
    public Queue<ICombatAction> actionQueue = new();
    public bool isPlayerTurnNext;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    public void StartCombat()
    {
        gameState = GameState.InCombat;
        combatState = CombatState.PreBattle;

        StartCoroutine(CombatLoop());
    }

    private IEnumerator CombatLoop()
    {
        while (combatState != CombatState.Victory && combatState != CombatState.Defeat)
        {
            switch (combatState)
            {
                case CombatState.PreBattle:
                    yield return EnemyManager.instance.SpawnEnemy();

                    combatState = CombatState.PlayerTurn;
                    break;

                case CombatState.PlayerTurn:
                    yield return PlayerHand.instance.DrawUntilFullHand();
                    PlayerHand.instance.isTurnActive = true;
                    yield return new WaitUntil(() => !PlayerHand.instance.isTurnActive);

                    isPlayerTurnNext = false;
                    combatState = CombatState.TurnResolution;
                    break;

                case CombatState.EnemyTurn:
                    EnemyManager.instance.currentEnemy.MakeAttackDecision();

                    isPlayerTurnNext = true;
                    combatState = CombatState.TurnResolution;
                    break;

                case CombatState.TurnResolution:
                    while (actionQueue.Count > 0)
                    {
                        yield return actionQueue.Dequeue().Execute();
                    }

                    if (Player.instance.health <= 0) combatState = CombatState.Defeat;
                    else if (EnemyManager.instance.currentEnemy.currentHP <= 0) combatState = CombatState.Victory;
                    else if (isPlayerTurnNext) combatState = CombatState.PlayerTurn;
                    else combatState = CombatState.EnemyTurn;
                    break;
                        
                default:
                    Debug.LogError("Invalid combat state " + combatState);
                    break;
            }
        }

        if (combatState == CombatState.Victory) PlayerVictory();
        else if (combatState == CombatState.Defeat) PlayerDefeat();
    }


    public void PlayerVictory()
    {
        Destroy(EnemyManager.instance.currentEnemy.gameObject);
        EnemyManager.instance.currentEnemy = null;

        // TEMP: Remove later
        Debug.Log("Victory!");
    }

    public void PlayerDefeat()
    {
        combatState = CombatState.Defeat;
        
        Debug.Log("Defeat!");
    }
}
