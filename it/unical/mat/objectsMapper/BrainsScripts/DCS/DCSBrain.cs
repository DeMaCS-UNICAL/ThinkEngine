using it.unical.mat.embasp.languages.asp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.Specialitations.ASP;
using UnityEngine;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.DCS
{
    internal class DCSBrain : Brain
    {
        public bool done { get; private set;}
        public List<DCSPrefabConfigurator> instantiablePrefabs = new List<DCSPrefabConfigurator>();
        HashSet<string> factsToAdd;
        List<string> tempToAdd;
        List<string> tempToDelete;
        public int numberOfAnswerSet;

        internal string FactsForExecutor
        {
            get
            {
                return string.Join(Environment.NewLine, factsToAdd)+Environment.NewLine;
            }
        }
        string prefabFacts="";
        IActualDCSBrain _dcsBrain;
        [SerializeField,HideInInspector]
        internal string initAIFile;

        IActualDCSBrain DcsBrain
        {
            get
            {
                if (_dcsBrain == null)
                {
                    if (FileExtension.Equals("asp"))
                    {
                        _dcsBrain = new ASPDCSBrain();
                    }
                }
                return _dcsBrain;
            }
        }
        void Awake()
        {
            foreach(DCSPrefabConfigurator configurator in instantiablePrefabs)
            {
                configurator.Init();
            }
        }
        protected override IEnumerator Init()
        {
            if (DcsBrain != null)
            {
                factsToAdd = new HashSet<string>();
                tempToAdd = new List<string>();
                tempToDelete = new List<string>();
                yield return StartCoroutine(base.Init());
                executor = DcsBrain.GetDCSExecutor(this);
                Utility.AddPrefabInstantiator();
                executorName = "Solver executor " + gameObject.name;
                prefabFacts = DcsBrain.PrefabFacts(this);
                executionThread = new Thread(() =>
                {
                    Thread.CurrentThread.Name = executorName;
                    Thread.CurrentThread.IsBackground = true;
                    executor.Run();
                });
                executionThread.Start();
            }
        }


        internal void ContentReady(object content)
        {
            DcsBrain.ContentReady(content, this);
        }

        internal void FactsToAdd(string temp)
        {
            tempToAdd.Add(temp);
        }

        internal void FactsToDelete(string temp)
        {
            tempToDelete.Add(temp);

        }

        internal void FactsToUpdate(string value1, string value2)
        {
            tempToDelete.Add(value1);
            tempToAdd.Add(value2);
        }
        internal void AddFact(string fact)
        {
            if (!factsToAdd.Contains(fact))
            {
                factsToAdd.Add(fact+".");
            }
        }
        internal void DeleteFact(string fact)
        {
            string toRemve=fact+".";
            if (factsToAdd.Contains(toRemve))
            {
                factsToAdd.Remove(toRemve);
            }
        }
        internal void ApplyChangesToFacts()
        {
            foreach(string fact in tempToDelete)
            {
                DeleteFact(fact);
            }
            tempToDelete.Clear();
            foreach (string fact in tempToAdd)
            {
                AddFact(fact);
            }
            tempToAdd.Clear();
        }
        internal string PrefabFacts()
        {
            return prefabFacts;
        }

        protected override HashSet<string> SupportedFileExtensions
        {
            get
            {
                return new HashSet<string> { "asp" };
            }
        }

        protected override string SpecificFileParts()
        {
            if (DcsBrain != null)
            {
                return DcsBrain.SpecificFileParts();
            }
            string toReturn = "%For ASP programs:\n" + new ASPDCSBrain().SpecificFileParts();
            return toReturn;
        }


        internal override string ActualSensorEncoding(string sensorsAsASP)
        {
            if (DcsBrain != null)
            {
                return DcsBrain.ActualSensorEncoding(sensorsAsASP);
            }
            return sensorsAsASP;
        }

    }
}
