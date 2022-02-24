using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
            if (!PlanState().Equals(State.READY))
            {
                throw new Exception("Plan not ready to be executed");
            }
            if (actions.Count == 0)
            {
                throw new Exception("Empty plan");
            }
            Action toReturn = actions[0];
            actions.RemoveAt(0);
            return toReturn;
        }
        private State PlanState()
        {
            List<Action> toRemove = new List<Action>();
            while (actions.Count>0 && actions[0].Prerequisite() == State.SKIP)
            {
                actions.RemoveAt(0);
            }
            if (actions.Count > 0)
            {
                return actions[0].Prerequisite(); //WAIT, READY, ABORT
            }
            return State.ABORT;
        }

        internal bool IsExecutable()
        {
            return !PlanState().Equals(State.ABORT);
        }

        internal bool IsReadyToExecute()
        {
            return PlanState().Equals(State.READY);
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
                while (PlanWaiting(ref planState))
                {
                    yield return null;
                }
                totalAction -= actions.Count;
                checkingPlan--;
                if (planState==State.READY)
                {
                    Action next = Next();
                    next.Do();
                    yield return new WaitUntil(()=> next.Done());
                }
                else
                {
                    Debug.Log("Breaking for abort");
                    break;
                }
            }
            _isExecuting = false;
            scheduler.IsWaiting = true;
            scheduler.currentPlan = null;
        }

        bool PlanWaiting(ref State planState)
        {
            planState = PlanState();
            return planState==State.WAIT;
        }
    }

}
