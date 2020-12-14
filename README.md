# ThinkEngine
**ThinkEngine** is a [Unity](https://unity.com/) asset that provides an easy integration of an [ASP](https://en.wikipedia.org/wiki/Answer_set_programming) solver in a videogame or other projects developed in Unity. 

ThinkEngine’s goal is to make the integration of a declarative formalism within a videogame transparent from the point of view of a game developer.
Indeed, multiple problems arising from the integration have been solved, for example data mapping, threads synchronizations and triggering events. 

The asset is released as a bunch of .dll files collecting a number of C# scripts. 

## General Architecture

![ThinkEngine's Architecture](ThinkEngine_Architecture.JPG)

The main component of the framework is the **Brain**. A brain is associated with a number of **sensors** and **actuators**, has an ASP encoding file and a triggering event for the reasoning task. 
It starts an auxiliary thread running a **Solver Executor** that, every time that the triggering conditions are met, requests the brain's sensors data and feed in the ASP solver with these and the encoding file. 
Both sensors and actuators are generated (at run-time) basing on some configurations defined at design-time. 
**Sensors** read data from the game world (in the [*LateUpdate*](https://docs.unity3d.com/Manual/ExecutionOrder.html) event!) every *X* frames where *X* can possibly change during the game depending on the amount of sensors instantiated and on the frame rate.
*X* is computed at each frame by the **Sensors Manager**. The manager is also in charge of retrieving the sensors’ ASP mapping and of returning this value to some requesting brain. 
**Actuators** are provided by the **Actuators Manager** with the decisions coming from the reasoner layer. Every time that a **Solver Executor** notifies the manager that an Answer Set (AS) is available, the following happens:
* the manager checks if some actuator of the corresponding brain has to be applied (checking the triggering function chosen at design-time);
* the actuators that can be applied are provided with this AS;
* each actuator checks if the AS contains some literal corresponding to its properties;
* if there is some of them, the actual value of the logical assertion is set on the game logic.

Both sensors and actuators managers run in the main Unity thread. 
When a Solver Executor (running in an auxiliary thread) needs input facts, it requests these information first to the sensors manager and later to the actuators one. 
At each request it WAITS on a queue to receive what it needs. The two managers reply to these requests in the *Update* event. 
Since sensors are updated in the *LateUpdate* event while the Sensors Manager retrieves the sensors’ mapping in the *Update* one, there is an automatic synchronization of the main thread and the auxiliary ones.

## Reflection Layer
This layer is in charge of translating back and forth from object data structure to logical assertion. Both Sensors and Actuators need to be configured at desig-time via a configuration component. Once that the configuration has been saved, it can be associated to some **Brain**. A brain is associated with a some sensor and actuator configurations, an ASP encoding file and a triggering condition for the reasoning task (more details will be given in next paragraphs).

### Sensors
Principal involved class: SensorConfiguration, MonoBehaviourSensorsManager, MonoBehaviourSensor
#### SensorConfiguration
When some information of a GameObject are needed as input facts of an ASP program, you need to add, at design-time, a SensorConfiguration component to the GameObject. Once you choose the name of the configuration (that HAS to be UNIQUE in the game), you can graphically explore the properties hierarchy of the GameObject and you can choose the properties you want to feed in input of the solver. For each property, the ThinkEngine stores the last 100 read values. While configuring the sensor, you can choose which aggregation function has to be applied when generating the logical assertion (e.g. min, max, avg, newest value, oldest value). For what concerns complex data structures, at the moment, we support only **List** and **Bidimensional Arrays** of (non basic but non generics) objects.

#### MonoBehaviourSensorsManager
At design-time, when a sensor configuration is saved, a **MonoBehaviourSensorsManager** is automatically added to the GameObject. 
At design-time, the manager is in charge of notifing the active brains of all the updates the occurs to the configuration of it owner GameObject.
At run-time, instead, it manages the actual instantiation of the sensors. For each configuration, it instantiates a sensor for each simple property and a sensor for each element of a complex data structure. During the game, if the size of a complex data structure increases, the manager instantiates as many sensors as many new elements are added to the data structure. 

#### MonoBehaviourSensor

### Actuators
Principal involved class: ActuatorConfiguration, MonoBehaviourActuatorsManager, MonoBehaviourActuator
