using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour {

    Board m_board;

    public int armorID;

    public bool isActive = false;

    public bool brokenSword = false;

    protected Node m_currentNode;

    public Node CurrentNode { get { return m_currentNode; } }



    private void Awake() {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }

    private void Start() {
        m_currentNode = m_board.FindNodeAt(transform.position);
    }

    public int GetID() {
        return armorID;
    }

    public void ActivateSword() {
        if (!brokenSword) {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void DeactivateSword() {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void DestroySword() {
        brokenSword = true;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public Node FindArmorNode() {
        return m_board.FindNodeAt(transform.position);
    }

   

}
