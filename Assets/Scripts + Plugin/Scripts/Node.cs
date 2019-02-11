using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    Vector2 m_coordinate;
    public Vector2 Coordinate { get { return Utility.Vector2Round(m_coordinate); } }

    List<Node> m_neighborNodes = new List<Node>();
    public List<Node> NeighborNodes {  get { return m_neighborNodes; } }

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

    private void Awake() {
        m_board = Object.FindObjectOfType<Board>();
        m_coordinate = new Vector2(transform.position.x, transform.position.z);
    }

    // Use this for initialization
    void Start () {
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
                "time",scaleTime,
                "scale", Vector3.one,
                "easetype", easeType,
                "delay" , delay
            ));
        }
    }

    public List<Node> FindNeighbors(List<Node> nodes) {

        List<Node> n_List = new List<Node>();

        foreach (Vector2 dir in Board.directions) {
            Node foundNeighbor = nodes.Find(n => n.Coordinate == Coordinate + dir);

            if (foundNeighbor != null && !n_List.Contains(foundNeighbor)) {
                n_List.Add(foundNeighbor);
            }
        }
        
        return n_List;
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

        foreach(Node n in m_neighborNodes) {

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
            if(link != null) {
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

        if (Physics.Raycast(transform.position, checkDirection , out raycastHit , Board.spacing + 0.1f , obstacleLayer)) {
            //Debug.Log("Node FindObstacle: Ha colpito un ostacolo da " + this.name + " a " + targetNode.name);
            return raycastHit.collider.GetComponent<Obstacle>();
        }
        return null;
    }
}
