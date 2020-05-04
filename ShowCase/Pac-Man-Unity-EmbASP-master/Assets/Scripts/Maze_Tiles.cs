using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Maze_Tiles : MonoBehaviour
{
    public MyTile[,] tiles;
    internal List<TileManager.Tile> Tiles { get; set; }
    private void Awake()
    {
        //Debug.Log("Maze awaken");
        /*Tiles = new List<TileManager.Tile>();
        Tiles = GameObject.Find("Game Manager").GetComponent<TileManager>().tiles;
        int x = 28;//transform.GetChild(0).childCount;
        int y = 31; //transform.childCount;
        tiles = new MyTile[x, y];
        for(int i=0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                tiles[i, j] = new MyTile(i,j);
                
            }
        }*/

    }
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Maze started");
        Tiles = new List<TileManager.Tile>();
        Tiles = GameObject.Find("Game Manager").GetComponent<TileManager>().tiles;
        int x = 29;//transform.GetChild(0).childCount;
        int y = 32; //transform.childCount;
        tiles = new MyTile[x, y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                tiles[i, j] = new MyTile(i, j);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        for (int j = 0; j < transform.childCount; j++)
        {
            for (int i = 0; i < transform.GetChild(j).childCount; i++)
            {
                Transform p = transform.GetChild(j).GetChild(i);
                Vector3 currentPos = p.position;
                //Debug.Log(currentPos.x + " " + currentPos.y);
                //if (p != null)
                //{
                    //Debug.Log("pacdot in " + ((int)currentPos.x - 1)+" "+ ((int)currentPos.y - 1));
                    tiles[(int)currentPos.x, (int)currentPos.y].pacdot = true;
                /*}
                else
                {
                    //Debug.Log("NOT pacdot in " + ((int)currentPos.x - 1)+" "+ ((int)currentPos.y - 1));
                    tiles[(int)currentPos.x, (int)currentPos.y].pacdot = false;
                }*/
            }
        }
        foreach (TileManager.Tile p in Tiles)
        {

            if (p.occupied)
            {
               //Debug.Log(p.x + " " + p.y+" occupied");
               // Debug.Log(tiles.GetLength(0) + " " + tiles.GetLength(1));
                tiles[p.x, p.y].occupied = true;
            }
            else
            {
               // Debug.Log(p.y + " " + p.x + " free");
                tiles[p.x, p.y].occupied = false;
            }
        }
    }
}
