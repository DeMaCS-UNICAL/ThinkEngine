using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NewMappers.NewOperationContainer;

namespace NewMappers
{
    interface NeedingAggregatesMapper
    {
        List<NewOperation> OperationList();
        Type GetAggregationTypes();
        int GetAggregationSpecificIndex();
    }
}
