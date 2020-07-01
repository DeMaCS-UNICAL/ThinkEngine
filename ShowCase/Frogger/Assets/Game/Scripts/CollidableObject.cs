using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableObject : MonoBehaviour
{

    public bool isSafe;
    public bool isLog;
    public bool isTurtle;
    public bool isHomeBay;
    public bool isOccupied = false;

    Rect playerRect;
    Vector2 playerSize;
    Vector2 playerPosition;

    public int leftMargin;
    public int rightMargin;
    public int y;
    public bool right;

    Rect collidableObjectRect;
    Vector2 collidableObjectSize;
    Vector2 collidableObjectPosition;

    private void Update()
    {
        leftMargin = (int)(transform.localPosition.x+ 8- collidableObjectSize.x/2);
        rightMargin = (int)(leftMargin+ collidableObjectSize.x);
        if (leftMargin < 0)
        {
            if (rightMargin >= 0)
            {
                leftMargin = 0;
            }
            else
            {
                leftMargin = 17;
                rightMargin = 17;
            }
        }
        y = (int)transform.localPosition.y+6;
        if (isLog)
        {
            right = GetComponent<Log>().moveRight;
        }
    }

    public bool IsColliding(GameObject playerGameObject)
    {
        playerSize = playerGameObject.transform.GetComponent<SpriteRenderer>().size;
        playerPosition = playerGameObject.transform.localPosition;

        collidableObjectSize = GetComponent<SpriteRenderer>().size;
        collidableObjectPosition = transform.localPosition;

        playerRect = new Rect(playerPosition.x - playerSize.x / 2, playerPosition.y - playerSize.y / 2, playerSize.x, playerSize.y);
        collidableObjectRect = new Rect(collidableObjectPosition.x - collidableObjectSize.x / 2, collidableObjectPosition.y - collidableObjectSize.y / 2, collidableObjectSize.x, collidableObjectSize.y);

        if(collidableObjectRect.Overlaps(playerRect,true))
        {
            return true;
        }

        return false;
    
    }
}
