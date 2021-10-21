using Editors;
using it.unical.mat.embasp.languages.asp;
using Planner;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private Dictionary<int, List<KeyValuePair<string, string>>> _actionParameters;
        private Dictionary<int, List<KeyValuePair<string, string>>> ActionParameters
        {
            get
            {
                if (_actionParameters == null)
                {
                    _actionParameters = new Dictionary<int, List<KeyValuePair<string, string>>>();
                }
                return _actionParameters;
            }
        }
        private Dictionary<int, Action> _actions;
        private Dictionary<int, Action> Actions
        {
            get
            {
                if (_actions == null)
                {
                    _actions = new Dictionary<int, Action>();
                }
                return _actions;
            }
        }
        [SerializeField,HideInInspector]
        private int _priority=-1;
        List<Action> plan;
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
        bool answerSetAvailable;
        AnswerSet currentAnswerSet;

        void Reset()
        {
            if (Priority == -1)
            {
                Priority = GetComponents<PlannerBrain>().Count() - 1;
            }
        }
        internal void AnswerSetAvailable(AnswerSet answerSet)
        {
            answerSetAvailable = true;
            currentAnswerSet = answerSet;
        }


        private List<Action> Parse(AnswerSet answerSet)
        {
            foreach(string literal in answerSet.GetAnswerSet())
            {
                if (literal.StartsWith("applyAction("))
                {
                    ParseApplyActionLiteral(literal);
                }
                else if (literal.StartsWith("actionArgument("))
                {
                    ParseActionArgumentLiteral(literal);
                }
            }
            AssociateActionsToParameters();
            List<Action> toReturn = new List<Action>();
            toReturn = Actions.Values.ToList();
            toReturn.Sort();
            return toReturn;
        }

        private void AssociateActionsToParameters()
        {
            foreach(int order in ActionParameters.Keys)
            {
                if (Actions.ContainsKey(order))
                {
                    foreach (KeyValuePair<string, string> parameter in ActionParameters[order])
                    {
                        PropertyInfo property = Actions[order].GetType().GetProperty(parameter.Key);
                        if (property != null)
                        {
                            Type propertyType = property.PropertyType;
                            property.SetValue(Actions[order], Convert.ChangeType(parameter.Value, propertyType));
                        }
                        else
                        {
                            Debug.Log("Property " + parameter.Key + " not found in class " + Actions[order].GetType().Name);
                        }
                    }
                }
                else
                {
                    Debug.Log("Action with order of execution " + order + " not set.");
                }
            }
        }

        private void ParseActionArgumentLiteral(string literal)
        {
            string partial = literal.Remove(0, "actionArgument(".Count());
            string toParse = partial.Substring(0, partial.IndexOf(","));
            bool converted = int.TryParse(toParse, out int order);
            if (converted)
            {
                partial = partial.Remove(0, partial.IndexOf(",")+1);
                string parameterName = partial.Substring(0, partial.IndexOf(",")).Trim('\"');
                partial = partial.Remove(0, partial.IndexOf(",")+1);
                string parameterValue = partial.Substring(0, partial.IndexOf(")")).Trim('\"');
                ActionParameters[order].Add( new KeyValuePair<string, string>(parameterName,parameterValue));
            }
        }

        private void ParseApplyActionLiteral(string literal)
        {
            string partial = literal.Remove(0, "applyAction(".Count());
            string toParse = partial.Substring(0, partial.IndexOf(","));
            bool converted = int.TryParse(toParse, out int order);
            if (converted)
            {
                partial = partial.Remove(0, partial.IndexOf(",")+1);
                string actionClass = partial.Substring(0, partial.IndexOf(")")).Trim('\"');
                Actions[order] = ScriptableObject.CreateInstance(actionClass) as Action;
                ActionParameters[order] = new List<KeyValuePair<string, string>>();
            }
        }
        internal override void GenerateFile()
        {
            base.GenerateFile();
            using (StreamWriter fs = File.AppendText(ASPFileTemplatePath))
            {
                fs.Write("% Predicates for Action invokation.\n");
                fs.Write("% applyAction(OrderOfExecution,ActionClassName).\n");
                fs.Write("% actionArguments(OrderOfExecution,ArgumentName, ArgumentValue).\n");
            }
        }
        internal List<Action> GetPlan()
        {
            return plan;
        }
        protected override IEnumerator Init()
        {
            yield return StartCoroutine(base.Init());
            executor = new PlannerExecutor(this);
            string GOname = gameObject.name;
            executionThread = new Thread(() =>
            {
                Thread.CurrentThread.Name = "Solver executor " + GOname;
                Thread.CurrentThread.IsBackground = true;
                executor.Run();
            });
            executionThread.Start();
        }
        protected override void Update()
        {
            base.Update();
            if (answerSetAvailable)
            {
                answerSetAvailable = false;
                plan = Parse(currentAnswerSet);
                if (plan.Count != 0)
                {
                    Coordinator.PlanReady(this);
                }
            }
        }

        public int CompareTo(PlannerBrain other)
        {
            return -Priority.CompareTo(other.Priority);
        }
    }
}
