using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper
{
    [ExecuteInEditMode]
    public class AbstractConfigurator: MonoBehaviour
    {
        [SerializeField]
        protected IManager manager;
        [SerializeField]
        protected List<AbstractConfiguration> gOConfigurations;
        [SerializeField]
        protected List<string> localNames;
        [SerializeField]
        protected string goName;

        protected void Awake()
        {
            Debug.Log("awaken " + manager);
            if (goName is null)
            {
                goName = gameObject.name;
            }
            checkGOName();
            checkConfigurations();
        }

        private void checkConfigurations()
        {
            localNames = new List<string>();
            gOConfigurations = new List<AbstractConfiguration>();
            foreach (AbstractConfiguration c in manager.getConfigurations())
            {
                if (c.gOName.Equals(goName))
                {
                    gOConfigurations.Add(c);
                    localNames.Add(c.configurationName);
                }
            }
        }

        void Update()
        {
            checkGOName();
            checkConfigurations();
        }

        private void checkGOName()
        {
            if (!goName.Equals(gameObject.name))
            {
                foreach (AbstractConfiguration c in manager.getConfigurations())
                {
                    if (c.gOName.Equals(goName))
                    {
                        c.gOName = gameObject.name;
                    }
                }
                goName = gameObject.name;
            }
        }

        public virtual AbstractConfiguration newConfiguration(string n, string chosenGO)
        {
            return manager.newConfiguration(n,chosenGO);
        }
        

        internal AbstractConfiguration findConfiguration(string v)
        {
            foreach(AbstractConfiguration c in gOConfigurations)
            {
                if (c.configurationName.Equals(v))
                {
                    return c;
                }
            }
            return null;
        }

        internal void deleteConfiguration(string v)
        {
            Debug.Log("deleting " + v);
            int i = 0;
            for (; i < gOConfigurations.Count; i++)
            {
                if (gOConfigurations[i].configurationName.Equals(v))
                {
                    break;
                }
            }
            if (i < gOConfigurations.Count)
            {
                Debug.Log(gOConfigurations[i].configurationName);
                onDeleting(gOConfigurations[i]);
                gOConfigurations.RemoveAt(i);
                localNames.Remove(v);
            }
            manager.delete(v);
        }

        internal void addConfiguration(AbstractConfiguration abstractConfiguration)
        {
            deleteConfiguration(abstractConfiguration.configurationName);
            gOConfigurations.Add(abstractConfiguration);
            if (!localNames.Contains(abstractConfiguration.configurationName))
            {
                localNames.Add(abstractConfiguration.configurationName);
            }
            manager.addConfiguration(abstractConfiguration);
        }

        

        internal List<string> configurationNames()
        {
            return localNames;
        }

        internal List<string> generalUsedNames()
        {
            return manager.getUsedNames();
        }

        internal virtual string onSaving() { return ""; }
        internal virtual void onDeleting(AbstractConfiguration c) { }
    }
}
