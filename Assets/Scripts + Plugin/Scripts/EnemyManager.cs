using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(EnemySensor))]

public class EnemyManager : TurnManager {

    EnemyMover m_enemyMover;

    EnemySensor m_enemySensor;
    public EnemySensor GetEnemySensor { get { return m_enemySensor; } }

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

    //public void checkFirstNode()
    //{
    //    if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(transform.position + new Vector3(0, 0, 2f))).Count != 0)
    //    {
            
    //        m_enemyMover.Spin();
    //    }
    //}

        
    public void Die() {
        if (m_isDead) {
            return;
        }
        m_isDead = true;
        

        if (deathEvent != null) {
            deathEvent.Invoke();
        }
    }

    public void SetMovementType(MovementType mt)
    {
        m_enemyMover.movementType = mt; 
    }

    public MovementType GetMovementType()
    {
        return m_enemyMover.movementType;
    }

    public MovementType GetFirstMovementType()
    {
        return m_enemyMover.firstMovementType;
    }

    public ItemData GetData() {
        ItemData itemData = new ItemData() {
            BoardPosition = transform.position,
            ItemType = ItemData.Type.Enemy,
        };
        return itemData;
    }
}
