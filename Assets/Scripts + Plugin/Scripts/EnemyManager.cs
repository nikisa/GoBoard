﻿using System.Collections;
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

    PlayerManager m_player;

    

    bool m_isDead = false;
    public bool isScared = false;

    public bool isDead { get { return m_isDead; } }

    public UnityEvent deathEvent;

    protected override void Awake() {

        base.Awake();

        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        m_enemyMover = GetComponent<EnemyMover>();
        m_enemySensor = GetComponent<EnemySensor>();
        m_enemyDeath = GetComponent<EnemyDeath>();
        m_player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
    }

    public void PlayTurn() {
        
        if (m_isDead || isScared) {
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

            if (m_enemySensor.FoundPlayer && isScared == false) {
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


    public void PushLeft() {
        if (m_player.hasLightBulb) {
            
            Node EnemyNode = m_board.FindNodeAt(transform.position);

            if (m_board.playerNode.transform.position.z == EnemyNode.transform.position.z && Vector3.Distance(EnemyNode.transform.position, m_board.playerNode.transform.position) <= 2f && m_board.playerNode.transform.position.x > EnemyNode.transform.position.x) {
                Debug.Log("MoveRight");
                m_enemyMover.MoveLeft();
            }
        }
    }

    public void PushRight() {
        if (m_player.hasLightBulb) {

            Node EnemyNode = m_board.FindNodeAt(transform.position);

            if (m_board.playerNode.transform.position.z == EnemyNode.transform.position.z && Vector3.Distance(EnemyNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.x < EnemyNode.transform.position.x) {
                Debug.Log("MoveRight");
                m_enemyMover.MoveRight();
            }
        }
    }


    //PUSH UP


    //PUSH DOWN


    public ItemData GetData() {
        ItemData itemData = new ItemData() {
            BoardPosition = transform.position,
            ItemType = ItemData.Type.Enemy,
        };
        return itemData;
    }
}
