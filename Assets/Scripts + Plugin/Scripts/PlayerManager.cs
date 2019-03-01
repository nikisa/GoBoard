using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerManager : TurnManager {

    public PlayerMover playerMover;
    public PlayerInput playerInput;

    Board m_board;
    GameManager m_gm;
    
    protected override void Awake() {
        base.Awake();
        
        playerMover = GetComponent<PlayerMover>();
        playerInput = GetComponent<PlayerInput>();

        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        m_gm = Object.FindObjectOfType<GameManager>().GetComponent<GameManager>();
        
    }

    void Update() {
        if (playerMover.isMoving || m_gameManager.CurrentTurn != Turn.Player) {
            return;
        }

        playerInput.GetKeyInput();

        if (m_board.playerNode.isASwitch && playerInput.S) {
            Debug.Log("S");
            bool switchState = m_board.playerNode.GetSwitchState();
            if (switchState) {
                m_board.playerNode.UpdateSwitchToFalse();
            }
            else {
                m_board.playerNode.UpdateSwitchToTrue();
            }
        }


        if (playerInput.V == 0) {
            if (playerInput.H < 0) {
                if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count == 0) { //Aggiunto AND per evtiare di entrare nei MO facendo la pull verso di essi
                    playerMover.MoveLeft();
                    foreach (var movableObject in m_gm.GetMovableObjects()) {
                        movableObject.PullLeft();
                    }
                }
                else {
                    if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f,0,0))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0)))[0].leftBlocked) { //Se alla nostra Sx c'è un M.O e non è bloccato
                        
                        foreach (var movableObject in m_gm.GetMovableObjects()) {
                            movableObject.PushLeft();
                        }
                        playerMover.MoveLeft();

                    }
                    else if(m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(-2f, 0, 0))).Count == 0) { //Se non c'è nulla muovi solo il pg
                        playerMover.MoveLeft();
                    }
                }
                //END LEFT
            }
            else if (playerInput.H > 0) {
                if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0) {
                    playerMover.MoveRight();
                    foreach (var movableObject in m_gm.GetMovableObjects()) {
                        movableObject.PullRight();
                    }
                }
                else {
                    if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0)))[0].rightBlocked) { //Se alla nostra Dx c'è un M.O e non è bloccato
                        playerMover.MoveRight();
                        foreach (var movableObject in m_gm.GetMovableObjects()) {
                            movableObject.PushRight();
                        }
                    }
                    else if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(2f, 0, 0))).Count == 0) { //Se non c'è nulla muovi solo il pg
                        playerMover.MoveRight();
                    }
                }
                
            }

        }
        else if (playerInput.H == 0) {
            if (playerInput.V < 0) {
                if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0) {
                    playerMover.MoveBackward();
                    foreach (var movableObject in m_gm.GetMovableObjects()) {
                        movableObject.PullBackward();
                    }
                }
                else {
                    if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f)))[0].downBlocked) { //Se sotto non c'è un M.O e non è bloccato
                        playerMover.MoveBackward();
                        foreach (var movableObject in m_gm.GetMovableObjects()) {
                            movableObject.PushBackward();
                        }
                    }
                    else if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, -2f))).Count == 0) { //Se non c'è nulla muovi solo il pg
                        playerMover.MoveBackward();
                    }
                }
            }

            else if (playerInput.V > 0) {
                if (playerInput.P && m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count == 0) {
                    playerMover.MoveForward();
                    foreach (var movableObject in m_gm.GetMovableObjects()) {
                        movableObject.PullForward();
                    }
                }
                else {
                    if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count != 0 && !m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f)))[0].upBlocked) { //Se sopra non c'è un M.O e non è bloccato
                        playerMover.MoveForward();
                        foreach (var movableObject in m_gm.GetMovableObjects()) {
                            movableObject.PushForward();
                        }
                    }
                    else if (m_board.FindMovableObjectsAt(m_board.FindNodeAt(m_board.playerNode.transform.position + new Vector3(0, 0, 2f))).Count == 0) { //Se non c'è nulla muovi solo il pg
                        playerMover.MoveForward();
                    }
                }
            }
        }
    }

    void CaptureEnemies() {
        if (m_board != null) {
            List<EnemyManager> enemies = m_board.FindEnemiesAt(m_board.playerNode);
            if (enemies.Count != 0) {
                foreach (EnemyManager enemy in enemies) {
                    if (enemy != null) {
                        enemy.Die();
                    }
                }
            }
        }
    }

    public override void FinishTurn() {
        CaptureEnemies();
        base.FinishTurn();
    }

    
}