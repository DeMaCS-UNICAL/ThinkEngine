using UnityEngine;

public class TetrominoRotationTile : MonoBehaviour {
    private bool _canRotate = false;
    private ArenaTile _tile;

    public bool canRotate { get { return _canRotate; } private set { _canRotate = value; } }
    public ArenaTile tile { get { return _tile; } private set { _tile = value; } }

    void OnCollisionEnter2D(Collision2D collision) { checkRotation(collision); }
    void OnCollisionStay2D(Collision2D collision) { checkRotation(collision); }

    void OnCollisionExit2D(Collision2D collision) {
        canRotate = false;
        tile = null;
    }

    private void checkRotation(Collision2D collision) {
        tile = collision.gameObject.GetComponent<ArenaTile>();

        if (tile.empty) canRotate = true;
        else {
            //Debug.Log(tile.x + " " + tile.y + " is not empty");
            if (tile.tile.transform.parent == transform.parent.parent) canRotate = true;
            else {
                tile = null;
                canRotate = false;
            }
        }
    }
}
