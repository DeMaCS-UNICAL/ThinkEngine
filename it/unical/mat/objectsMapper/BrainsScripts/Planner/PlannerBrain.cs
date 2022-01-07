using it.unical.mat.embasp.languages.asp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Planner
{
    [RequireComponent(typeof(Scheduler)),RequireComponent(typeof(PlannerBrainsCoordinator))]
    class PlannerBrain:Brain,IComparable<PlannerBrain>
    {
        private PlannerBrainsCoordinator _coordinator;
        internal PlannerBrainsCoordinator Coordinator
        {
            get
            {
                if (_coordinator == null)
                {
                    _coordinator = GetComponent<PlannerBrainsCoordinator>();
                }
                return _coordinator;
            }
        }

        [SerializeField,HideInInspector]
        private int _priority=-1;
        Plan plan;
        [HideInInspector]
        public int Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                int previous = _priority;
                try
                {
                    _priority = value;
                    Coordinator.SetPriority(previous, _priority, this);
                }
                catch(Exception e)
                {
                    _priority = previous;
                    throw e;
                }
            }
        }
        IActualPlannerBrain planner;
        protected override HashSet<string> SupportedFileExtensions
        {
            get
            {
                return new HashSet<string> { "asp" };
            }
        }
        internal void PlanAvailable(object plan)
        {
            planner.NewPlanAvailable(plan);
        }

        internal Plan GetPlan()
        {
            return plan?? new Plan();
        }

        void Reset()
        {
            if (Priority == -1)
            {
                Priority = GetComponents<PlannerBrain>().Count() - 1;
            }
        }
        protected override void  Start()
        {
            if (FileExtension.Equals("asp"))
            {
                planner = new ASPPlannerBrain();
            }
            base.Start();
        }
        protected override IEnumerator Init()
        {
            if (planner != null)
            {
                yield return StartCoroutine(base.Init());
                executor = planner.GetPlannerExecutor(this);
                string GOname = gameObject.name;
                executionThread = new Thread(() =>
                {
                    Thread.CurrentThread.Name = "Solver executor " + GOname;
                    Thread.CurrentThread.IsBackground = true;
                    executor.Run();
                });
                executionThread.Start();
            }
        }
        protected override void Update()
        {
            if (Application.isPlaying && planner!=null)
            {
                base.Update();
                if (planner.IsNewPlanAvailable())
                {
                    plan = planner.GetNewPlan();
                    if (plan!=null)
                    {
                        Coordinator.PlanReady(this);
                    }
                }
            }
        }

        public int CompareTo(PlannerBrain other)
        {
            return -Priority.CompareTo(other.Priority);
        }

        internal override string ActualSensorEncoding(string sensorsAsASP)
        {
            return planner.ActualSensorEncoding(sensorsAsASP);
        }

        protected override string SpecificFileParts()
        {
            return planner.SpecificFileParts();
        }
    }
}
