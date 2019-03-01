using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public enum Turn {
    Player,
    Enemy
}

public class GameManager : MonoBehaviour {

    Board m_board;
    PlayerManager m_player;

    EnemySensor m_enemySensor;
    EnemyMover m_enemyMover;



    List<EnemyManager> m_enemies;
    List<MovableObject> m_movableObjects;

    Turn m_currentTurn = Turn.Player;
    public Turn CurrentTurn { get { return m_currentTurn; } }

    bool m_hasLevelStarted = false;
    public bool HasLevelStarted { get { return m_hasLevelStarted; } set { m_hasLevelStarted = value; } }

    bool m_isGamePlaying = false;
    public bool IsGamePlaying { get { return m_isGamePlaying; } set { m_isGamePlaying = value; } }

    bool m_isGameOver = false;
    public bool IsGameOver { get { return m_isGameOver; } set { m_isGameOver = value; } }

    bool m_hasLevelFinished = false;
    public bool HasLevelFinished { get { return m_hasLevelFinished; } set { m_hasLevelFinished = value; } }

    public float delay = 1f;


    public UnityEvent setupEvent;
    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;
    public UnityEvent loseLevelEvent;

    private void Awake() {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        m_player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();


        EnemyManager[] enemies = GameObject.FindObjectsOfType<EnemyManager>() as EnemyManager[];
        m_enemies = enemies.ToList();

        MovableObject[] movableObjects = GameObject.FindObjectsOfType<MovableObject>() as MovableObject[];
        m_movableObjects = movableObjects.ToList();

    }

    void Start() {
        if (m_player != null && m_board != null) {
            StartCoroutine("RunGameLoop");
        }
        else {
            Debug.LogWarning("GameManager ERROR: no player or board found");
        }
    }

    IEnumerator RunGameLoop() {
        yield return StartCoroutine("StartLevelRoutine");
        yield return StartCoroutine("PlayLevelRoutine");
        yield return StartCoroutine("EndLevelRoutine");
    }

    IEnumerator StartLevelRoutine() {

        Debug.Log("SETUP LEVEL");
        if (setupEvent != null) {
            setupEvent.Invoke();
        }

        Debug.Log("START LEVEL");

        m_player.playerInput.InputEnabled = false;
        while (!m_hasLevelStarted) {

            //todo: start screen
            //todo: when a button is pressed , start
            //todo: HasLevelStarted = true
            yield return null;
        }

        if (startLevelEvent != null) {
            startLevelEvent.Invoke();
        }

    }

    IEnumerator PlayLevelRoutine() {

        Debug.Log("PLAY LEVEL");

        m_isGamePlaying = true;
        yield return new WaitForSeconds(delay);
        m_player.playerInput.InputEnabled = true;

        if (playLevelEvent != null) {
            playLevelEvent.Invoke();
        }

        while (!m_isGameOver) {

            yield return null;
            //todo: Check win(end reached)/lose(player dead)

            m_isGameOver = IsWinner();


        }

        Debug.Log("U got what I call ... swag!");
    }

    public void LoseLevel() {
        StartCoroutine(LoseLevelRoutine());
    }

    IEnumerator LoseLevelRoutine() {
        m_isGameOver = true;

        if (loseLevelEvent != null) {
            loseLevelEvent.Invoke();
        }

        yield return new WaitForSeconds(2f);

        Debug.Log("Your swag has been turned off , m8");

        RestartLevel();

    }

    IEnumerator EndLevelRoutine() {

        Debug.Log("END LEVEL");

        m_player.playerInput.InputEnabled = false;

        if (endLevelEvent != null) {
            endLevelEvent.Invoke();
        }

        //todo: changing scene?

        while (!m_hasLevelFinished) {

            //todo: user confirm button --> HasLevelFinished = true
            yield return null;
        }

        RestartLevel();

    }

    void RestartLevel() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void PlayLevel() {
        m_hasLevelStarted = true;
    }

    bool IsWinner() {
        if (m_board.playerNode != null) {
            return (m_board.playerNode == m_board.GoalNode);
        }
        return false;
    }

    void PlayPlayerTurn() {
        m_currentTurn = Turn.Player;
        m_player.IsTurnComplete = false;
    }

    void PlayEnemyTurn() {
        m_currentTurn = Turn.Enemy;

        foreach (EnemyManager enemy in m_enemies) { //play each enemy's turn
            if (enemy != null && !enemy.isDead) {
                enemy.IsTurnComplete = false;
                enemy.PlayTurn();
            }
        }
    }


    bool IsEnemyTurnComplete() {
        foreach (EnemyManager enemy in m_enemies) {
            if (enemy.isDead) {
                continue;
            }
            if (!enemy.IsTurnComplete) {
                return false;
            }
        }
        return true;
    }


