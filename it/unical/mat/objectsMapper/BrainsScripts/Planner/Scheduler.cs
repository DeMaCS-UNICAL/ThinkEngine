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


        internal void NewPlan(Plan plan)
        {
            if(currentPlan!=null && currentPlan.IsExecuting)
            {
                if (plan.IsExecutable())
                {
                    StopCoroutine(currentPlan.ApplyPlan(this));
                }
                else
                {
                    return;
                }
            }
            currentPlan = plan;
            StartCoroutine(currentPlan.ApplyPlan(this));
            IsWaiting = false;
        }
    }
}
