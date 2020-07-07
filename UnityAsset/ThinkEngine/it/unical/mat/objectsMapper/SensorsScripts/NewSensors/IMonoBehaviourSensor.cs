using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IMonoBehaviourSensor
{
    string sensorName { get; set; }
    Brain brain { get; set; }
    string path { get; set; }
    string propertyType { get; set; }
    int operationType { get; set; }
    string collectionElementProperty { get; set; }
    string collectionElementType { get; set; }
    List<int> indexes { get; set; }
    bool ready { get; set; }
    IList propertyValues { get; set; }
    string Map();
    void done();
}
