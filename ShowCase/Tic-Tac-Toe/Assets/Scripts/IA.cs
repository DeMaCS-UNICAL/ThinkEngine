using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IA : MonoBehaviour {
    public int x, y;
    public int previousX, previousY;

    private GameObject[, ] gridSpaceGameObjects;
    private GameController gameController;

    void Awake() {
        x = y = previousX = previousY = -1;
        gameController = GameObject.FindObjectOfType<GameController>();
    }

    void Start() {
        gridSpaceGameObjects = new GameObject[3, 3];
        GameObject[] unordered = GameObject.FindGameObjectsWithTag("GridSpace");
        for (int i = 0; i < 9; i++) {
            var gridSpace = unordered[i];
            int x = gridSpace.GetComponent<GridSpace>().x;
            int y = gridSpace.GetComponent<GridSpace>().y;
            gridSpaceGameObjects[x, y] = gridSpace;
        }
    }

    void Update() {
        if (gameController.HumanHasToPlay()) {
            return;
        }
        Play();
    }

    void Play() {
        if (x != previousX || y != previousY) {
            previousX = x;
            previousY = y;
            gridSpaceGameObjects[x, y].GetComponent<GridSpace>().SetSpace();
        }
    }
}