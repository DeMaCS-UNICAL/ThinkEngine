using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ThinkEngine
{
    [DefaultExecutionOrder(-1)]
    internal class StopwatchManager : MonoBehaviour
    {
        private Stopwatch _stopwatch;

        private void Awake()
        {
            _stopwatch = new Stopwatch();
        }

        private void Update()
        {
            _stopwatch.Restart(); // Stopwatch start maesurement
        }

        internal double TakeCurrentTime()
        {
            _stopwatch.Stop();
            double endTime = _stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Start();
            return endTime;
        }

        internal double TakeDeltaTime(double startTime)
        {
            _stopwatch.Stop();
            double endTime = _stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Start();
            return (endTime - startTime);
        }
    }
}
