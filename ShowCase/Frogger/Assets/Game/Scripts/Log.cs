using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public bool moveRight = true;

    private readonly float playAreaWidth = 19.0f;
    // Update is called once per frame
    void Update()
    {

        Vector2 pos = transform.localPosition;

        if (moveRight)
        {
            pos.x += moveSpeed * Time.deltaTime;

            if (pos.x >= ((playAreaWidth / 2) - 1) + (playAreaWidth - 1) - GetComponent<SpriteRenderer>().size.x / 2)
            {
                pos.x = -playAreaWidth / 2 - GetComponent<SpriteRenderer>().size.x / 2;
            }
        }

        else
        {
            pos.x -= moveSpeed * Time.deltaTime;

            if(pos.x <= ((-playAreaWidth / 2) + 1) - (playAreaWidth - 1) + GetComponent<SpriteRenderer>().size.x / 2)
            {
                pos.x = playAreaWidth / 2 + GetComponent<SpriteRenderer>().size.x / 2;

            }
        }
       

        transform.localPosition = pos;
    }
}
