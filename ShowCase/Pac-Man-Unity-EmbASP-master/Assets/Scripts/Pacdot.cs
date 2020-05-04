using UnityEngine;
using System.Collections;

public class Pacdot : MonoBehaviour {
    Maze_Tiles maze;

    private void Start()
    {
        maze =  GameObject.FindObjectOfType<Maze_Tiles>();

    }

    void OnTriggerEnter2D(Collider2D other)
	{
		if(other.name == "pacman")
		{
			GameManager.score += 10;
		    GameObject[] pacdots = GameObject.FindGameObjectsWithTag("pacdot");
            maze.tiles[(int)transform.position.x, (int)transform.position.y].pacdot = false;
            Destroy(gameObject);
		    if (pacdots.Length == 1)
		    {
		        GameObject.FindObjectOfType<GameGUINavigation>().LoadLevel();
		    }
		}
	}
}
