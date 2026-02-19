---
title: "Brain"
description: "How Brains work in ThinkEngine, including configuration, linking with sensors and actuators, supported solvers, reactive vs planner logic, and ThinkEngineUtility."
order: 4
tags: ["ThinkEngine", "brain", "planner", "reactive", "ASP", "Unity"]
---

# 4. Brain

A **Brain** is the central component that connects the game world to the reasoning engine.

While Sensors and Actuators handle reading and writing GameObjects properties, the Brain is responsible for **reasoning**:

- It collects sensor data  
- It triggers the ASP solver  
- It applies the solver’s result through actuators or action plans

ThinkEngine provides two types of brains:

- 🟡 **Reactive Brain** – for immediate, per-frame decisions  
- 🔵 **Planner Brain** – for generating and executing high-level plans made of actions

---

## 4.1 Where to Place a Brain

A Brain can be attached to:

- The agent itself (e.g., if it only controls one object)  
- A dedicated GameObject (e.g., `BrainHost`), which controls multiple objects through their sensor and actuator configurations

---

## 4.1.1 Supported Solvers

ThinkEngine supports multiple ASP solvers, which can be selected directly from the Brain inspector under:

```plaintext
Choose the solver ▼
```

### Currently supported solvers

| Solver | Description |
| --- | --- |
| **DLV2** | Designed to solve complex declarative reasoning problems. |
| **Clingo** | Combines grounding and solving in a single executable. It’s fast, extensible and widely used for modeling and solving combinatorial problems. |
| **IDLV²** | An incremental and interactive version of DLV2, allowing rules and facts to be added or removed dynamically while maintaining the reasoning state. |
**How to use**

- Place the solver binary inside  
  `Assets/StreamingAssets/ThinkEngineer/ThinkEngine/lib/`
- In the Brain inspector, pick the solver from the dropdown menu.

⚠ Ensure the binary has execution permissions.  
Only one solver is used per brain at a time, but different brains can use different solvers simultaneously.

---

## 4.1.2 Brains on Prefab + currentBrainID

Brains can also be attached directly to a Prefab.  
When multiple instances of the same prefab are present, use the fact:

```plaintext
currentBrainID(ID)
```

to disambiguate which sensor facts belong to the brain that triggered the reasoning.

In your `.asp` rules, join decision with the matching `objectIndex` of that brain:

```prolog
% Example: only act on the actuator index that belongs to the current brain
setOnActuator(agentActuator_desiredX(agent, objectIndex(I), 1)) :-
    currentBrainID(I),
    objectIndex(agentActuator, I).
```

This guarantees that each prefab instance’s Brain only reads/writes its own data, avoiding cross-instance interference.

**Note:** The project convention expects Prefab under `Assets/Prefab`.  
If you instantiate at runtime, ensure the prefab carries the Brain/Sensor/Actuator components you need.

---

## 4.2 Linking Sensors and Actuators to a Brain

After adding a Brain component:

1. In the Inspector, select the previously created **SensorConfiguration** components in “All the available Sensor Configurations”.  
2. For Reactive Brain only, also select “All the available Actuator Configurations”.  
3. Choose the Trigger that will control when reasoning happens (e.g., *When Sensors are ready* by default).  
4. Enable **Debug Mode** if you want to inspect reasoning cycles in the Console.

---

## 4.3 Generating the ASP Template

Once sensors and actuators are linked, you can press in the Brain inspector:

```plaintext
👉 Generate ASP-like file template
```

This generates a `.asp` file in:

```plaintext
Assets/ThinkEngineer/ThinkEngine/Templates/
```

The generated file shows:

- Commented sensor mappings, e.g.:

```prolog
% agentSensor_x(agent,objectIndex(Index),Value).
% agentSensor_y(agent,objectIndex(Index),Value).
```

- Actuator rules skeleton, e.g.:

```prolog
setOnActuator(agentActuator_desiredX(agent,objectIndex(Index),Value)) :-
  objectIndex(agentActuator, Index), ...
```

This file is meant as a starting point for your logic — not something automatically executed.  
You can duplicate, rename, and edit it to express your own decision rules.

---

## 4.4 Choosing Your ASP Files

ThinkEngine requires your runtime `.asp` files to be placed inside:

```plaintext
Assets/StreamingAssets/ThinkEngineer/ThinkEngine/
```

