using System.Collections.Generic;
using static Mappers.OperationContainer;

namespace Structures
{
    interface IInfoAndValue
    {
        object GetValuesForPlaceholders();
    }
}