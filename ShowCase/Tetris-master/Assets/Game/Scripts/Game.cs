using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {
    [SerializeField] private Game_components.Tetromino _tetromino;
    [SerializeField] private Game_components.Level _level;
    [SerializeField] private Game_components.Point _points;
    [SerializeField] private Game_components.PauseGame _pause;

    [SerializeField] private GameFinished gameFinishedScreen;

    public Game_components.Tetromino tetromino { get { return _tetromino; } }
    public Game_components.Level level { get { return _level; } }
    public Game_components.Point points { get { return _points; } }
    public Game_components.PauseGame pause { get { return _pause; } }

    void Start() {
        points.setPoints(0);
        level.bar.BarFinished += level.newLevel;
        level.bar.BarFinished += tetromino.newLevel;
        level.bar.BarFinished += points.newLevel;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) StartCoroutine(loadMainMenuSceneAsnyc());    
    }

    public void addPoints() {
        points.addPoints();
        level.rowRemoved();
    }

    public void setPause(bool val) { pause.paused = val; }

    public void finishGame() {
        gameFinishedScreen.display(points.points);
    }

    public void resetGame() {
        StartCoroutine(loadGameSceneAsnyc());
    }

    private IEnumerator loadGameSceneAsnyc() {
        AsyncOperation async = SceneManager.LoadSceneAsync("Game");
        yield return async;
    }

    private IEnumerator loadMainMenuSceneAsnyc() {
        AsyncOperation async = SceneManager.LoadSceneAsync("MainMenu");
        yield return async;
    }
    
}