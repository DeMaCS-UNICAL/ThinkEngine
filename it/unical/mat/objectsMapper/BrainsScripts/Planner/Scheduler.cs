using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Planner
{

    internal class Scheduler : MonoBehaviour
    {
        Queue<Action> plan;

        public bool IsWaiting { get; internal set; }

            
        internal void CleanPlan()
        {
            plan = new Queue<Action>();
            StopCoroutine(ApplyPlan());
        }

        internal void NewPlan(List<Action> actions)
        {
            CleanPlan();
            for (int i = 0; i < actions.Count; i++)
            {
                AppendAction(actions[i]);
            }
            StartCoroutine(ApplyPlan());
        }

        internal void AppendAction(Action action)
        {
            plan.Enqueue(action);
        }

        internal void AppendPlan(List<Action> actions)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                plan.Enqueue(actions[i]);
            }
        }

        private bool ActionAvailable()
        {
            return plan.Count > 0;
        }
            
        private Action NextAction()
        {
            if (ActionAvailable())
            {
                return plan.Dequeue();
            }
            return null;
        }
          
        private IEnumerator ApplyPlan()
        {
            while (true)
            {
                yield return new WaitUntil(()=>ActionAvailable());
                Action next = NextAction();
                IsWaiting = !ActionAvailable();
                if(next == null)
                {
                    continue;
                }
                yield return new WaitUntil(()=>next.Prerequisite());
                next.Do();
                yield return new WaitUntil(()=>next.Done());
            }
        }

    }
}
