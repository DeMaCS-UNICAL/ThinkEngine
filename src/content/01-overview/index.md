---

title: "Conceptual Overview"

description: "High-level introduction to ThinkEngine, its architecture, reasoning paradigms, and triggers."

order: 1

tags: ["ThinkEngine", "overview", "architecture", "reasoning", "Unity"]

---



# 1. Conceptual Overview



## 1.1 What is ThinkEngine



ThinkEngine is a Unity asset designed to integrate automated reasoning modules into video games and other interactive Unity-based applications.  

Its core purpose is to provide declarative decision-making logic, allowing developers to separate what should happen (rules, goals) from how it happens (imperative game logic).



ThinkEngine was originally developed to support **ASP (Answer Set Programming)** but can also integrate other reasoning formalisms (e.g., **PDDL**) with minimal adaptation.



In practice:



- Unity controls the game world (objects, physics, input, rendering…)

- ThinkEngine observes the world through sensors

- It passes data to an external reasoning solver (**DLV2**)

- It receives decisions in the form of actions or plans

- Actuators apply these decisions back to Unity



This approach is particularly suitable for:

- Autonomous agents

- Complex game logic

- AI-driven NPCs

- Multi-agent systems

- Serious games



---



## 1.2 ThinkEngine Architecture



ThinkEngine is built on a layered architecture, where each layer has a well-defined responsibility:



- 🛰 **Sensor Layer** – maps Unity game state to logical facts.  

- 🧠 **Reasoning Layer** – sends facts to an ASP solver together with declarative rules (`.asp`), then processes the solver’s answers.  

- ⚡ **Actuator Layer** – translates the solver’s output into concrete modifications to the Unity world.  

- 🔁 **Reflection Layer** – handles the back-and-forth translation between Unity object structures and logical assertions.



### Reasoning Cycle



1\. **SensorConfiguration components** read selected GameObject properties.  

2\. The values are translated into ASP facts.  

3\. The **DLV2 solver** performs logical reasoning using the provided `.asp` files.  

4\. ThinkEngine receives the results:

&nbsp;  - For a **Reactive Brain**, it generates immediate actuator outputs.

&nbsp;  - For a **Planner Brain**, it generates a plan, i.e., an ordered set of actions.

5\. Actuators apply the computed decisions to the game scene.



---



## 1.3 Reactive Brain vs Planner Brain



ThinkEngine supports two complementary reasoning paradigms, each suitable for different use cases:



| Feature | Reactive Brain 🧠⚡ | Planner Brain 🧠📝 |
| --- | --- | --- |
| Reasoning style | Immediate / event-driven | Plan generation |
| Output | Actuator values | Ordered plan (sequence of actions) |
| Uses sensors | ✅ Yes | ✅ Yes |
| Uses actuators | ✅ Yes | ❌ No |
| Requires C# actions | Not required | Required (`Action` class) |
| Execution | Real-time | Scheduled plan execution |
| Typical use cases | NPC behaviors, movement, simple reactions | Complex tasks, high-level decision making |
**Examples**



- \*Reactive Brain\*: “Always move in the +X direction” – computed each frame.  

- \*Planner Brain\*: “Face the target, then walk five steps” – computed once as a plan and then executed step by step.



---



## 1.4 Triggering Reasoning



**Triggers** define when ThinkEngine performs a reasoning cycle.  

Triggers are static, parameterless C# functions that return a `bool`.



### Example



```csharp

public static class ThinkEngineTrigger

{

&nbsp;   public static bool AlwaysTrue() => true;

&nbsp;   public static bool Every10Frames() => Time.frameCount % 10 == 0;

}

```



\*Default: When Sensors are ready.\*



These functions automatically appear in the configuration dropdowns for **Brains** and **Actuators**, allowing developers to specify:



- when a Brain should start reasoning

- when actuators should be applied



### Built-in trigger options



- 🕒 **When sensors are ready** → runs reasoning as soon as the sensors update.  

- ♾ **Always** → actuators are applied on every frame.



### Custom triggers allow for:



- Periodic reasoning  

- Event-based reasoning  

- Temporal control over decision-making


