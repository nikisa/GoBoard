using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    Vector2 m_coordinate;
    public Vector2 Coordinate { get { return Utility.Vector2Round(m_coordinate); } }

    List<Node> m_neighborNodes = new List<Node>();
    public List<Node> NeighborNodes { get { return m_neighborNodes; } }

    List<Node> m_linkedNodes = new List<Node>();
    public List<Node> LinkedNodes { get { return m_linkedNodes; } }

    
    Board m_board;

    public GameObject geometry;

    public GameObject linkPrefab;

    public float scaleTime = 0.3f;
    public iTween.EaseType easeType = iTween.EaseType.easeInExpo;


    public float delay = 1f;

    bool m_isInitialized = false;

    public LayerMask obstacleLayer;

    public bool isLevelGoal = false;

    public bool isCrackable = false;

    public int crackableState = 2;

    public Sprite[] currentTexture = new Sprite[3];

    public bool isATrigger = false;

    public bool triggerState = false;

    public bool isASwitch = false;

    public bool switchState = false;

    public bool isAGate = false;

    public bool gateOpen = false;

    public int gateID = 0;

    private Vector3 m_nodePosition;

    public Sprite[] sprites;



    private void Awake() {
        m_board = Object.FindObjectOfType<Board>();
        m_coordinate = new Vector2(transform.position.x, transform.position.z);
        UpdateCrackableTexture();
        m_nodePosition = new Vector3(1000f, 1000f, 1000f);
    }

    // Use this for initialization
    void Start() {
        if (geometry != null) {
            geometry.transform.localScale = Vector3.zero;

            if (m_board != null) {
                m_neighborNodes = FindNeighbors(m_board.AllNodes);
            }
        }
    }

    public void ShowGeometry() {
        if (geometry != null) {
            iTween.ScaleTo(geometry, iTween.Hash(
                "time", scaleTime,
                "scale", Vector3.one,
                "easetype", easeType,
                "delay", delay
            ));
        }
    }

    public List<Node> FindNeighbors(List<Node> nodes) {

        List<Node> n_List = new List<Node>();

        foreach (Vector2 dir in Board.directions) {
            Node foundNeighbor = FindNeighborAt(nodes, dir);

            if (foundNeighbor != null && !n_List.Contains(foundNeighbor)) {
                n_List.Add(foundNeighbor);
            }
        }
        return n_List;
    }

    public Node FindNeighborAt(List<Node> nodes, Vector2 dir) {
        return nodes.Find(n => n.Coordinate == Coordinate + dir);
    }

    public Node FindNeighborAt(Vector2 dir) {
        return FindNeighborAt(NeighborNodes, dir);
    }

    public void InitNode() {

        if (!m_isInitialized) {
            ShowGeometry();
            InitNeighbors();
            m_isInitialized = true;
        }

    }

    void InitNeighbors() {
        StartCoroutine(InitNeighborsRoutine());
    }

    IEnumerator InitNeighborsRoutine() {
        yield return new WaitForSeconds(delay);

        foreach (Node n in m_neighborNodes) {

            if (!m_linkedNodes.Contains(n)) {

                Obstacle obstacle = FindObstacle(n);

                if (obstacle == null) {
                    LinkNode(n);
                    n.InitNode();
                }
            }
        }
    }

    void LinkNode(Node targetNode) {
        if (linkPrefab != null) {
            GameObject linkInstance = Instantiate(linkPrefab, transform.position, Quaternion.identity);
            linkInstance.transform.parent = transform;

            Link link = linkInstance.GetComponent<Link>();
            if (link != null) {
                link.DrawLink(transform.position, targetNode.transform.position);
            }

            if (!m_linkedNodes.Contains(targetNode)) {
                m_linkedNodes.Add(targetNode);
            }

            if (!targetNode.LinkedNodes.Contains(this)) {
                targetNode.LinkedNodes.Add(this);
            }
        }
    }

    Obstacle FindObstacle(Node targetNode) {
        Vector3 checkDirection = targetNode.transform.position - transform.position;
        RaycastHit raycastHit;

        if (Physics.Raycast(transform.position, checkDirection, out raycastHit, Board.spacing + 0.1f, obstacleLayer)) {
            //Debug.Log("Node FindObstacle: Ha colpito un ostacolo da " + this.name + " a " + targetNode.name);
            return raycastHit.collider.GetComponent<Obstacle>();
        }
        return null;
    }


    public int GetCrackableState() {
        return crackableState;
    }

    public void UpdateCrackableState() {
        this.crackableState--;
    }

    public void UpdateCrackableTexture() {
        if (this.isCrackable) {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sprites[crackableState];
        }
        else {
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public bool UpdateTriggerToTrue() {

        if (m_nodePosition != transform.position) {
            StaticCounter.triggerCounter++;
        }
        UpdateGateToOpen();

        return triggerState = true;
    }//UpdateTriggerToFalse --> in Board

    public bool UpdateTriggerToFalse() {
        return triggerState = false;
    }

    public bool GetSwitchState() {
        return switchState;
    }

    public bool UpdateSwitchToTrue() {
        if (m_nodePosition != transform.position) {
            StaticCounter.switchCounter++;
        }
        UpdateGateToOpen();
        return switchState = true;
    }

    public bool UpdateSwitchToFalse() {
        UpdateGateToClose();
        return switchState = false;
    }

    public bool GetGateState() {
        return gateOpen;
    }

    public int GetGateID() {
        return gateID;
    }


    public void SetGateOpen() {
        gateOpen = true;
    }

    public void SetGateClose() {
        gateOpen = false;
    }

    public bool UpdateGateToOpen() {
        gateOpen = false;

        foreach (var node in m_board.AllNodes) {
            if (node.GetGateID() == StaticCounter.switchCounter || node.GetGateID() == StaticCounter.triggerCounter) {
                node.SetGateOpen();
            }
        }
        return gateOpen;
    }

    public bool UpdateGateToClose() {
        gateOpen = true;

        foreach (var node in m_board.AllNodes) {
            if (node.GetGateID() == StaticCounter.switchCounter || node.GetGateID() == StaticCounter.triggerCounter) {
                node.SetGateClose();
            }
        }
        return gateOpen;
    }
}