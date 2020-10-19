using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public interface IMonoBehaviourSensor
{
    string sensorName { get; set; }
    Brain brain { get; set; }
    MyListString property { get; set; }
    string propertyType { get; set; }
    int operationType { get; set; }
    List<string> collectionElementProperties { get; set; }
    string collectionElementType { get; set; }
    List<int> indexes { get; set; }
    bool ready { get; set; }
    List<IList> propertyValues { get; set; }
    string Map();
    void done();
}
