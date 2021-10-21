using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Planner
{
    class PlannerBrainsCoordinator : MonoBehaviour
    {
        private Scheduler scheduler;
        private int priorityExecuting;
        private PlannerBrain waiting;
        internal void SetPriority(int previousPriority, int newPriority, PlannerBrain brain)
        {
            if(!ValidPriority(newPriority))
            {
                throw new Exception("Planner priority should be between 1 and the number of brains associated to the gameobject");
            }
            if (ValidPriority(previousPriority) &&  previousPriority != newPriority) 
            {
                foreach (PlannerBrain currentBrain in GetComponents<PlannerBrain>())
                {
                    if (brain!=currentBrain && currentBrain.Priority == newPriority)
                    {
                        currentBrain.Priority = previousPriority;
                    }
                }
            }
        }

        private bool ValidPriority(int priority)
        {
            if(priority<0 || priority >= NumberOfBrains())
            {
                return false;
            }
            return true;
        }

        internal int NumberOfBrains()
        {
            return GetComponents<PlannerBrain>().Count();
        }

        internal void PlanReady(PlannerBrain brain)
        {
            if (priorityExecuting < brain.Priority)
            {
                priorityExecuting = brain.Priority;
                scheduler.NewPlan(brain.GetPlan());
            }
        }
        void Start()
        {
            scheduler = GetComponent<Scheduler>();
        }
        

        internal void RemoveBrain(int priority)
        {
            foreach(PlannerBrain brain in GetComponents<PlannerBrain>()) 
            {
                if (brain!=null && brain.Priority > priority)
                {
                    brain.Priority--;
                }
            }
        }
    }
}
