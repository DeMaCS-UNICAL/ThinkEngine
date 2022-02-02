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
        Plan currentPlan;
        private bool _isWaiting;
        Coroutine currentCoroutine;
        public bool IsWaiting {
            get
            {
                return _isWaiting;
            }
            internal set
            {
                _isWaiting = value;
                if (_isWaiting)
                {
                    GetComponent<PlannerBrainsCoordinator>().SchedulerIsWaiting();
                }
            }
        }


        internal bool NewPlan(Plan plan)
        {
            if(currentPlan!=null && currentPlan.IsExecuting)
            {
                if (plan.IsReadyToExecute())
                {
                    StopCoroutine(currentCoroutine);
                }
                else
                {
                    return false;
                }
            }
            
            currentPlan = plan;
            currentCoroutine = StartCoroutine(currentPlan.ApplyPlan(this));
            IsWaiting = false;
            return true;
        }
    }
}
