using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CircleBar : MonoBehaviour {
    public delegate void barFinish();
    public event barFinish BarFinished;

    [SerializeField] private float _currentProgress;
    [SerializeField] private float _currentValue;
    [SerializeField] private string valueFormat = "{0:0}";
    [SerializeField] private bool displayCurrentValue = true;
    [SerializeField] private bool raiseSmooth = true;

    private Image progressBar;
    private Text valueText;

    public float currentProgress { get { return _currentProgress; } private set { _currentProgress = value; } }
    public float currentValue { get { return _currentValue; } private set { _currentValue = value; } }

	void Awake () {
        BarFinished = null;
        progressBar = transform.GetChild(0).GetComponent<Image>();
        valueText = transform.GetChild(2).GetComponent<Text>();
	}

    public void raiseProgress(float change) {
        if (change > 1f) Debug.LogWarning(string.Format("Bar progression cannot be bigger than 1.0f - was {0}", change), gameObject);
        else if (change < 0.01f) Debug.LogWarning(string.Format("Bar progression cannot be less than 0.01f - was {0}", change), gameObject);
        else {
            if (raiseSmooth) StartCoroutine(smoothProgressCoroutine(change));
            else {
                currentProgress += change;
                updateProgress();
            }
        }
    }
	
    private void progressFinished() {
        if(displayCurrentValue) {
            ++currentValue;
            currentProgress = 0;
            updateProgress();
        }
        if (BarFinished != null) BarFinished();
    }

    private void updateProgress() {
        progressBar.fillAmount = currentProgress;
        if (displayCurrentValue) updateValue();
        if (currentProgress >= 1f) progressFinished();
    }

    private void updateValue() {
        valueText.text = string.Format(valueFormat, currentValue);
    }

    private IEnumerator smoothProgressCoroutine(float change) {
        float movePerChange = 0.01f;

        while (change > 0) {
            yield return new WaitForSeconds(0.01f);
            change = (float)System.Math.Round(change - movePerChange, 2);

            currentProgress += movePerChange;
            currentProgress = (float)System.Math.Round(currentProgress, 2);
            updateProgress();
        }

        if(currentProgress >= 1f) progressFinished();
    }
}
