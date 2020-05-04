using System;
using UnityEngine;

public class TetrominoTile : MonoBehaviour {
    public ArenaTile position;
    private Arena arena;

    void Start() {
        arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<Arena>();
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(position == null) position = collision.gameObject.GetComponent<ArenaTile>();
    }

    public bool canFallDown() {
        if (position.y + 1 >= arena.maxTileY) return false;
        else if (!arena.tile[position.x, position.y + 1].empty) {
            if (arena.tile[position.x, position.y + 1].tile.transform.parent == transform.parent) return true; //Check if is locked by the same tetromino
            else return false;
        }

        return true;
    }

    public bool canTurn(Tetromino.TurnDirection dir) {
        if(dir == Tetromino.TurnDirection.LEFT) {
            if (position.x <= 0) return false;
            if (!arena.tile[position.x - 1, position.y].empty) {
                if (arena.tile[position.x - 1, position.y].tile.transform.parent == transform.parent) return true; //Check if is locked by the same tetromino
                else return false;
            }
        }
        else {
            if (position.x + 1 >= arena.maxTileX) return false;
            if (!arena.tile[position.x + 1, position.y].empty) {
                if (arena.tile[position.x + 1, position.y].tile.transform.parent == transform.parent) return true; //Check if is locked by the same tetromino
                else return false;
            }
        }

        return true;
    }

    public void endFalling() {
        position.lockTile(this);
        arena.addRowToCheck(position.y);
    }

    public void rotate(TetrominoRotationTile rot) {
        position.unlockTile();
        position = rot.tile;
        position.lockTile(this);
    }

    public void turn(Tetromino.TurnDirection dir, bool withPositionChange = false) {
        position.unlockTile();
        position = arena.tile[position.x + (int)dir, position.y];
        if (withPositionChange) transform.position = position.transform.position;
        position.lockTile(this);
    }

    public void fallDownOnce(bool withPositionChange = false) {
        position.unlockTile();
        position = arena.tile[position.x, position.y + 1];
        if (withPositionChange) transform.position = position.transform.position;
        position.lockTile(this);
    }

    public void fallDown() {
        while(position.y + 1 < arena.maxTileY && arena.tile[position.x, position.y + 1].empty) {
            fallDownOnce(true);
        }

        endFalling();
    }

    
}
