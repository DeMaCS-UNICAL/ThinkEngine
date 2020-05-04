using Assets.Scripts;
using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using System;
using System.Collections;
using UnityEngine;

public class SwipeInput : MonoBehaviour {
    public delegate void swipeInput();
    public event swipeInput SwipedLeft;
    public event swipeInput SwipedRight;
    public event swipeInput SwipedUp;
    public event swipeInput SwipedDown;

    [SerializeField] private bool detectionEnabled = true;
    [SerializeField] private KeyCode mouseSwipeKey = KeyCode.Mouse0;
    [SerializeField] private float minDistance;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private AIPlayer ai;

	void Start() {
        ai = GameObject.FindObjectOfType<AIPlayer>();
        /*if (Application.isMobilePlatform) {
            Debug.Log("Start swipe detection for touch input");
            StartCoroutine(swipeDetectionTouch());
        }
        else {
            Debug.Log("Start swipe detection for mouse input");
            StartCoroutine(swipeDetectionMouse());
        }*/
        if (GameObject.FindObjectOfType<Brain>().enableBrain)
        {
            StartCoroutine(detectAIMove());
        }
        else
        {
            StartCoroutine(swipeDetectionMouse());
        }
    }

    private IEnumerator detectAIMove()
    {
        while (true)
        {
            if (detectionEnabled)
            {
                
                if (ai.start!=Vector3.zero || ai.end!=Vector3.zero)
                {
                    //Debug.Log(ai.start+" "+ai.end);
                    startPosition = ai.start;
                    endPosition = ai.end;
                    checkDirection();
                    ai.start = Vector3.zero;
                    ai.end = Vector3.zero;
                }
            }
            yield return null;
        }
    }

    private IEnumerator swipeDetectionMouse() {
        while (true) {
            if(detectionEnabled) {
                if (Input.GetKeyDown(mouseSwipeKey)) {
                    startPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                }
                else if (Input.GetKeyUp(mouseSwipeKey)) {
                    endPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                    checkDirection();
                }
            }

            yield return null;
        }
    }

    private IEnumerator swipeDetectionTouch() {
        while (true) {
            if(detectionEnabled) {
                foreach (Touch th in Input.touches) {
                    if (th.phase == TouchPhase.Began) {
                        startPosition = Camera.main.ScreenToViewportPoint(th.position);
                    }
                    else if (th.phase == TouchPhase.Ended) {
                        endPosition = Camera.main.ScreenToViewportPoint(th.position);
                        checkDirection();
                    }
                }
            }

            yield return null;
        }
    }

    private void checkDirection() {
        float directionHorizontal = endPosition.x - startPosition.x;
        float directionVertical = endPosition.y - startPosition.y;
        //Debug.Log(string.Format("Swipe direction: [{0:0.000}, {1:0.000}]", directionHorizontal, directionVertical));

        if (directionHorizontal < 0 && directionHorizontal + minDistance < 0) {
            //Debug.Log("Swipe left");
            if (SwipedLeft != null) SwipedLeft();
        }
        else if (directionHorizontal > 0 && directionHorizontal - minDistance > 0) {
            //Debug.Log("Swipe right");
            if (SwipedRight != null) SwipedRight();
        }
        else if(directionVertical > 0 && directionVertical - minDistance > 0) {
            //Debug.Log("Swipe up");
            if (SwipedUp != null) SwipedUp();
        }
        else if (directionVertical < 0 && directionVertical + minDistance < 0) {
            //Debug.Log("Swipe down");
            if (SwipedDown != null) SwipedDown();
        }
    }
}
