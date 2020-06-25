using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CollidersCollector : MonoBehaviour
{
    public List<Car> cars;
    public List<CollidableObject> logs;
    // Start is called before the first frame update
    void Start()
    {
        cars = new List<Car>();
        cars.AddRange(GameObject.FindObjectsOfType<Car>());
        logs = new List<CollidableObject>();
        foreach(CollidableObject o in GameObject.FindObjectsOfType<CollidableObject>())
        {
            if (o.isLog)
            {
                logs.Add(o);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
