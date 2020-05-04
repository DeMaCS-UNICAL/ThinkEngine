using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFinished : MonoBehaviour {
    [SerializeField] string pointsFormat = "Points: {0}";
    [SerializeField] string highscoreFormat = "Highscore: {0}";
    private Text points;
    private Text highscore;
    private GameObject newHighscore;

    void Awake() {
        points = transform.GetChild(2).GetComponent<Text>();
        highscore = transform.GetChild(3).GetComponent<Text>();
        newHighscore = transform.GetChild(4).gameObject;
    }

    public void display(int points) {
        gameObject.SetActive(true);

        int hs = PlayerPrefs.GetInt("Highscore");
        this.points.text = string.Format(pointsFormat, points);

        if (points > hs) {
            newHighscore.SetActive(true);
            PlayerPrefs.SetInt("Highscore", points);
            highscore.text = string.Format(highscoreFormat, points);
        }
        else {
            newHighscore.SetActive(false);
            highscore.text = string.Format(highscoreFormat, hs);
        }
    }
}
