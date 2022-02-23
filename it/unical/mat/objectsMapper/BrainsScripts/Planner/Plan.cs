using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Planner
{
    public class Plan
    {
        internal static int checkingPlan = 0;
        internal static int totalAction=0;
        internal List<Action> actions;
        public int priority;
        private bool _isExecuting;
        public bool IsExecuting
        {
            get
            {
                return _isExecuting;
            }
        }
        internal Plan()
        {
            actions = new List<Action>();
        }

        public Plan(List<Action> lists)
        {
            actions = new List<Action>();
            for (int i = 0; i < lists.Count; i++)
            {
                actions.Add(lists[i]);
                actions[i].belongingTO = this;
            }
        }

        private Action Next()
        {
            if (!HasExecutableAction().Equals(State.READY))
            {
                throw new Exception("No executable actions are available");
            }
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].Prerequisite().Equals(State.READY))
                {
                    actions.RemoveRange(0, i);
                    break;
                }
            }
            Action toReturn = actions[0];
            actions.RemoveAt(0);
            return toReturn;
        }
        private State HasExecutableAction()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].Prerequisite().Equals(State.READY))
                {
                    return State.READY;
                }
                if (actions[i].Prerequisite().Equals(State.ABORT))
                {
                    return State.ABORT;
                }
            }
            return State.WAIT;
        }

        internal bool IsExecutable()
        {
            return !HasExecutableAction().Equals(State.ABORT);
        }

        internal bool IsReadyToExecute()
        {
            return HasExecutableAction().Equals(State.READY);
        }

        internal IEnumerator ApplyPlan(Scheduler scheduler)
        {
            if (actions.Count == 0)
            {
                try
                {
                    yield break;
                }
                finally
                {
                    scheduler.IsWaiting = true;
                }
            }
            scheduler.IsWaiting = false;
            _isExecuting = true;
            while (actions.Count > 0)
            {
                State planState=State.WAIT;
                checkingPlan++;
                totalAction += actions.Count;
                while (!PlanNotWaiting(ref planState))
                {
                    yield return null;
                }
                totalAction -= actions.Count;
                checkingPlan--;
                if (planState.Equals(State.READY))
                {
                    Action next = Next();
                    next.Do();
                    yield return new WaitUntil(()=> next.Done());
                }
                else
                {
                    break;
                }
            }
            _isExecuting = false;
            scheduler.IsWaiting = true;
        }

        bool PlanNotWaiting(ref State planState)
        {
            planState = HasExecutableAction();
            return !planState.Equals(State.WAIT);
        }
    }

}
