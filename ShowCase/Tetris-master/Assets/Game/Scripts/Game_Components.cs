using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game_components {
    [System.Serializable]
    public class Tetromino : ILevelable {
        [SerializeField] private float _fallTime = 1.0f;
        [SerializeField] private float _boostMultiplier = 0.1f;
        [SerializeField] private float newLevelMultiplier = 0.9f;

        public float fallTime { get { return _fallTime; } private set { _fallTime = value; } }
        public float fallTimeBoosted { get { return _fallTime * _boostMultiplier; } }
        public void newLevel() { fallTime *= newLevelMultiplier; }
    }

    [System.Serializable]
    public class Level : ILevelable {
        [SerializeField] private CircleBar _bar;
        [SerializeField] private int rowsToLevel = 5;
        [SerializeField] private float newLevelMultiplier = 1.5f;

        public CircleBar bar { get { return _bar; } }

        public void rowRemoved() { bar.raiseProgress(1f / rowsToLevel); }
        public void newLevel() { rowsToLevel = System.Convert.ToInt32(rowsToLevel * newLevelMultiplier); }
    }

    [System.Serializable]
    public class Point : ILevelable {
        [SerializeField] private Text displayValue;
        [SerializeField] private int pointsPerRow = 15;
        [SerializeField] private float newLevelMultiplier = 1.5f;
        private int _points;

        public int points {
            get { return _points; }
            set {
                _points = value;
                displayValue.text = value.ToString();
            }
        }

        public void setPoints(int val) { points = val; }
        public void addPoints() { points += pointsPerRow; }
        public void newLevel() { pointsPerRow = System.Convert.ToInt32(pointsPerRow * newLevelMultiplier); }
    }

    [System.Serializable]
    public class PauseGame {
        [SerializeField] private GameObject controller;
        [SerializeField] private bool _paused = false;

        public bool paused {
            get { return _paused; }
            set {
                _paused = value;
                controller.SetActive(value);
            }
        }
    }

    public interface ILevelable {
        void newLevel();
    }
}