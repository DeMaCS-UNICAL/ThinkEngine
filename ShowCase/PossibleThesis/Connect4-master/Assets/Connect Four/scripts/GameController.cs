using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConnectFour
{
  public class GameController : MonoBehaviour
  { 
    [Range (3, 8)]
    public int numRows = 4;
    [Range (3, 8)]
    public int numColumns = 4;
    [Range (1, 8)]
    public int parallelProcesses = 2;
    [Range (7, 10000)]
    public int MCTS_Iterations = 1000;

    [Tooltip ("Shows column number next to its probability.")]
    public bool log_column = false;

    [Tooltip ("How many pieces have to be connected to win.")]
    public int numPiecesToWin = 4;

    [Tooltip ("Allow diagonally connected Pieces?")]
    public bool allowDiagonally = true;
		
    public float dropTime = 4f;

    // Gameobjects
    public GameObject pieceRed;
    public GameObject pieceBlue;
    public GameObject pieceField;

    public GameObject winningText;
    public string playerWonText = "You Won!";
    public string playerLoseText = "You Lose!";
    public string drawText = "Draw!";

    public GameObject btnPlayAgain;
    bool btnPlayAgainTouching = false;
    Color btnPlayAgainOrigColor;
    Color btnPlayAgainHoverColor = new Color (255, 143, 4);

    GameObject gameObjectField;

    // temporary gameobject, holds the piece at mouse position until the mouse has clicked
    GameObject gameObjectTurn;

    /// <summary>
    /// The Game field.
    /// 0 = Empty
    /// 1 = Blue
    /// 2 = Red
    /// </summary>
    public Field field;

    bool isLoading = true;
    bool isDropping = false;
    bool mouseButtonPressed = false;

    bool gameOver = false;
    bool isCheckingForWinner = false;

    // Use this for initialization
    void Start ()
    {

      int max = Mathf.Max (numRows, numColumns);

      if (numPiecesToWin > max)
        numPiecesToWin = max;

      CreateField ();

      //IsPlayersTurn = System.Convert.ToBoolean(Random.Range (0, 1));

      btnPlayAgainOrigColor = btnPlayAgain.GetComponent<Renderer> ().material.color;
    }

    public Field GetField()
    {
            if(field != null)
                return field.Clone();
            return null;
    }
    /// <summary>
    /// Creates the field.
    /// </summary>
    void CreateField ()
    {
      winningText.SetActive (false);
      btnPlayAgain.SetActive (false);

      isLoading = true;

      gameObjectField = GameObject.Find ("Field");
      if (gameObjectField != null) {
        DestroyImmediate (gameObjectField);
      }
      gameObjectField = new GameObject ("Field");

      // create an empty field and instantiate the cells
      field = new Field (numRows, numColumns, numPiecesToWin, allowDiagonally);
      for (int x = 0; x < numColumns; x++) {
        for (int y = 0; y < numRows; y++) {
          GameObject g = Instantiate (pieceField, new Vector3 (x, y * -1, -1), Quaternion.identity) as GameObject;
          g.transform.parent = gameObjectField.transform;
        }
      }

      isLoading = false;
      gameOver = false;

      // center camera
      Camera.main.transform.position = new Vector3 (
        (numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f), Camera.main.transform.position.z);

      winningText.transform.position = new Vector3 (
        (numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f) + 1, winningText.transform.position.z);

      btnPlayAgain.transform.position = new Vector3 (
        (numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f) - 1, btnPlayAgain.transform.position.z);
    }

    /// <summary>
    /// Spawns a piece at mouse position above the first row
    /// </summary>
    /// <returns>The piece.</returns>
    GameObject SpawnPiece ()
    {
      Vector3 spawnPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

      if (!field.IsPlayersTurn) {

        int column;


        // Inutile de lancer MCST le premier tour
        if (field.PiecesNumber != 0) {
          // One event is used for each MCTS.
          ManualResetEvent[] doneEvents = new ManualResetEvent[parallelProcesses];
          MonteCarloSearchTree[] trees = new MonteCarloSearchTree[parallelProcesses];

          for (int i = 0; i < parallelProcesses; i++) {
            doneEvents [i] = new ManualResetEvent (false);
            trees[i] = new MonteCarloSearchTree (field, doneEvents [i], MCTS_Iterations);
            ThreadPool.QueueUserWorkItem( new WaitCallback(ExpandTree), trees [i]);
          }
				
          WaitHandle.WaitAll(doneEvents);

          //regrouping all results
          Node rootNode = new Node ();
          string log = "";

          for (int i = 0; i < parallelProcesses; i++) {

            log += "( ";
            var sortedChildren = (List<KeyValuePair<Node, int>>)trees [i].rootNode.children.ToList ();
            sortedChildren.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));

            foreach (var child in sortedChildren) {

              if (log_column)
                log += child.Value + ": ";
              log += (int) ( ((double) child.Key.wins / (double) child.Key.plays) * 100) + "% | ";

              if (!rootNode.children.ContainsValue (child.Value)) {
                Node rootChild = new Node ();
                rootChild.wins = child.Key.wins;
                rootChild.plays = child.Key.plays;
                rootNode.children.Add (rootChild, child.Value);
              } else {
                Node rootChild = rootNode.children.First( p => p.Value == child.Value ).Key;
                rootChild.wins += child.Key.wins;
                rootChild.plays += child.Key.plays;
              }
            }

            log = log.Remove(log.Length-3, 3);
            log += " )\n";
          }

          /****************************/
          /***** Log final result *****/
          /****************************/

          string log2 = "( ";
          foreach (var child in rootNode.children) {
            if (log_column)
              log2 += child.Value + ": ";
            log2 += (int) ( ((double) child.Key.wins / (double) child.Key.plays) * 100) + "% | ";
          }
          log2 = log2.Remove(log2.Length-3, 3);
          log2 += " )\n";
          log2 += "*********************************************\n";
          Debug.Log (log);
          Debug.Log (log2);

          /****************************/

          column = rootNode.MostSelectedMove ();
        }
        else
          column = field.GetRandomMove ();
					
        spawnPos = new Vector3 (column, 0, 0);
      }

      GameObject g = Instantiate (
                    field.IsPlayersTurn ? pieceBlue : pieceRed, // is players turn = spawn blue, else spawn red
                    new Vector3 (
                      Mathf.Clamp (spawnPos.x, 0, numColumns - 1), 
                      gameObjectField.transform.position.y + 1, 0), // spawn it above the first row
                    Quaternion.identity) as GameObject;

      return g;
    }

    /// <summary>
    /// Expands the tree.
    /// </summary>
    /// <returns>Root node of the tree.</returns>
		public static void ExpandTree (System.Object t)
    {
      var tree = (MonteCarloSearchTree) t;
      tree.simulatedStateField = tree.currentStateField.Clone ();
      tree.rootNode = new Node (tree.simulatedStateField.IsPlayersTurn);

      Node selectedNode;
      Node expandedNode;
			System.Random r = new System.Random (System.Guid.NewGuid().GetHashCode());

      for (int i = 0; i < tree.nbIteration; i++) {
        // copie profonde
        tree.simulatedStateField = tree.currentStateField.Clone ();

        selectedNode = tree.rootNode.SelectNodeToExpand (tree.rootNode.plays, tree.simulatedStateField);
        expandedNode = selectedNode.Expand (tree.simulatedStateField, r);
        expandedNode.BackPropagate (expandedNode.Simulate (tree.simulatedStateField, r));
      }

      tree.doneEvent.Set ();
    }

    void UpdatePlayAgainButton ()
    {
      RaycastHit hit;
      //ray shooting out of the camera from where the mouse is
      Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			
      if (Physics.Raycast (ray, out hit) && hit.collider.name == btnPlayAgain.name) {
        btnPlayAgain.GetComponent<Renderer> ().material.color = btnPlayAgainHoverColor;
        //check if the left mouse has been pressed down this frame
        if (Input.GetMouseButtonDown (0) || Input.touchCount > 0 && btnPlayAgainTouching == false) {
          btnPlayAgainTouching = true;

          Application.LoadLevel (0);
        }
      } else {
        btnPlayAgain.GetComponent<Renderer> ().material.color = btnPlayAgainOrigColor;
      }
			
      if (Input.touchCount == 0) {
        btnPlayAgainTouching = false;
      }
    }

    // Update is called once per frame
    void Update ()
    {
      if (isLoading)
        return;

      if (isCheckingForWinner)
        return;

      if (gameOver) {
        winningText.SetActive (true);
        btnPlayAgain.SetActive (true);

        UpdatePlayAgainButton ();

        return;
      }

      if (field.IsPlayersTurn) {
        if (gameObjectTurn == null) {
          gameObjectTurn = SpawnPiece ();
        } else {
          // update the objects position
          Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
          gameObjectTurn.transform.position = new Vector3 (
            Mathf.Clamp (pos.x, 0, numColumns - 1), 
            gameObjectField.transform.position.y + 1, 0);

          // click the left mouse button to drop the piece into the selected column
          if (Input.GetMouseButtonDown (0) && !mouseButtonPressed && !isDropping) {
            mouseButtonPressed = true;

            StartCoroutine (dropPiece (gameObjectTurn));
          } else {
            mouseButtonPressed = false;
          }
        }
      } else {
        if (gameObjectTurn == null) {
          gameObjectTurn = SpawnPiece ();
        } else {
          if (!isDropping)
            StartCoroutine (dropPiece (gameObjectTurn));
        }
      }
    }

    /// <summary>
    /// This method searches for a empty cell and lets 
    /// the object fall down into this cell
    /// </summary>
    /// <param name="gObject">Game Object.</param>
    IEnumerator dropPiece (GameObject gObject)
    {
      isDropping = true;

      Vector3 startPosition = gObject.transform.position;
      Vector3 endPosition = new Vector3 ();

      // round to a grid cell
      int x = Mathf.RoundToInt (startPosition.x);
      startPosition = new Vector3 (x, startPosition.y, startPosition.z);

      int y = field.DropInColumn (x);

      if (y != -1) {
        endPosition = new Vector3 (x, y * -1, startPosition.z);

        // Instantiate a new Piece, disable the temporary
        GameObject g = Instantiate (gObject) as GameObject;
        gameObjectTurn.GetComponent<Renderer> ().enabled = false;

        float distance = Vector3.Distance (startPosition, endPosition);

        float t = 0;
        while (t < 1) {
          t += Time.deltaTime * dropTime * ((numRows - distance) + 1);

          g.transform.position = Vector3.Lerp (startPosition, endPosition, t);
          yield return null;
        }

        g.transform.parent = gameObjectField.transform;

        // remove the temporary gameobject
        DestroyImmediate (gameObjectTurn);

        // run coroutine to check if someone has won
        StartCoroutine (Won ());

        // wait until winning check is done
        while (isCheckingForWinner)
          yield return null;

        field.SwitchPlayer ();
      }

      isDropping = false;

      yield return 0;
    }

    /// <summary>
    /// Check for Winner
    /// </summary>
    IEnumerator Won ()
    {
      isCheckingForWinner = true;

      gameOver = field.CheckForWinner ();

      // if Game Over update the winning text to show who has won
      if (gameOver == true) {
        winningText.GetComponent<TextMesh> ().text = field.IsPlayersTurn ? playerWonText : playerLoseText;
      } else {
        // check if there are any empty cells left, if not set game over and update text to show a draw
        if (!field.ContainsEmptyCell ()) {
          gameOver = true;
          winningText.GetComponent<TextMesh> ().text = drawText;
        }
      }

      isCheckingForWinner = false;

      yield return 0;
    }
  }
}