    bool AreEnemiesAllDead() {
        foreach (EnemyManager enemy in m_enemies) {
            if (!enemy.isDead) {
                return false;
            }
        }
        return true;
    }

    public void UpdateTurn() {

        triggerNode();
<<<<<<< HEAD
        checkNodeForObstacles();
        
=======
>>>>>>> 6c91b03535c4b98922e07880ceacdf772f1b60e2


        if (m_currentTurn == Turn.Player && m_player != null) {
            if (m_player.IsTurnComplete && !AreEnemiesAllDead()) {
                PlayEnemyTurn();
<<<<<<< HEAD
                m_movableObjects = GetMovableObjects();
            }
=======
            }
            
>>>>>>> 6c91b03535c4b98922e07880ceacdf772f1b60e2
        }
        
        else if (m_currentTurn == Turn.Enemy) {
            if (IsEnemyTurnComplete()) {
                PlayPlayerTurn();

                crackNode();
<<<<<<< HEAD

            }
=======
                
            }            
>>>>>>> 6c91b03535c4b98922e07880ceacdf772f1b60e2
        }
    }

    public void crackNode() {

        //______ENEMY ON CRACKNODE___________________

        List<EnemyManager> enemies;

        foreach (var node in m_board.CrackableNodes) {
            enemies = m_board.FindEnemiesAt(node);
            foreach (EnemyManager enemy in enemies) {
                node.UpdateCrackableState();
                node.UpdateCrackableTexture();
                //node.GetComponentInChildren<CrackableTexture>().UpdateCrackableTexture();

                if (node.GetCrackableState() == 0) {
                    enemy.Die();
                }
            }
        }

        //______ENEMY ON CRACKNODE___________________


        if (m_board.playerNode.isCrackable) {
            m_board.playerNode.UpdateCrackableState();
            m_board.playerNode.UpdateCrackableTexture();
        }

        if (m_board.playerNode.GetCrackableState() == 0) {
            loseLevelEvent.Invoke();
        }
    }

    public void triggerNode() {

        if (m_board.playerNode.isATrigger) {
            m_board.SetPreviousPlayerNode(m_board.playerNode);
            m_board.playerNode.UpdateTriggerToTrue();
        }
        else if (m_board.GetPreviousPlayerNode() != null) {
            m_board.UpdateTriggerToFalse();
        }

        List<EnemyManager> enemies;
        List<MovableObject> movableObjects;

        foreach (var node in m_board.TriggerNodes) {
            enemies = m_board.FindEnemiesAt(node);
            foreach (EnemyManager enemy in enemies) {



                if (enemy.GetEnemySensor.FindEnemyNode().isATrigger) {
                    enemy.GetEnemySensor.SetPreviousEnemyNode(enemy.GetEnemySensor.FindEnemyNode());
                    enemy.GetEnemySensor.FindEnemyNode().UpdateTriggerToTrue();
                    Debug.Log(enemy.GetEnemySensor.GetPreviousEnemyNode());
                }
                else if (enemy.GetEnemySensor.GetPreviousEnemyNode() != null) {
                    enemy.GetEnemySensor.GetPreviousEnemyNode().triggerState = false;
                    Debug.Log("yolo");
<<<<<<< HEAD
                }
            }

            movableObjects = m_board.FindMovableObjectsAt(node);
            foreach (MovableObject movableObject in movableObjects) {
                if (movableObject.FindMovableObjectNode().isATrigger) {
                    movableObject.SetPreviousMovableObjectNode(movableObject.FindMovableObjectNode());
                    movableObject.FindMovableObjectNode().UpdateTriggerToTrue();
                }
                else if (movableObject.GetPreviousMovableObjectNode() != null) {
                    movableObject.GetPreviousMovableObjectNode().triggerState = false;
=======
>>>>>>> 6c91b03535c4b98922e07880ceacdf772f1b60e2
                }
            }
        }
    }
<<<<<<< HEAD
    


    public List<MovableObject> GetMovableObjects() {
        Debug.Log("A");
        foreach (var movObj in m_board.AllMovableObjects) {
            Debug.Log(movObj);
            foreach (var node in m_board.playerNode.LinkedNodes) {
                Debug.Log(node);
                if (movObj.transform.position == node.transform.position) {
                    Debug.Log("ESKEREEE");
                    m_movableObjects.Add(movObj);
                    
                }
            }
        }
        return m_movableObjects;
    }


    public void checkNodeForObstacles() {
        foreach (var movableObject in m_movableObjects) {
            movableObject.checkNodeForObstacle();
        }
    }
}
=======
}
>>>>>>> 6c91b03535c4b98922e07880ceacdf772f1b60e2
