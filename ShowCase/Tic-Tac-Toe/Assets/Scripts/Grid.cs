using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    public GridSpace[, ] grid = new GridSpace[3, 3];

    void Awake() {
        GameObject[] gridSpaceObjects = GameObject.FindGameObjectsWithTag("GridSpace");
        for (int i = 0; i < 9; i++) {
            GridSpace gridSpace = gridSpaceObjects[i].GetComponent<GridSpace>();
            grid[gridSpace.x, gridSpace.y] = gridSpace;
        }
    }

}