using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;


public class PlayerController : MonoBehaviour {

  public static int numberOfSteps = 0;
  public float speed = 0.4f;
  Vector2 _dest = Vector2.zero;
  Vector2 _dir = Vector2.zero;
  Vector2 _nextDir = Vector2.zero;
  private bool keyboard = false;
  public string nextStep;
    public string previousStep;
    public string prePreviousStep;



  [Serializable]
  public class PointSprites {
    public GameObject[] pointSprites;
  }

  public PointSprites points;

  public static int killstreak = 0;

  // script handles
  private GameGUINavigation GUINav;
  private GameManager GM;
  private ScoreManager SM;

  //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
  public bool _deadPlaying = false;

  // Use this for initialization
  void Start() {
    //watch.Start();
    GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
    SM = GameObject.Find("Game Manager").GetComponent<ScoreManager>();
    GUINav = GameObject.Find("UI Manager").GetComponent<GameGUINavigation>();
    _dest = transform.position;
        nextStep = "n";
        previousStep = "n";
        prePreviousStep = "n";
        // embasp = EmbASPManager.Instance;

    }


  IEnumerator GetText() {
    UnityWebRequest www = UnityWebRequest.Get("172.16.1.5/test.php");
    yield return www.SendWebRequest();

    if (www.isNetworkError || www.isHttpError) {
      Debug.Log(www.error);
    }
    else {
      // Show results as text
      Debug.Log(www.downloadHandler.text);

      // Or retrieve results as binary data
      byte[] results = www.downloadHandler.data;
    }
  }


  // Update is called once per frame
  void FixedUpdate() {
    switch (GameManager.gameState) {
      case GameManager.GameState.Game:

        //if (keyboard)
          ReadInputAndMove();

          //ReadInputAndMove(new SymbolicConstant());
        /*else {
          //StartCoroutine(GetText());
          SymbolicConstant newMove = embasp.PreviousMove;
          Vector3 currentPos = new Vector3((int)(embasp.Pacman.transform.position.x + 0.499f), (int)(embasp.Pacman.transform.position.y + 0.499f));
          //Debug.Log(currentPos + " " + previousPos);
          if (Math.Abs(currentPos.x - embasp.PreviousPos.x) + Math.Abs(currentPos.y - embasp.PreviousPos.y) >= 1) {
            //Debug.Log("Current Pos: " + currentPos);
            embasp.PreviousPos = currentPos;
            newMove = embasp.ASPMove();
          }
          ReadInputAndMove(newMove);
        }*/
        Animate();
        break;

      case GameManager.GameState.Dead:
                /* if (!_deadPlaying)
                   EmbASPManager.Instance.GenerateCharacters();*/
                nextStep = "n";
        StartCoroutine("PlayDeadAnimation");
        break;
    }


  }


  IEnumerator PlayDeadAnimation() {
    _deadPlaying = true;
    GetComponent<Animator>().SetBool("Die", true);
    yield return new WaitForSeconds(1);
    GetComponent<Animator>().SetBool("Die", false);

    if (GameManager.lives <= 0) {
      UnityEngine.Debug.Log("Treshold for High Score: " + SM.LowestHigh());
      if (GameManager.score >= SM.LowestHigh())
        GUINav.getScoresMenu();
      else
        GUINav.H_ShowGameOverScreen();
    }

    else
      GM.ResetScene();
        _deadPlaying = false;

    }

    void Animate() {
    Vector2 dir = _dest - (Vector2)transform.position;
    GetComponent<Animator>().SetFloat("DirX", dir.x);
    GetComponent<Animator>().SetFloat("DirY", dir.y);
  }

  bool Valid(Vector2 direction) {
    // cast line from 'next to pacman' to pacman
    // not from directly the center of next tile but just a little further from center of next tile
    Vector2 pos = transform.position;
    direction += new Vector2(direction.x * 0.45f, direction.y * 0.45f);
    RaycastHit2D hit = Physics2D.Linecast(pos + direction, pos);
    return hit.collider.name == "pacdot" || (hit.collider == GetComponent<Collider2D>());
  }

