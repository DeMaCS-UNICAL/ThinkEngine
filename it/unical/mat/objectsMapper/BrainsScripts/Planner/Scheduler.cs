using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Planner
{

    public class Scheduler : MonoBehaviour
    {
        Plan currentPlan;
        private bool _isWaiting;
        Coroutine planExecutionCoroutine;
        public int ResidualActions
        {
            get
            {
                if (currentPlan == null)
                {
                    return 0;
                }
                return currentPlan.actions.Count;
            }
        }
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
                    if (planExecutionCoroutine != null)
                    {
                        StopCoroutine(planExecutionCoroutine);
                    }
            }
            currentPlan = plan;
            planExecutionCoroutine = StartCoroutine(currentPlan.ApplyPlan(this));
            return true;
        }
    }
}
