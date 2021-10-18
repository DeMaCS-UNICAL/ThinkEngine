using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Planner
{
    public abstract class Action : ScriptableObject
    {
        public abstract bool Prerequisite();
        public abstract void Do();
        public abstract bool Done();
    }
}
