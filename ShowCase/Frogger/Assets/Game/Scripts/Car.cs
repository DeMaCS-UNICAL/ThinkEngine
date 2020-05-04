using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool moveRight = false;

    public int xpos = 0;
    public int ypos = 0;

    private int x;
    private int y;
  
    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.localPosition;

        x = (int)transform.localPosition.x;
        if (x + 8 < 0)
        {
            xpos = 0;
        }
        else if (x + 8 > 16)
        {
            xpos = 16;
        }
        else
        {
            xpos = x + 8;
        }

        y = (int)transform.localPosition.y;
        if (y + 6 < 0)
        {
            ypos = 0;
        }
        else if (y + 6 > 12)
        {
            ypos = 12;
        }
        else
        {
            ypos = y + 6;
        }

        if (moveRight)
        {
            pos.x += Vector2.right.x * moveSpeed * Time.deltaTime;
            if (pos.x >= 10)
            {
                pos.x = -10;
            }
        }
     

        else
        {
            //Left
            pos.x += Vector2.left.x * moveSpeed * Time.deltaTime;
            if (pos.x <= -10)
            {
                pos.x = 10;
            }
        }

       

        transform.localPosition = pos;
    }
}
