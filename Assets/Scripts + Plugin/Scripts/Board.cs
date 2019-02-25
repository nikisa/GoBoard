using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour {

    public static float spacing = 2f;

    public static readonly Vector2[] directions = {
        new Vector2(spacing, 0f),
        new Vector2(-spacing , 0f),
        new Vector2(0f , spacing),
        new Vector2(-0f , -spacing)
    };

    //public enum consequence{nothing , lose , enemyDies , fallenObject};


    List<Node> m_allNodes = new List<Node>();
    public List<Node> AllNodes { get { return m_allNodes; } }

    List<Node> m_crackableNodes = new List<Node>();
    public List<Node> CrackableNodes { get { return m_crackableNodes; } }

    List<Node> m_triggerNodes = new List<Node>();
    public List<Node> TriggerNodes { get { return m_triggerNodes; } }

    List<MovableObject> m_AllmovableObjects = new List<MovableObject>();
    public List<MovableObject> AllMovableObjects { get { return m_AllmovableObjects; }}
    

    Node m_playerNode;

    public Node playerNode { get { return m_playerNode; } }

    Node m_goalNode;
    public Node GoalNode { get { return m_goalNode; } }

    Node m_previousPlayerNode;
    public Node PreviousPlayerNode { get { return m_previousPlayerNode; } set { m_previousPlayerNode = playerNode;} }

    
    public GameObject goalPrefab;
    public float drawGoalTime = 2f;
    public float drawGoalDelay = 2f;
    public iTween.EaseType drawGoalEaseType = iTween.EaseType.easeOutExpo;

    PlayerMover m_player;

    //---------------------------------------------------------------------
    //public List<Transform> capturePositions;
    //int m_currentCapturePosition = 0;

    //public int CurrentCapturePosition { get { return m_currentCapturePosition; } set { m_currentCapturePosition = value; } }

    //public float capturePositionIconSize = 0.4f;
    //public Color capturePositionIconColor = Color.blue;
    //---------------------------------------------------------------------

    void Awake() {
        m_player = Object.FindObjectOfType<PlayerMover>().GetComponent<PlayerMover>();

        m_AllmovableObjects = FindMovableObjects();
        
        GetNodeList();
        m_crackableNodes = FindCrackableNodes();
        m_goalNode = FindGoalNode();
        m_triggerNodes = FindTriggerNodes();
    }

    public void GetNodeList() {
        Node[] nList = GameObject.FindObjectsOfType<Node>();
        m_allNodes = new List<Node>(nList);
    }
    
    public Node FindNodeAt(Vector3 pos) {
        Vector2 boardCoord = Utility.Vector2Round(new Vector2(pos.x, pos.z));
        return m_allNodes.Find(n => n.Coordinate == boardCoord);
    }

    Node FindGoalNode() {
        return m_allNodes.Find(n => n.isLevelGoal);
    }

    public Node FindPlayerNode() {
        if (m_player != null && !m_player.isMoving) {
            return FindNodeAt(m_player.transform.position);
        }
        return null;
    }

    public List<Node> FindCrackableNodes() {
        foreach (var node in m_allNodes) {
            if (node.isCrackable) {
                CrackableNodes.Add(node);
            }
        }
        return CrackableNodes;
    }

    public List<Node> FindTriggerNodes() {
        foreach (var node in m_allNodes) {
            if (node.isATrigger) {
                TriggerNodes.Add(node);
            }
        }
        return TriggerNodes;
    }



    public List<EnemyManager> FindEnemiesAt(Node node) {
        List<EnemyManager> foundEnemies = new List<EnemyManager>();
        EnemyManager[] enemies = Object.FindObjectsOfType<EnemyManager>() as EnemyManager[];

        foreach (EnemyManager enemy in enemies) {
            EnemyMover mover = enemy.GetComponent<EnemyMover>();

            if (mover.CurrentNode == node) {
                foundEnemies.Add(enemy);
            }
        }
        return foundEnemies;
    }

    public void UpdatePlayerNode() {
        m_playerNode = FindPlayerNode();
    }


    public void SetPreviousPlayerNode(Node n) {
        PreviousPlayerNode = n; 
    }

    public Node GetPreviousPlayerNode() {
        return PreviousPlayerNode;
    }

    public void UpdateTriggerToFalse() {
        PreviousPlayerNode.triggerState = false;
    }

    
    public List<MovableObject> FindMovableObjects() {
        List<MovableObject> foundMovableObjects = new List<MovableObject>();
        MovableObject[] movableObjects = Object.FindObjectsOfType<MovableObject>() as MovableObject[];
        foundMovableObjects = movableObjects.ToList();

        return foundMovableObjects;
    }


    

    public void DrawGoal() {
        if (goalPrefab != null && m_goalNode != null) {
            GameObject goalInstance = Instantiate(goalPrefab, m_goalNode.transform.position, Quaternion.identity);

            iTween.ScaleFrom(goalInstance, iTween.Hash(
                "scale", Vector3.zero,
                "time", drawGoalTime,
                "delay", drawGoalDelay,
                "easetype", drawGoalEaseType));
        }
    }

    public void InitBoard() {
        if (m_player != null) {
            m_playerNode.InitNode();
        }
    }
}