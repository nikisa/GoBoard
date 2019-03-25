using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MovementType {
    Stationary,
    Patrol,
    Spinner,
    Chaser
}

public class EnemyMover : Mover {
    
    public Vector3 directionToMove = new Vector3(0f , 0f , Board.spacing);

    public MovementType firstMovementType = MovementType.Stationary;
    public MovementType movementType = MovementType.Stationary;

    
    public float standTime = 1f;

	protected override void Awake() {
        base.Awake();
        faceDestination = true;
        movementType = firstMovementType;

    }

    protected override void Start(){
        base.Start();
        
    }

    public void MoveOneTurn() {
        
        switch (movementType) {
            case MovementType.Patrol:
                Patrol();
                break;
            case MovementType.Stationary:
                Stand();
                break;
            case MovementType.Spinner:
                Spin();
                break;
            case MovementType.Chaser:
                Chase();
                break;
        }
    }

    void Chase() {
        StartCoroutine(ChaseRoutine());
    }

    IEnumerator ChaseRoutine() {

        Vector3 startPos = new Vector3(m_currentNode.Coordinate.x, 0f, m_currentNode.Coordinate.y);

        Vector3 firstDest = startPos + transform.TransformVector(directionToMove);

        Vector3 spottedDest = startPos + transform.TransformVector(directionToMove * 2f);

        if (m_board.playerNode == m_board.FindNodeAt(spottedDest) && !m_player.spottedPlayer && m_player.hasLightBulb == false && m_board.FindNodeAt(firstDest).LinkedNodes.Contains(m_board.FindNodeAt(spottedDest))) {

            Debug.Log("Spotted!");
            
            m_board.PreviousPlayerNode = m_board.playerNode;
            Move(firstDest , 0f);
            m_player.spottedPlayer = true;

        }

        else if (m_player.spottedPlayer) {

            Debug.Log("Chasing...");
            Debug.Log(spottedDest);

            m_board.ChaserNewDest = m_board.PreviousPlayerNode;
            m_board.PreviousPlayerNode = m_board.playerNode;

            Move(m_board.ChaserNewDest.transform.position, 0f);
        }
        

        while (isMoving) {
            yield return null;
        }

        base.finishMovementEvent.Invoke();
    }



    
    void Patrol() {
        StartCoroutine(PatrolRoutine());
    }

    IEnumerator PatrolRoutine() {
        Vector3 startPos = new Vector3(m_currentNode.Coordinate.x, 0f, m_currentNode.Coordinate.y);

        //One space forward
        Vector3 newDest = startPos + transform.TransformVector(directionToMove);

        //Two space forward
        Vector3 nextDest = startPos + transform.TransformVector(directionToMove * 2f);
        
        Move(newDest, 0f);

        while (isMoving) {
           yield return null;
        }

        if (m_board != null) {
            Node newDestNode = m_board.FindNodeAt(newDest);
            Node nextDestNode = m_board.FindNodeAt(nextDest);

            if (nextDestNode == null || nextDestNode.LinkedNodes.Contains(nextDestNode) || m_board.FindMovableObjectsAt(nextDestNode).Count != 0 || (m_board.playerNode == nextDestNode && m_player.spottedPlayer)) {

                //SPOSTARE MOVIMENTO QUI DENTRO ALTRIMENTI SI BLOCCA NELL ANGOLINO E NON SI GIRA

                destination = startPos;
                FaceDestination();

                yield return new WaitForSeconds(rotateTime);
            }
        }
        base.finishMovementEvent.Invoke();
    }

    void Stand() {
        StartCoroutine(StandRoutine());
    }

    IEnumerator StandRoutine() {
        yield return new WaitForSeconds(standTime);
        base.finishMovementEvent.Invoke();
    }

    public void Spin() {
        StartCoroutine(SpinRoutine());
    }
    IEnumerator SpinRoutine() {
        Vector3 localForward = new Vector3(0f, 0f , Board.spacing);
        destination = transform.TransformVector(localForward * -1f) + transform.position;

        FaceDestination();

        yield return new WaitForSeconds(rotateTime);

        base.finishMovementEvent.Invoke();
    }

}
