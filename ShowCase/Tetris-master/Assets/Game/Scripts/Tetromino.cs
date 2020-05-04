using Assets.Scripts;
using System.Collections;
using UnityEngine;

public class Tetromino : MonoBehaviour {
    public enum TurnDirection { LEFT = -1, RIGHT = 1 }

    [SerializeField]
    private bool rotation = true;
    private readonly float tileSize = 0.4096f;
    public AIPlayer ai;
    private TetrominoTile[] tetrominoTiles = new TetrominoTile[4];
    private TetrominoRotationTile[] rotationTiles;
    private Game game;
    private float fallingTime;
    private TetrominoSpawner spawner;
    private SwipeInput input;
    private Arena arena;

    void Start() {
        ai = GameObject.FindObjectOfType<AIPlayer>();
        rotationTiles = GetComponentsInChildren<TetrominoRotationTile>();
        arena = GameObject.FindObjectOfType<Arena>();
        for (int i = 1; i < transform.childCount; ++i) tetrominoTiles[i - 1] = transform.GetChild(i).GetComponent<TetrominoTile>();
        game = Camera.main.GetComponent<Game>();
        input = Camera.main.GetComponent<SwipeInput>();
        spawner = GameObject.FindGameObjectWithTag("TetrominoSpawner").GetComponent<TetrominoSpawner>();
        fallingTime = game.tetromino.fallTime;

        assignToInputEvents();
        StartCoroutine(fallingCoroutine());
    }

    private void assignToInputEvents() {
        input.SwipedUp += rotate;
        input.SwipedDown += boostFalling;
        input.SwipedLeft += turnLeft;
        input.SwipedRight += turnRight;
    }

    private void removeFromInputEvents() {
        input.SwipedUp -= rotate;
        input.SwipedDown -= boostFalling;
        input.SwipedLeft -= turnLeft;
        input.SwipedRight -= turnRight;
    }

    private void rotate() {
        //Debug.Log("can rotate? "+canRotate());
        if(rotation && canRotate()) {
            for (int i = 0; i < rotationTiles.Length; ++i) tetrominoTiles[i].rotate(rotationTiles[i]);
            transform.Rotate(0, 0, 90f);
            ai.moveDone = true;
        }
    }

	private bool canRotate() {
        foreach(var obj in rotationTiles) {
            if (!obj.canRotate) return false;
            else continue;
        }
        return true;
    }

    private IEnumerator fallingCoroutine() {
        bool falling = true;

        while(falling) {
            yield return new WaitForSeconds(fallingTime);

            if(!game.pause.paused) {
                foreach (var tile in tetrominoTiles) {
                    falling = tile.canFallDown();
                    if (!falling) break;
                }

                if (falling) {
                    foreach (var tile in tetrominoTiles) tile.fallDownOnce();

                    transform.position = new Vector3(transform.position.x, transform.position.y - tileSize, transform.position.z);
                }
            }
        }

        if(!falling) endFalling();
    }

    private void turnLeft() { turn(TurnDirection.LEFT); }
    private void turnRight() { turn(TurnDirection.RIGHT); }

    private void turn(TurnDirection dir) {
        bool canTurn = true;

        foreach(var tile in tetrominoTiles) {
            canTurn = tile.canTurn(dir);
            if (!canTurn) break;
        }

        if (canTurn) {
            foreach (var tile in tetrominoTiles) tile.turn(dir);

            if (dir == TurnDirection.LEFT) transform.position = new Vector3(transform.position.x - tileSize, transform.position.y, transform.position.z);
            else transform.position = new Vector3(transform.position.x + tileSize, transform.position.y, transform.position.z);
            ai.moveDone = true;
        }
    }

    private void boostFalling() {
        fallingTime = game.tetromino.fallTimeBoosted;
    }

    private void endFalling() {
        removeFromInputEvents();
        int maxRow = 0;
        for (int i = 0; i < tetrominoTiles.Length; i++)
        {
            tetrominoTiles[i].endFalling();
            if (tetrominoTiles[i].position.y > maxRow)
            {
                maxRow = tetrominoTiles[i].position.y;
            }
            
        }
        arena.checkRows(maxRow);
        spawner.spawn();
        Destroy(transform.GetChild(0).gameObject); //Rotation colliders
        Destroy(GetComponent<Rigidbody>());
        Destroy(this);
    }
}
