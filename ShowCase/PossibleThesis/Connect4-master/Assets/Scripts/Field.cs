using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

  public class Field
  {

    enum Piece
    {
      Empty = 0,
      Blue = 1,
      Red = 2
    }

    private int numRows;

    public int NumRows {
      get { return numRows; }
    }

    private int numColumns;

    public int NumColumns {
      get { return numColumns; }
    }

    private int numPiecesToWin;

    private bool allowDiagonally = true;

    public int[,] field;

    private bool isPlayersTurn;

    public bool IsPlayersTurn {
      get { return isPlayersTurn; }
    }

    private int piecesNumber = 0;

    public int PiecesNumber {
      get { return piecesNumber; }
    }

    // memorize last move
    private int dropColumn;
    private int dropRow;

  // Field constructor
  public Field(int numRows, int numColumns, int numPiecesToWin, bool allowDiagonally)
  {
            this.numRows = numRows;
            this.numColumns = numColumns;
            this.numPiecesToWin = numPiecesToWin;
            this.allowDiagonally = allowDiagonally;

            isPlayersTurn = System.Convert.ToBoolean(UnityEngine.Random.Range(0, 2));

            field = new int[numColumns, numRows];
            for (int x = 0; x < numColumns; x++)
            {
                for (int y = 0; y < numRows; y++)
                {
                    field[x, y] = (int)Piece.Empty;
                }
            }
            dropColumn = 0;
            dropRow = 0;
        }

        public Field(int numRows, int numColumns, int numPiecesToWin, bool allowDiagonally, bool isPlayersTurn, int piecesNumber, int[,] field)
        {
            this.numRows = numRows;
            this.numColumns = numColumns;
            this.numPiecesToWin = numPiecesToWin;
            this.allowDiagonally = allowDiagonally;
            this.isPlayersTurn = isPlayersTurn;
            this.piecesNumber = piecesNumber;

            this.field = new int[numColumns, numRows];
            for (int x = 0; x < numColumns; x++)
            {
                for (int y = 0; y < numRows; y++)
                {
                    this.field[x, y] = field[x, y];
                }
            }
        }
  	
    // Renvoie la liste des cases où le joueur peut ajouter une pièce
    public Dictionary<int, int> GetPossibleCells ()
    {
      Dictionary<int, int> possibleCells = new Dictionary<int, int> ();
      for (int x = 0; x < numColumns; x++) {
        for (int y = numRows - 1; y >= 0; y--) {
          if (field [x, y] == (int)Piece.Empty) {
            possibleCells.Add (x, y);
            break;
          }
        }
      }
      return possibleCells;
    }

    // Renvoie la liste des colonnes où le joueur peut lâcher une pièce
    public List<int> GetPossibleDrops ()
    {
      List<int> possibleDrops = new List<int> ();
      for (int x = 0; x < numColumns; x++) {
        for (int y = numRows - 1; y >= 0; y--) {
          if (field [x, y] == (int)Piece.Empty) {
            possibleDrops.Add (x);
            break;
          }
        }
      }
      return possibleDrops;
    }
     
    // renvoie un mouvement aléatoire parmi tous ceux possibles
    public int GetRandomMove ()
    {
      List<int> moves = GetPossibleDrops ();

      if (moves.Count > 0) {
				System.Random r = new System.Random ();
				return moves [r.Next(0, moves.Count)];
      }
      return -1;
    }

		// renvoie un mouvement aléatoire parmi tous ceux possibles
		public int GetRandomMove (System.Random r)
		{
			List<int> moves = GetPossibleDrops ();

			if (moves.Count > 0) {
				return moves [r.Next(0, moves.Count)];
			}
			return -1;
		}

    // Lâche une pièce dans la colonne i, renvoie la ligne où elle tombe
    public int DropInColumn (int col)
    {
      for (int i = numRows - 1; i >= 0; i--) {
        if (field [col, i] == 0) {
          field [col, i] = isPlayersTurn ? (int)Piece.Blue : (int)Piece.Red;
          piecesNumber += 1;
          dropColumn = col;
          dropRow = i;
          return i;
        }
      }
      return -1;
    }

    // Alterne de joueur
    public void SwitchPlayer ()
    {
      isPlayersTurn = !isPlayersTurn;
    }

    // Vérifie si la partie est gagnée (se référer à isPlayerTurn pour savoir qui est le gagnant)
    public bool CheckForWinner ()
    {
      for (int x = 0; x < numColumns; x++) {
        for (int y = 0; y < numRows; y++) {
          // Get the Laymask to Raycast against, if its Players turn only include
          // Layermask Blue otherwise Layermask Red
          int layermask = isPlayersTurn ? (1 << 8) : (1 << 9);

          // If its Players turn ignore red as Starting piece and wise versa
          if (field [x, y] != (isPlayersTurn ? (int)Piece.Blue : (int)Piece.Red)) {
            continue;
          }

          // shoot a ray of length 'numPiecesToWin - 1' to the right to test horizontally
          RaycastHit[] hitsHorz = Physics.RaycastAll (
                                    new Vector3 (x, y * -1, 0), 
                                    Vector3.right, 
                                    numPiecesToWin - 1, 
                                    layermask);

          // return true (won) if enough hits
          if (hitsHorz.Length == numPiecesToWin - 1) {
            return true;
          }

          // shoot a ray up to test vertically
          RaycastHit[] hitsVert = Physics.RaycastAll (
                                    new Vector3 (x, y * -1, 0), 
                                    Vector3.up, 
                                    numPiecesToWin - 1, 
                                    layermask);

          if (hitsVert.Length == numPiecesToWin - 1) {
            return true;
          }

          // test diagonally
          if (allowDiagonally) {
            // calculate the length of the ray to shoot diagonally
            float length = Vector2.Distance (new Vector2 (0, 0), new Vector2 (numPiecesToWin - 1, numPiecesToWin - 1));

            RaycastHit[] hitsDiaLeft = Physics.RaycastAll (
                                         new Vector3 (x, y * -1, 0), 
                                         new Vector3 (-1, 1), 
                                         length, 
                                         layermask);

            if (hitsDiaLeft.Length == numPiecesToWin - 1) {
              return true;
            }

            RaycastHit[] hitsDiaRight = Physics.RaycastAll (
                                          new Vector3 (x, y * -1, 0), 
                                          new Vector3 (1, 1), 
                                          length, 
                                          layermask);

            if (hitsDiaRight.Length == numPiecesToWin - 1) {
              return true;
            }
          }
        }
      }
      return false;
    }

    // Vérifie si la partie a été gagné en spécifiance quel est le dernier pion joué
    public bool CheckForVictory ()
    {
      int colour = field [dropColumn, dropRow];
      if (colour == 0) {
        return false;
      }
        
      bool bottomDirection = true;
      int currentAlignment = 1; //count current Piece

      //check vertical alignment
      for(int i = 1; i <= numPiecesToWin; i++) {
        if (bottomDirection && dropRow + i < NumRows) {
          if (field [dropColumn, dropRow + i] == colour)
            currentAlignment++;
          else
            bottomDirection = false;
        }

        if (currentAlignment >= numPiecesToWin)
          return true;
      }

      //reset var
      bool rightDirection = true;
      bool leftDirection = true;
      currentAlignment = 1;

      //check horizontal alignment
      for(int i = 1; i <= numPiecesToWin; i++) {
        if (rightDirection && dropColumn + i < numColumns) {
          if (field [dropColumn + i, dropRow] == colour)
            currentAlignment++;
          else
            rightDirection = false;
        }

        if (leftDirection && dropColumn - i >= 0) {
          if (field [dropColumn - i, dropRow] == colour)
            currentAlignment++;
          else
            leftDirection = false;
        }

        if (currentAlignment >= numPiecesToWin)
          return true;
      }

      //check diagonal alignment
      if (allowDiagonally) {
        //reset var
        bool upRightDirection = true;
        bool bottomLeftDirection = true;
        currentAlignment = 1;

        for(int i = 1; i <= numPiecesToWin; i++) {
          if (upRightDirection && dropColumn + i < numColumns && dropRow + i < numRows) {
            if (field [dropColumn + i, dropRow + i] == colour)
              currentAlignment++;
            else
              upRightDirection = false;
          }

          if (bottomLeftDirection && dropColumn - i >= 0 && dropRow - i >= 0 ) {
            if (field [dropColumn - i, dropRow - i] == colour)
              currentAlignment++;
            else
              bottomLeftDirection = false;
          }

          if (currentAlignment >= numPiecesToWin)
            return true;
        }

        //reset var
        bool upLeftDirection = true;
        bool bottomRightDirection = true;
        currentAlignment = 1;

        for(int i = 1; i <= numPiecesToWin; i++) {
          if (upLeftDirection && dropColumn + i < numColumns && dropRow - i >= 0) {
            if (field [dropColumn + i, dropRow - i] == colour)
              currentAlignment++;
            else
              upLeftDirection = false;
          }

          if (bottomRightDirection && dropColumn - i >= 0 && dropRow + i < numRows ) {
            if (field [dropColumn - i, dropRow + i] == colour)
              currentAlignment++;
            else
              bottomRightDirection = false;
          }

          if (currentAlignment >= numPiecesToWin)
            return true;
        }
      }

      return false;
    }

    // Vérifie si la grille contient encore des cellules vides
    public bool ContainsEmptyCell ()
    {
      return (piecesNumber < numRows * numColumns);
    }

    // Execute une copie profonde de l'état du jeu
    public Field Clone ()
    {
      return new Field (numRows, numColumns, numPiecesToWin, allowDiagonally, isPlayersTurn, piecesNumber, field);
    }

    public String ToString ()
    {
      String str = "Player";
      str += isPlayersTurn ? "1\n" : "2\n";
      for (int y = 0; y < numRows; y++) {
        for (int x = 0; x < numColumns; x++) {
          str += (field [x, y]).ToString ();
        }
        str += "\n";
      }
      return str;
    }
  }
 
