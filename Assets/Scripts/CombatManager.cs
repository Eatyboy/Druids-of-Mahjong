using System;
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

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    [SerializeField] private RoundEndUI roundEndUI;

    public CombatState combatState;
    public Queue<(Func<IEnumerator> action, string actionName)> actionQueue = new();
    public bool isPlayerTurnNext;
    private int qiDroppedThisRound = 0;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    private void Start()
    {
        StartCombat();
    }

    public void StartCombat()
    {
        combatState = CombatState.PreBattle;
        qiDroppedThisRound = 0;

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
                    FlowerTileManager.instance.ActivateFlowerTilesOnTurnStart();
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
                        yield return actionQueue.Dequeue().action.Invoke();
                    }

                    if (GameManager.playerData.health <= 0) combatState = CombatState.Defeat;
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

    public void QiDropped(int amount)
    {
        qiDroppedThisRound += amount;
    }

    public void PlayerVictory()
    {
        Destroy(EnemyManager.instance.currentEnemy.gameObject);
        EnemyManager.instance.currentEnemy = null;

        roundEndUI.gameObject.SetActive(true);

        roundEndUI.Initialize(qiDroppedThisRound);
    }

    public void PlayerDefeat()
    {
        combatState = CombatState.Defeat;
        
        GameManager.instance.QuitToTitleScreen();
    }

    public void EnqueueAction(Func<IEnumerator> action, string actionName = null)
    {
        if (action == null)
        {
            Debug.LogError("Tried to enqueue a null action");
            return;
        }

        string finalActionName = actionName ?? Utils.GetReadableMethodName(action);
        actionQueue.Enqueue((action, finalActionName));
    }
}
