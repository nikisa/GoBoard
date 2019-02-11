using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    Board m_board;
    PlayerManager m_player;

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

    private void Awake() {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        m_player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
        
    }

    void Start () {
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
        if(setupEvent != null) {
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

    IEnumerator EndLevelRoutine() {

        Debug.Log("END LEVEL");

        m_player.playerInput.InputEnabled = false;

        if (endLevelEvent!=null) {
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
        if(m_board.playerNode != null) {
            return (m_board.playerNode == m_board.GoalNode);
        }
        return false;
    }

}
