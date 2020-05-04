using System.Collections;
using UnityEngine;

public class StartGameTimer : MonoBehaviour {
    [SerializeField] private int jumpsToStart = 4;
    [SerializeField] private float timeToJump = 1f;

    private CircleBar bar;
    private TetrominoSpawner spawner;

    void Start() {
        bar = GetComponentInChildren<CircleBar>();
        spawner = GameObject.FindGameObjectWithTag("TetrominoSpawner").GetComponent<TetrominoSpawner>();
        startCounter();
    }

    public void startCounter() {
        StartCoroutine(counterCoroutine());
    }

    private IEnumerator counterCoroutine() {
        for(int i = jumpsToStart; i > 0; --i) {
            yield return new WaitForSeconds(timeToJump);
            bar.raiseProgress(1f / jumpsToStart);
        }

        yield return new WaitForSeconds(timeToJump / 2f);
        startGame();
    }

    private void startGame() {
        spawner.spawn();
        gameObject.SetActive(false);
    }
}
