using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planner
{
    internal class Plan
    {
        private List<Action> actions;
        private bool _isExecuting;
        internal bool IsExecuting
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
            actions = new List<Action>(lists);
        }

        private Action Next()
        {
            if (!HasExecutableAction())
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
        private bool HasExecutableAction()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].Prerequisite().Equals(State.READY))
                {
                    return true;
                }
            }
            return false;
        }
        internal bool IsExecutable()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].Prerequisite().Equals(State.ABORT))
                {
                    return false;
                }
            }
            return true;
        }
        internal IEnumerator ApplyPlan(Scheduler scheduler)
        {
            Debug.Log("Applying plan");
            if (actions.Count == 0)
            {
                scheduler.IsWaiting = true;
                yield break;
            }
            _isExecuting = true;
            while (actions.Count > 0 && IsExecutable())
            {
                yield return new WaitUntil(() => !IsExecutable() || HasExecutableAction());
                if (IsExecutable())
                {
                    Action next = Next();
                    next.Do();
                    yield return new WaitUntil(() => next.Done());
                }
            }
            _isExecuting = false;
            scheduler.IsWaiting = true;
        }
    }
}
