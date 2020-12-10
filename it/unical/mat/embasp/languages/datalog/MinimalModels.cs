using it.unical.mat.parsers.datalog;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

namespace it.unical.mat.embasp.languages.datalog
{
    using Output = it.unical.mat.embasp.@base.Output;

    public abstract class MinimalModels : Output, IDatalogDataCollection
    {
        protected internal ISet<MinimalModel> minimalModels;
        
        public MinimalModels(string @out) : base(@out) { }

        public MinimalModels(string @out, string err) : base(@out, err) { }

        public override object Clone() => base.Clone();

        public virtual ISet<MinimalModel> Minimalmodels
        {
            get
            {
                if (minimalModels == null)
                {
                    minimalModels = new HashSet<MinimalModel>();
                    Parse();
                }

                return ImmutableHashSet.Create<MinimalModel>(minimalModels.ToArray());
            }
        }

        public virtual string MinimalModelsAsString => output;

        public void AddMinimalModel(MinimalModel minMod)
        {
            minimalModels.Add(minMod);
        }


    }
}