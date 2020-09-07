using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Player {
    public Image panel;
    public Text text;
    public Button button;
}

[System.Serializable]
public class PlayerColor {
    public Color panelColor;
    public Color textColor;
}

public class GameController : MonoBehaviour {

    public Text[] buttonList;
    public GameObject gameOverPanel;
    public Text gameOverText;
    public GameObject restartButton;
    public Player playerX;
    public Player playerO;
    public PlayerColor activePlayerColor;
    public PlayerColor inactivePlayerColor;
    public GameObject startInfo;
    public string PlayerToPlay { get => playerSide.ToLower(); }
    public static bool GameStarted { get; private set; }
    public static bool IaHasToPlay { get; private set; }

    private string humanPlayerSide;
    private string iaPlayerSide;
    private string playerSide = "empty";
    private int moveCount;

    void Awake() {
        SetGameControllerReferenceOnButtons();
        gameOverPanel.SetActive(false);
        moveCount = 0;
        restartButton.SetActive(false);
    }

    void SetGameControllerReferenceOnButtons() {
        for (int i = 0; i < buttonList.Length; i++) {
            buttonList[i].GetComponentInParent<GridSpace>().SetGameControllerReference(this);
        }
    }

    /*
    public void SetStartingSide(string startingSide) {
        playerSide = startingSide;
        if (playerSide == "X") {
            SetPlayerColors(playerX, playerO);
        } else {
            SetPlayerColors(playerO, playerX);
        }

        StartGame();
    }
    */

    public void SetPlayerSide(string humanPlayerSide) {
        playerSide = "X";
        this.humanPlayerSide = humanPlayerSide;
        iaPlayerSide = (this.humanPlayerSide == "X") ? "O" : "X";
        IaHasToPlay = iaPlayerSide == "X";
        SetPlayerColors(playerX, playerO);
        StartGame();
    }

    void StartGame() {
        SetBoardInteractable(HumanHasToPlay());
        SetPlayerButtons(false);
        startInfo.SetActive(false);
        GameStarted = true;
    }

    public string GetPlayerSide() {
        return playerSide;
    }

    public void EndTurn() {
        moveCount++;

        if (buttonList[0].text == playerSide && buttonList[1].text == playerSide && buttonList[2].text == playerSide) {
            GameOver(playerSide);
        } else if (buttonList[3].text == playerSide && buttonList[4].text == playerSide && buttonList[5].text == playerSide) {
            GameOver(playerSide);
        } else if (buttonList[6].text == playerSide && buttonList[7].text == playerSide && buttonList[8].text == playerSide) {
            GameOver(playerSide);
        } else if (buttonList[0].text == playerSide && buttonList[3].text == playerSide && buttonList[6].text == playerSide) {
            GameOver(playerSide);
        } else if (buttonList[1].text == playerSide && buttonList[4].text == playerSide && buttonList[7].text == playerSide) {
            GameOver(playerSide);
        } else if (buttonList[2].text == playerSide && buttonList[5].text == playerSide && buttonList[8].text == playerSide) {
            GameOver(playerSide);
        } else if (buttonList[0].text == playerSide && buttonList[4].text == playerSide && buttonList[8].text == playerSide) {
            GameOver(playerSide);
        } else if (buttonList[2].text == playerSide && buttonList[4].text == playerSide && buttonList[6].text == playerSide) {
            GameOver(playerSide);
        } else if (moveCount >= 9) {
            GameOver("draw");
        } else {
            ChangeSides();
        }
    }

    public bool HumanHasToPlay() {
        return playerSide == humanPlayerSide;
    }

    void ChangeSides() {
        playerSide = (playerSide == "X") ? "O" : "X";
        IaHasToPlay = !IaHasToPlay;
        SetBoardInteractable(HumanHasToPlay());
        if (playerSide == "X") {
            SetPlayerColors(playerX, playerO);
        } else {
            SetPlayerColors(playerO, playerX);
        }

    }

    void SetPlayerColors(Player newPlayer, Player oldPlayer) {
        newPlayer.panel.color = activePlayerColor.panelColor;
        newPlayer.text.color = activePlayerColor.textColor;
        oldPlayer.panel.color = inactivePlayerColor.panelColor;
        oldPlayer.text.color = inactivePlayerColor.textColor;
    }

    void GameOver(string winningPlayer) {
        SetBoardInteractable(false);
        if (winningPlayer == "draw") {
            SetGameOverText("It's a Draw!");
            SetPlayerColorsInactive();
        } else {
            SetGameOverText(winningPlayer + " Wins!");
        }
        IaHasToPlay = false;
        restartButton.SetActive(true);
    }

    void SetGameOverText(string value) {
        gameOverPanel.SetActive(true);
        gameOverText.text = value;
    }

    public void RestartGame() {
        moveCount = 0;
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        SetPlayerButtons(true);
        SetPlayerColorsInactive();
        startInfo.SetActive(true);

        for (int i = 0; i < buttonList.Length; i++) {
            buttonList[i].text = "";
            buttonList[i].GetComponentInParent<GridSpace>().player = "empty";
        }
    }

    void SetBoardInteractable(bool toggle) {
        for (int i = 0; i < buttonList.Length; i++) {
            bool empty = buttonList[i].GetComponentInParent<GridSpace>().player == "empty";
            buttonList[i].GetComponentInParent<Button>().interactable = toggle && empty;
        }

    }

    void SetPlayerButtons(bool toggle) {
        playerX.button.interactable = toggle;
        playerO.button.interactable = toggle;
    }

    void SetPlayerColorsInactive() {
        playerX.panel.color = inactivePlayerColor.panelColor;
        playerX.text.color = inactivePlayerColor.textColor;
        playerO.panel.color = inactivePlayerColor.panelColor;
        playerO.text.color = inactivePlayerColor.textColor;
    }
}