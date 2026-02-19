---

title: "Sensors and Actuators"

description: "Configuration and usage of sensors and actuators in ThinkEngine, including mappings, triggers, and debugging."

order: 3

tags: ["ThinkEngine", "sensors", "actuators", "Unity", "ASP", "configuration"]

---



# 3. Sensors and Actuators



ThinkEngine connects the game world to the reasoning layer through **Sensors** and **Actuators**.  

Sensors read selected properties from your GameObjects and convert them into ASP facts.  

Actuators write back values decided by the solver, modifying the game scene at runtime.



This section explains how to add and configure these components correctly.



---



## 3.1 SensorConfiguration



A **SensorConfiguration** defines which GameObject properties are exposed to the reasoning layer.  

At runtime, these properties are read at a fixed update frequency and translated into ASP facts (e.g., positions, states, counters, health values, etc.).



### Adding a Sensor



1\. Select the GameObject you want to observe (e.g., `Agent` or `Target`).  

2\. Click **Add Component → Sensor Configuration**.  

3\. Give the configuration a unique name (e.g., `agentSensor`).  

4\. In the property tree, check the properties to expose (e.g., `transform.position.x`, `transform.position.y`, `transform.position.z`).  

5\. Save the configuration.



🧠 **Tip:** When you save the configuration, ThinkEngine automatically adds a `MonoBehaviourSensorsManager` component to the same GameObject.  

This manager handles sensor instantiation at runtime.



### Supported Property Types



- Scalar values (`float`, `int`, `bool`, `string`)  

- Lists and arrays (1D or 2D)  

- Basic structs (e.g., `Vector3` → split into `x`, `y`, `z`)  

- No support for dictionaries (as of current version)



### Aggregation



For properties with historical values, you can choose how the fact is generated:



- `newest` (default)  

- `oldest`  

- `avg` (average)  

- `min` / `max`



**Example fact generated at runtime:**



```prolog
agentSensor_x(agent, objectIndex(43918), "3.14")

```



---



## 3.1.1 Customizing Sensor Predicate Names



When configuring a SensorConfiguration, each mapped property is automatically translated into an ASP predicate (e.g., `agentSensor_x(agent,objectIndex(0),value)`).



However, you can customize the name of this predicate during sensor configuration:



1\. In the SensorConfiguration inspector, select the property.  

2\. Click **Configure**.  

3\. Edit the predicate name that will be used in the generated ASP facts.



**Example:**



| Property | Default fact | Customized predicate |
| --- | --- | --- |
| `transform.position.x` | `agentSensor_x(agent,objectIndex(I),Value)` | `player_pos_x(agent,objectIndex(I),Value)` |
ThinkEngine will then use `player_pos_x` in all generated fact files and reasoning cycles instead of the default.



### Why this matters



1\. Makes your ASP code cleaner and more semantic (e.g., `player_pos_x` instead of `agentSensor_x`).  

2\. Avoids naming conflicts when multiple agents or prefabs share similar property mappings.  

3\. Helps maintain consistency between your `.asp` files and sensor configuration.



Renaming predicates does **not** affect the runtime behavior of the sensor — it only changes the name of the fact passed to the solver.



---



## 3.2 ActuatorConfiguration



An **ActuatorConfiguration** defines which GameObject properties are controlled by the reasoner.  

At runtime, when the solver produces an answer set, the mapped properties are updated accordingly.



### Adding an Actuator



1\. Select the GameObject you want to control (e.g., `Agent`).  

2\. Click **Add Component → Actuator Configuration**.  

3\. Give the configuration a unique name (e.g., `agentActuator`).  

4\. Select the properties to control (e.g., `DesiredX`, `DesiredY`, `DesiredZ` from your `AgentMover` script).  

5\. Optionally, choose a **trigger** to decide when the actuator applies updates (e.g., `Always` or a custom trigger from `ThinkEngineTrigger.cs`).



⚡ **Note:** When you save the configuration, ThinkEngine automatically adds a `MonoBehaviourActuatorsManager` to the same GameObject.



---



## 3.3 Prefab Support



You can attach **SensorConfiguration** and **ActuatorConfiguration** directly to Prefabs as well as scene instances.  

At runtime, ThinkEngine will instantiate and wire them exactly like on scene objects.



**Project Convention:** place Prefabs under `Assets/Prefab`.



- Keep configuration names unique per prefab type to avoid ambiguous mapping at runtime.  

- When debugging, remember that each prefab instance will generate its own `objectIndex(...)` in facts.



---



## 3.4 Linking Properties to ASP Facts



Once sensors and actuators are configured, ThinkEngine automatically generates their ASP mapping.



**Example:**



Given:

- Sensor name: `agentSensor`  

- Actuator name: `agentActuator`  

- Property: `transform.position.x`



The corresponding facts might look like:



```prolog
agentSensor_x(agent, objectIndex(43918), "0.0").

setOnActuator(agentActuator_desiredX(agent, objectIndex(43918), 1.0)).

```



These are the facts and atoms that the solver receives and emits.



---



## 3.5 Multiple Sensors and Actuators



You can add multiple **SensorConfiguration** and **ActuatorConfiguration** components in the same scene:



- A single Brain can consume data from several sensors.  

- A single GameObject can have multiple sensors and actuators, if needed.  

- Multiple GameObjects can each expose their own sensors and actuators to the same Brain.



**Best practice:** Keep sensor and actuator names unique across the project to avoid index collisions and mapping ambiguity.



---



## 3.6 Triggers



Triggers determine **when** sensors are read and actuators are applied.  

ThinkEngine provides a default trigger:



- “When sensors are ready” for brains  

- “Always” for actuators



You can define custom triggers by editing `ThinkEngineTrigger.cs`:



```csharp

public static bool AlwaysTrue() => true;

public static bool Every10Frames() => Time.frameCount % 10 == 0;

```



These functions then appear in the trigger dropdown menu of **Brain** and **Actuator** components.



---



## 3.7 Runtime Behavior



When the game starts, `MonoBehaviourSensorsManager` begins collecting data.



Each reasoning cycle:



1\. Sensor values are translated into ASP facts.  

2\. The solver is called by the Brain.  

3\. If the solver returns actions, Actuators apply the new values to the GameObject properties.



---



## 3.8 Debugging Sensor and Actuator Mapping



- You can preview generated sensor and actuator scripts in the Inspector after saving configurations.  

- If actuators aren’t applying values:

&nbsp; - Check that the actuator property names exactly match what’s used in the `.asp` file.  

&nbsp; - Ensure your Trigger is firing.  

&nbsp; - Verify the solver is producing matching `setOnActuator(...)` atoms.



🧰 **Tip:** Maintaining clean naming conventions like `agentSensor` and `agentActuator` avoids most mapping issues.

![Sensor & Actuator Configuration]( /images/sensor-actuator.png )