This is the only folder from which the solver actually reads your logic at runtime.

⚠ Any `.asp` file outside this path will not be used during reasoning.

---

## 4.5 Reactive Brain

The **Reactive Brain** is the simpler of the two:

- It runs the reasoning task every time the trigger condition is met.

The solver produces atoms of the form:

```prolog
setOnActuator(agentActuator_desiredX(agent,objectIndex(43918),1.0)).
```

ThinkEngine parses these and applies the values directly to mapped Unity properties (e.g. `AgentMover.desiredX`).

### Use cases

- Movement  
- Chasing  
- Toggling lights  
- Reacting to the environment

### Advantages

- Easy to set up  
- Works well for low-level behavior  
- High frequency, short reasoning cycles  
- No explicit action planning required

---

## 4.6 Planner Brain

The **Planner Brain** is designed for more complex scenarios:

- It does not use actuators.  
- Instead, the solver produces plans in terms of actions:

```prolog
applyAction(1,"FaceTargetAction").
actionArgument(1,"agentName","Agent").
applyAction(2,"StepForwardAction").
actionArgument(2,"agentName","Agent").
actionArgument(2,"step","1").
```

Each action corresponds to a C# class you implement that inherits from `ThinkEngine.Planning.Action`:

```csharp
public class StepForwardAction : Action 
{
    public string agentName;
    public string step;

    public override void Do() 
    { 
        /* move agent forward */ 
    }

    public override State Done() 
    { 
        /* check completion */ 
    }

    public override State Prerequisite() 
    { 
        /* check readiness */ 
    }
}
```

The `Action` class is instantiated at runtime by ThinkEngine using `ScriptableObject.CreateInstance()`.

The plan is executed by ThinkEngine’s scheduler:

- First action is executed if its `Prerequisite()` is `READY`.  
- Once `Done()` returns `READY`, the next action starts.  
- `WAIT` or `ABORT` states can delay or cancel execution.

### Use cases

- Navigation sequences  
- Combos  
- Missions  
- Scripted behaviors

### Advantages

- Declarative and modular action definition  
- Planner can consider multiple steps ahead

You can define multiple custom Actions and reference them from `.asp` files using:

```prolog
applyAction(1, "StepForwardAction").
actionArgument(1, "agentName", "Agent").
```

---

## 4.6.1 Planner Actions

Planner Brains work by generating and executing plans — i.e., ordered sequences of actions.  
Each action in a plan corresponds to a C# class that inherits from `ThinkEngine.Planning.Action`.

---

## 4.6.2 Multiple Planner Brain and Priority Scheduling

ThinkEngine allows you to attach multiple Planner Brains to the same GameObject.  
This is useful when you want to layer different planning concerns (e.g., navigation, combat, interaction), each producing its own plan.

A per-GameObject scheduler coordinates execution and ensures only the higher-priority plans run at a time.

### How priority is resolved

- Each Planner Brain has an internal Brain ID.  
- Ordering rule: if `id < id1`, then the brain with `id` has higher priority than the brain with `id1`.  
- When multiple plans are available, the scheduler selects the plan from the highest-priority brain and aborts any lower-priority plan currently running if necessary.

### Runtime behavior

1. Each Planner Brain submits its latest plan to the per-object scheduler.  
2. The scheduler evaluates priorities (by Brain ID).  
3. The selected plan is executed action by action (`Prerequisite()` → `Do()` → `Done()`).

### Best practices

- It’s perfectly fine to have multiple Planner Brains active on the same object; the scheduler will serialize execution by priority.  
- Keep Planner Brains focused (e.g., “Path Planning”, “Tactical Planning”) to make plans modular and easier to debug.  
- If you also use a Reactive Brain on the same object, avoid conflicts by disabling actuators or using a mode toggle so reactive updates don’t fight the planner’s action.

---

## 4.7 ThinkEngineUtility

`ThinkEngineUtility` is a service GameObject automatically created when you add your first Brain.  
It is essential for reasoning execution:

- Manages reasoning scheduling and threading  
- Coordinates triggers and solver I/O  
- Tracks global state between brains  
- Spawns the DLV2 solver process

🧠 You usually don’t need to modify this object manually.  
Deleting it will break solver execution.

**Best practice:** keep one persistent `ThinkEngineUtility` in your scene or persistent manager.


![Brain Inspector]( /images/brain.png )