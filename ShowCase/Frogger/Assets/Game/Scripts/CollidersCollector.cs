using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CollidersCollector : MonoBehaviour
{
    public List<Car> cars;
    public List<Log> logs;
    // Start is called before the first frame update
    void Start()
    {
        cars = new List<Car>();
        logs = new List<Log>();
        cars.AddRange(GameObject.FindObjectsOfType<Car>());
        logs.AddRange(GameObject.FindObjectsOfType<Log>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
