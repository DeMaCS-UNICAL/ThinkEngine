//Starting project and assets from tutorial: https://www.youtube.com/watch?v=hV_d4-FCQtI&list=PLiRrp7UEG13ZpFzUGeZ-4762FBar-ZfIA

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Sprite playerUp, playerDown, playerLeft, playerRight;

    public int lives = 4;

    private HUD hud;

    private Vector2 originalPosition;

    public float gameTime = 30;

    public float gameTimeWarning = 5;

    public float gameTimer;

    private int occupiedCount;

    public int move = 0;

    public int xpos = 0;
    public int ypos = 0;

    private int x = 0;
    private int y = 0;

    public bool deadAnimation = false;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.localPosition;

        hud = GameObject.Find("CanvasHUD").GetComponent<HUD>();
    }

    // Update is called once per frame
    void Update()
    {
        KeyboarMove();
        UpdatePosition();
        CheckCollisions();
        CheckGameTimer();

    }

    private void KeyboarMove()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            move = 1;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            move = 2;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            move = 3;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            move = 4;
        }
    }

    private void CheckGameTimer()
    {
        gameTimer += Time.deltaTime;

        Vector3 scale = hud.timeband.GetComponent<RectTransform>().localScale;

        scale.x = gameTime - gameTimer;

        hud.timeband.GetComponent<RectTransform>().localScale = scale;

        if(scale.x <= gameTimeWarning)
        {
            hud.timeband.color = Color.red;
        }
        else
        {
            hud.timeband.color = Color.black;
        }

        if (gameTimer >= gameTime)
        {

            PlayerDead();
            if (lives == -1)
            {
                GameOver();
            }
           
        }
    }

    private void UpdatePosition()
    {
        Vector2 pos = transform.localPosition;

        //if (Input.GetKeyDown(KeyCode.UpArrow))
        if (move == 1)
        {
            //Debug.Log("going up!");
            GetComponent<SpriteRenderer>().sprite = playerUp;
            pos += Vector2.up;
            move = 0;
        }
        //else if (Input.GetKeyDown(KeyCode.DownArrow))
        else if (move == 4)
        {
            GetComponent<SpriteRenderer>().sprite = playerDown;

            if (pos.y > -6)
            {
                pos += Vector2.down;
            }
            move = 0;

        }
        //else if (Input.GetKeyDown(KeyCode.LeftArrow))
        else if (move == 2)
        {
            GetComponent<SpriteRenderer>().sprite = playerLeft;

            if (pos.x > -8)
            {
                pos += Vector2.left;
            }
            move = 0;
        }
        //else if (Input.GetKeyDown(KeyCode.RightArrow))
        else if (move == 3)
        {
            GetComponent<SpriteRenderer>().sprite = playerRight;

            if (pos.x < 8)
            {
                pos += Vector2.right;
            }
            move = 0;
        }
        else if (move == 0)
        {
            move = 0;
        }
        transform.localPosition = pos;


        x = (int)pos.x;
        y = (int)pos.y;

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


    }

    private void CheckCollisions()
    {
        bool isSafe = true;

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("CollidableObject");

        foreach (GameObject go in gameObjects)
        {
            CollidableObject collidableObject = go.GetComponent<CollidableObject>();

            if (collidableObject.IsColliding(this.gameObject))
            {

                if (collidableObject.isSafe)
                {
                    isSafe = true;
                    if (collidableObject.isLog)
                    {
                        Vector2 pos = transform.localPosition;

                        if (collidableObject.GetComponent<Log>().moveRight)
                        {
                            pos.x += collidableObject.GetComponent<Log>().moveSpeed * Time.deltaTime;

                            if (transform.localPosition.x >= 9.5)
                            {
                                pos.x = transform.localPosition.x - 18f;
                            }
                        }
                        else
                        {
                            pos.x -= collidableObject.GetComponent<Log>().moveSpeed * Time.deltaTime;

                            if (transform.localPosition.x <= -9.5)
                            {
                                pos.x = transform.localPosition.x + 18f;
                            }
                        }

                        transform.localPosition = pos;
                    }
                    if (collidableObject.isHomeBay)
                    {
                        if(!collidableObject.isOccupied)
                        {
                            GameObject trophy = (GameObject)Instantiate(Resources.Load("Prefabs/trophy", typeof(GameObject)), collidableObject.transform.localPosition, Quaternion.identity);
                            trophy.name = "trophy" + occupiedCount.ToString();
                            collidableObject.isOccupied = true;
                            occupiedCount += 1;
                            gameTimer = 0;

                            if(occupiedCount == 5)
                            {
                                GameOver();
                            }

                        }

                        ResetPosition();
                    }
                    break;
                }
                else
                {
                    isSafe = false;
                }
            }

        }

        if (!isSafe)
        {
            deadAnimation = true;
            if (lives == 0)
            {
                GameOver();
            }
            else
            {
                PlayerDead();
            }
        }
    }

    void PlayerDead()
    {
        gameTimer = 0;
        lives -= 1;
        ResetPosition();
        deadAnimation = false;
    }

    void GameOver()
    {
        lives = 4;

        for (int i = 0; i < occupiedCount; i++)
        {
            string cur = i.ToString();
            Destroy(GameObject.Find("trophy" + cur));
        }

        for (int i = 1; i <= 5; i++)
        {
            string cur = i.ToString();
            GameObject go = GameObject.Find("homebay" + cur);
            CollidableObject collidableObject = go.GetComponent<CollidableObject>();
            collidableObject.isOccupied = false;
        }

        ResetPosition();
        occupiedCount = 0;
        deadAnimation = false;
    }

    void ResetPosition()
    {

        hud.UpdatePlayerLivesHUD(lives);
        transform.localPosition = originalPosition;
        transform.GetComponent<SpriteRenderer>().sprite = playerUp;

    }
}
