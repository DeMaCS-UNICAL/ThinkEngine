using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GridSpace : MonoBehaviour {
    public int x;
    public int y;
    public string player;
    public Button button;
    public Text buttonText;
    private GameController gameController;

    public void SetGameControllerReference(GameController controller) {
        gameController = controller;
    }

    public void SetSpace() {
        buttonText.text = gameController.GetPlayerSide();
        player = (buttonText.text.Equals("X")) ? "x" : "o";
        button.interactable = false;
        gameController.EndTurn();
    }
}