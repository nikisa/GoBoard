using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(EnemySensor))]
public class EnemyManager : TurnManager {

    EnemyMover m_enemyMover;
    EnemySensor m_enemySensor;
    EnemyDeath m_enemyDeath;
    Board m_board;

    

    bool m_isDead = false;
    public bool isDead { get { return m_isDead; } }

    public UnityEvent deathEvent;

    protected override void Awake() {

        base.Awake();

        m_board = GetComponent<Board>();
        m_enemyMover = GetComponent<EnemyMover>();
        m_enemySensor = GetComponent<EnemySensor>();
        m_enemyDeath = GetComponent<EnemyDeath>();
    }

    public void PlayTurn() {
        if (m_isDead) {
            FinishTurn();
            return;
        }
        StartCoroutine(PlayTurnRoutine());
    }

    IEnumerator PlayTurnRoutine() {

        if (m_gameManager != null && !m_gameManager.IsGameOver) {
            //detect player
            m_enemySensor.UpdateSensor(m_enemyMover.CurrentNode);

            yield return new WaitForSeconds(0f);

            if (m_enemySensor.FoundPlayer) {
                //attack player
                //notify the GM to lose the level
                m_gameManager.LoseLevel();


            }
            else {
                // movement
                m_enemyMover.MoveOneTurn(); // --> finishMovementEvent.Invoke()
            }
            
        }
        
    }

    public void Die() {
        if (m_isDead) {
            return;
        }
        m_isDead = true;
        

        if (deathEvent != null) {
            deathEvent.Invoke();
        }
    }

}
