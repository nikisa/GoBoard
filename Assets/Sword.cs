using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    Board m_board;

    protected Node m_currentNode;
    public Node CurrentNode { get { return m_currentNode; } }

    private void Awake() {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }

    private void Start() {
        m_currentNode = m_board.FindNodeAt(transform.position);
    }

    public Node FindSwordNode() {
        return m_board.FindNodeAt(transform.position);
    }


}
