using it.unical.mat.embasp.languages.asp;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public bool done { get; private set; }
        public List<DCSPrefabConfigurator> instantiablePrefabs;

        IActualDCSBrain _dcsBrain;
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

        internal void ContentReady(object content)
        {
            DcsBrain.ContentReady(content, this);
        }

        protected override IEnumerator Init()
        {
            if (DcsBrain != null)
            {
                yield return StartCoroutine(base.Init());
                executor = DcsBrain.GetDCSExecutor(this);
                executorName = "Solver executor " + gameObject.name;

                executionThread = new Thread(() =>
                {
                    Thread.CurrentThread.Name = executorName;
                    Thread.CurrentThread.IsBackground = true;
                    executor.Run();
                });
                executionThread.Start();
            }
        }

        internal string PrefabFacts()
        {
            if (DcsBrain != null)
            {
                return DcsBrain.PrefabFacts(this);
            }
            return "";
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
            throw new NotImplementedException();
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
