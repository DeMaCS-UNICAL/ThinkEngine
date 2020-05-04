using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();    
    }

    public void startGame() {
        StartCoroutine(loadGameSceneAsnyc());
    }

    private IEnumerator loadGameSceneAsnyc() {
        AsyncOperation async = SceneManager.LoadSceneAsync("Game");
        yield return async;
    }
}