  bool ValidPacDot(Vector2 direction) {
    // cast line from 'next to pacman' to pacman
    // not from directly the center of next tile but just a little further from center of next tile
    Vector2 pos = transform.position;
    direction += new Vector2(direction.x * 0.45f, direction.y * 0.45f);
    RaycastHit2D hit = Physics2D.Linecast(pos + direction, pos);
    return hit.collider.name == "pacdot";
  }

  public void ResetDestination() {
    _dest = new Vector2(15f, 11f);
    GetComponent<Animator>().SetFloat("DirX", 1);
    GetComponent<Animator>().SetFloat("DirY", 0);
  }

  void ReadInputAndMove() {
    // move closer to destination
    Vector2 p = Vector2.MoveTowards(transform.position, _dest, speed);
    GetComponent<Rigidbody2D>().MovePosition(p);

        // get the next direction from keyboard
        //Debug.Log("NEXT: <" + nextStep + ">");
        /*if (!keyboard) {
          if (nextStep.Value.Equals("right")) _nextDir = Vector2.right;
          if (nextStep.Value.Equals("left")) _nextDir = -Vector2.right;
          if (nextStep.Value.Equals("up")) _nextDir = Vector2.up;
          if (nextStep.Value.Equals("down")) _nextDir = -Vector2.up;
        }*/
        if (!keyboard)
        {
            Debug.Log(nextStep);
            if (nextStep.Equals("right")) _nextDir = Vector2.right;
            if (nextStep.Equals("left")) _nextDir = -Vector2.right;
            if (nextStep.Equals("up")) _nextDir = Vector2.up;
            if (nextStep.Equals("down")) _nextDir = -Vector2.up;
            if (previousStep.Equals("N"))
            {
                previousStep = nextStep;
            }else if (!previousStep.Equals(nextStep))
            {
                prePreviousStep = previousStep;
                previousStep = nextStep;
            }

        }
        else
        {
            // get the next direction from keyboard
            if (Input.GetAxis("Horizontal") > 0) _nextDir = Vector2.right;
            if (Input.GetAxis("Horizontal") < 0) _nextDir = -Vector2.right;
            if (Input.GetAxis("Vertical") > 0) _nextDir = Vector2.up;
            if (Input.GetAxis("Vertical") < 0) _nextDir = -Vector2.up;
        }

    // if pacman is in the center of a tile
    if (Vector2.Distance(_dest, transform.position) < 0.00001f) {
      if (Valid(_nextDir)) {
        _dest = (Vector2)transform.position + _nextDir;
        _dir = _nextDir;
      }
      else   // if next direction is not valid
      {
        if (Valid(_dir))  // and the prev. direction is valid
          _dest = (Vector2)transform.position + _dir;   // continue on that direction

        // otherwise, do nothing
      }
    }
    if (Input.GetKeyDown(KeyCode.K))
      keyboard = !keyboard;

    //if (watch.Elapsed.Minutes == 1 && watch.Elapsed.Seconds == 0 && watch.Elapsed.Milliseconds == 0)
    //Debug.Log("EmbASP CALL: " + EmbASPManager.Instance.EmbaspCall + "\nTime: " + watch.Elapsed.Minutes + ":" + watch.Elapsed.Seconds + ":" + watch.Elapsed.Milliseconds);
  }

  public Vector2 GetDir() {
    return _dir;
  }

  public void UpdateScore() {
    killstreak++;

    // limit killstreak at 4
    if (killstreak > 4) killstreak = 4;

    Instantiate(points.pointSprites[killstreak - 1], transform.position, Quaternion.identity);
    GameManager.score += (int)Mathf.Pow(2, killstreak) * 100;

  }
}
