using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : Mover {

    public PlayerManager playerManager;


    public void PushRight() {
        Node movableObjectNode = m_board.FindNodeAt(transform.position);

        if (m_board.playerNode.transform.position.z == movableObjectNode.transform.position.z && Vector3.Distance(movableObjectNode.transform.position , m_board.playerNode.transform.position)<3f && m_board.playerNode.transform.position.x < movableObjectNode.transform.position.x) {
            Debug.Log("MoveRight");
            this.MoveRight();
        }    
    }

    public void PushLeft() {
        Node movableObjectNode = m_board.FindNodeAt(transform.position);

        if (m_board.playerNode.transform.position.z == movableObjectNode.transform.position.z && Vector3.Distance(movableObjectNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.x > movableObjectNode.transform.position.x) {
            Debug.Log("MoveRight");
            this.MoveLeft();
        }
    }

    public void PushForward() {
        Node movableObjectNode = m_board.FindNodeAt(transform.position);

        if (m_board.playerNode.transform.position.x == movableObjectNode.transform.position.x && Vector3.Distance(movableObjectNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.z < movableObjectNode.transform.position.z) {
            Debug.Log("MoveRight");
            this.MoveForward();
        }
    }

    public void PushBackward() {
        Node movableObjectNode = m_board.FindNodeAt(transform.position);

        if (m_board.playerNode.transform.position.x == movableObjectNode.transform.position.x && Vector3.Distance(movableObjectNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.z > movableObjectNode.transform.position.z) {
            Debug.Log("MoveRight");
            this.MoveBackward();
        }
    }


    public void Pull() {

    }
}
