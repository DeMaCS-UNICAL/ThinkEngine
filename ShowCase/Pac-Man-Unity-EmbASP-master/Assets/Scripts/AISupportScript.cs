
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AISupportScript : MonoBehaviour
{
    
    internal GameObject Pacman { get; set; }
    internal string PreviousMove { get; set; }
    internal Vector3 PreviousPos { get; set; }
    internal TileManager manager;
    internal List<TileManager.Tile> Tiles;
    internal List<Distance> distances;
    internal List<MyTile> neededTiles;
    internal int closestPelletX;
    internal int closestPelletY;
    internal int distanceClosestPellet;
    internal bool powerup;
    internal Maze_Tiles maze;
    //internal List<string> movesAvailable;
    int cont = 0;
    //internal Distance[][] pacman_distances;

    private static List<Distance>[,] distances_10;
    private static List<Distance>[,] distances_5;

    void Awake()
    {
        //Debug.Log("Support awaken");
        /*manager = GameObject.Find("Game Manager").GetComponent<TileManager>();
        Pacman = GameObject.Find("pacman");
        PreviousPos = new Vector3(0, 0);
        Tiles = new List<TileManager.Tile>();
        Tiles = manager.tiles;*/
    }
    void Start()
    {
        //Debug.Log("Support  started");
        manager = GameObject.Find("Game Manager").GetComponent<TileManager>();
        Pacman = GameObject.Find("pacman");
        PreviousPos = new Vector3(0, 0);
        Tiles = new List<TileManager.Tile>();
        Tiles = manager.tiles;
        // distances_5 = new Distance[28][][][];
        //distances_10 = new Distance[28][][][];
        distances_5 = new List<Distance>[28, 32];
        distances_10 = new List<Distance>[28, 32];
        distances = new List<Distance>();
        neededTiles = new List<MyTile>();
        maze = GameObject.Find("Pacdots").GetComponent<Maze_Tiles>();
        //movesAvailable = new List<string>();
        for (int i = 0; i < 28; i++)
            for (int j = 0; j < 32; j++)
            {
                distances_5[i, j] = new List<Distance>();
                distances_10[i, j] = new List<Distance>();
            }
        ReadFacts(10);
        ReadFacts(5);
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Tiles);
        //movesAvailable = new List<string>();
        Vector3 currentPos = new Vector3(Pacman.transform.position.x + 0.499f, Pacman.transform.position.y + 0.499f);
        //Debug.Log(Tiles.Count);
        //Debug.Log(currentPos.x + " " + currentPos.y);
        //Debug.Log(manager.Index((int)currentPos.x, (int)currentPos.y));
            TileManager.Tile currentTile;
        neededTiles = new List<MyTile>();
        if (Tiles.Count > 0)
        {
            currentTile = Tiles[manager.Index((int)currentPos.x, (int)currentPos.y)];

            TileManager.Tile down = currentTile.down;
            TileManager.Tile up = currentTile.up;
            TileManager.Tile left = currentTile.left;
            TileManager.Tile right = currentTile.right;
            
            GameObject[] pacdots = GameObject.FindGameObjectsWithTag("pacdot");
            distanceClosestPellet = int.MaxValue;
            foreach (GameObject p in pacdots)
            {
                TileManager.Tile pacdotsTile = new TileManager.Tile((int)p.transform.position.x, (int)p.transform.position.y);
                var myDistance = manager.distance(currentTile, pacdotsTile);
                if (myDistance < distanceClosestPellet)
                {
                    distanceClosestPellet = (int)myDistance;
                    closestPelletX = pacdotsTile.x;
                    closestPelletY = pacdotsTile.y;
                }
            }
            powerup = GameManager.scared;
            distances = new List<Distance>();
            for (int i = -1; i <= 1; i++)
            {
                if (currentTile.x + i > 0 && currentTile.x + i < 28)
                {
                    neededTiles.Add(maze.tiles[currentTile.x + i, currentTile.y]);
                    //Debug.Log((currentTile.x + i)+" "+ currentTile.y+" "+maze.tiles[currentTile.x + i, currentTile.y].pacdot);
                   /* if (powerup)
                    {
                        distances.AddRange(distances_10[currentTile.x + i, currentTile.y]);
                    }
                    else
                    {*/
                        distances.AddRange(distances_5[currentTile.x + i, currentTile.y]);
                  //  }
                }
                if (currentTile.y + i > 0 && currentTile.y + i < 32)
                {
                    neededTiles.Add(maze.tiles[currentTile.x, currentTile.y+i]);
                    //Debug.Log(currentTile.x + " " + (currentTile.y+i) + " " + maze.tiles[currentTile.x, currentTile.y+i].pacdot);
                   /* if (powerup)
                    {
                        distances.AddRange(distances_10[currentTile.x, currentTile.y + i]);
                    }
                    else
                    {*/
                        distances.AddRange(distances_5[currentTile.x, currentTile.y + i]);
                   // }
                }
            }
        }
       
    }




    private void ReadFacts(int dimension)
    {
        
        string text = System.IO.File.ReadAllText(@".\encodings\min_distances_" + dimension + ".asp");
        string[] atoms = text.Split('.');
        int x1, x2, y1, y2, d;
        foreach(string a in atoms)
        {
            string[] st = a.Split('(');
            if (st.Length > 1)
            {
                st = st[1].Remove(st[1].Length - 1).Split(',');
                x1 = int.Parse(st[0]);
                y1 = int.Parse(st[1]);
                x2 = int.Parse(st[2]);
                y2 = int.Parse(st[3]);
                d = int.Parse(st[4]);

                if (dimension == 5)
                {
                    distances_5[x1, y1].Add(new Distance(x1, y1, x2, y2, d));
                }
                else
                {
                    distances_10[x1, y1].Add(new Distance(x1, y1, x2, y2, d));
                }
            }
        }
        Debug.Log("read distances_" + dimension);
    }
}
