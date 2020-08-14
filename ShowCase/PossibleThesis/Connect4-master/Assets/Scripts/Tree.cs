using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ConnectFour
{
  public class NodeEvaluator
  {
    static double explorationParameter = Math.Sqrt (2);

    static double Evaluate (Node node)
    {
      return 0;
    }
  }

  public class Node
  {
    // le nombre de simulation gagnées depuis ce noeud
    public int wins { get; set; }
    // Le nombre de simulation jouées depuis ce noeud
    public int plays { get; set; }

    // vrai si c'est le joueur qui a appelé MCTS qui doit joueur à ce noeud
    bool isPlayersTurn { get; set; }
    // le noeud parent
    Node parent { get; set; }
    // on associe état enfant à l'action qui mène à cette état
    public Dictionary<Node, int> children;
    // référence vers le jeu utilisé pour la simulation


		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectFour.Node"/> class.
		/// </summary>
		/// <param name="parentNode">Parent node.</param>
    public Node (bool isPlayerTurn = false, Node parentNode = null)
    {
      wins = 0;
      plays = 0;
      this.isPlayersTurn = isPlayerTurn;
      // this.field = field;
      // children = new Dictionary<int, Node> ();
      children = new Dictionary<Node, int> ();
      parent = parentNode;
    }

    /// <summary>
    /// Adds a child to the node.
    /// </summary>
    /// <param name="node">The node to add to the child pool</param>
    /// <param name="line">The line number used to get to this child play</param>
    public void addChild (Node node, int line)
    {
      children.Add (node, line);
    }

    /// <summary>
    /// Gets the child action number.
    /// </summary>
    /// <returns>The child action.</returns>
    /// <param name="node">A node.</param>
    public int getChildAction (Node node)
    {
      return children [node];
    }

    /// <summary>
    /// Gets the children number.
    /// </summary>
    /// <returns>The children number.</returns>
    public int getChildrenNumber ()
    {
      return children.Count;
    }

    /// <summary>
    /// Select a node to expand in the current MCTS
    /// </summary>
    /// <returns>
    /// The selected node
    /// </returns>
    public Node SelectNodeToExpand (int nbSimulation, Field simulatedField)
    {
      //check for end game
      if (!simulatedField.ContainsEmptyCell () || simulatedField.CheckForVictory ()) {
        return this;
      }

      // If not all plays have been tried
      if (children.Keys.Count != simulatedField.GetPossibleDrops ().Count)
        return this;
      
      Node bestNode = null;
			bestNode = selectBestChild (nbSimulation);
      simulatedField.DropInColumn (children [bestNode]);
      simulatedField.SwitchPlayer ();
      return bestNode.SelectNodeToExpand (nbSimulation, simulatedField);
    }

    /// <summary>
    /// Instantiate a child below the selected node and attach it to the tree.
    /// </summary>
    /// <returns>The new node created</returns>
		public Node Expand (Field simulatedField, System.Random r)
    {

      //if selected node is a leaf
      if (!simulatedField.ContainsEmptyCell () || simulatedField.CheckForVictory ())
        return this;

      // Copy of the possible plays list
      List<int> drops = new List<int> (simulatedField.GetPossibleDrops ());

      // For each available plays, remove the ones that have already been play.
      foreach (int column in children.Values) {
        if (drops.Contains (column))
          drops.Remove (column);
      }
      // Gets a line to play on.
			int colToPlay = drops [r.Next(0, drops.Count)];
      Node n = new Node (simulatedField.IsPlayersTurn, this);
      // Adds the child to the tree
      addChild (n, colToPlay);
      simulatedField.DropInColumn (colToPlay);
      simulatedField.SwitchPlayer ();
      return n;
    }

    /// <summary>
    /// Simulate a game play based on the specified baseNode.
    /// </summary>
    /// <returns>True if the simulation leads to a win for the main player</returns>
		public bool Simulate (Field simulatedField, System.Random r)
    {
      if (simulatedField.CheckForVictory ()) {
        return !simulatedField.IsPlayersTurn;
      }
      while (simulatedField.ContainsEmptyCell ()) {
        int column = simulatedField.GetRandomMove (r);
        simulatedField.DropInColumn (column);

        if (simulatedField.CheckForVictory ()) {
          return simulatedField.IsPlayersTurn;
        }
        simulatedField.SwitchPlayer ();
      }
			return true;
    }

    /// <summary>
    /// Does the back propagation from this simulated game to the root.
    /// </summary>
    /// <param name="playersVictory">True if the value to propagate is a victory for the main player</param>
    public void BackPropagate (bool playersVictory)
    {
      plays++;
      if (isPlayersTurn == playersVictory) {
        wins++;
      }
      if (parent != null) {
        parent.BackPropagate (playersVictory);
      }
    }

    /// <summary>
    /// Selects the best child from this node with UBC1 technique.
    /// </summary>
    /// <returns>The best child.</returns>
    public Node selectBestChild (int nbSimulation)
    {
      double maxValue = -1;
      Node bestNode = null;
      foreach (Node child in children.Keys) {
        double evaluation = (double)child.wins / (double)child.plays + Math.Sqrt (2 * Math.Log ((double)nbSimulation) / (double)child.plays);
        if (maxValue < evaluation) {
          maxValue = evaluation;
          bestNode = child;
        }
      }
      return bestNode;
    }

		/// <summary>
		/// Select the mosts the selected move.
		/// </summary>
		/// <returns>The selected move column number.</returns>
    public int MostSelectedMove ()
    {
      double maxValue = -1;
      int bestMove = -1;
      foreach (var child in children) {
        if ((double)child.Key.wins/(double)child.Key.plays > maxValue) {
          bestMove = child.Value;
          maxValue = (double)child.Key.wins/(double)child.Key.plays;
        }
      }
      return bestMove;
    }
  }

  public class MonteCarloSearchTree
  {
    // correspond à l'état actuel du jeu
    // /!\ c'est une référence vers l'état du jeu, NE PAS MODIFIER
    public readonly Field currentStateField;
    // état du jeu utilisé pour la simulation
    public Field simulatedStateField;
    // Pour éviter de faire des copies profondes du jeu pour chaque noeud
    // je pense que chaque noeud doit juste avoir une référence vers cet état
    // lors de chaque itération de l'arbre, copier le contenu de currentStateField
    // dans simulatedStateField (fonction Clone()), puis pour itérer sur l'arbre
    // faire comme si on jouait avec ce Field (fonction DropInColumn(int)).

    public int nbIteration;

    //threadpool attribute (needed for parallel processing)
    public ManualResetEvent doneEvent;

    // racine de l'arbre
    public Node rootNode;

    public MonteCarloSearchTree(Field field, ManualResetEvent _doneEvent, int _nbIteration)
    {
      doneEvent = _doneEvent;
      currentStateField = field;
      this.nbIteration = _nbIteration;
    }
  }
}

