---

title: "Installation and Setup"

description: "How to install and set up ThinkEngine, configure the project structure, and ensure the reasoning engine runs correctly."

order: 2

tags: ["ThinkEngine", "installation", "setup", "Unity", "DLV2", "ASP"]

---



# 2. Installation and Setup



This section explains how to correctly install ThinkEngine, place its files in the Unity project, and configure the environment so that the reasoning engine (**DLV2**) runs smoothly both in the Editor and at runtime.



---



## 2.1 Project Structure Overview



A typical ThinkEngine project follows a clear folder structure to ensure both the Unity Editor and the build can find the necessary components:



```plaintext
Assets/

├─ StreamingAssets/

│  └─ ThinkEngineer/

│     └─ ThinkEngine/

│        ├─ lib/

│        │  └─ dlv2.exe               # solver binary (runtime)

│        ├─ ReactiveFollowTarget.asp  # runtime ASP rules (example)

│        ├─ ReactiveTest.asp          # runtime ASP rules (example)

│        └─ PlannerTest.asp           # runtime ASP rules (example)

│

├─ ThinkEngineer/

│  └─ ThinkEngine/

│     ├─ Plugins/

│     │  ├─ ThinkEngine.dll           # runtime DLL

│     │  └─ ThinkEngineDLL/           # design-time/editor DLLs

│     │     ├─ ThinkEngine.dll        # editor DLL

│     │     └─ (other deps: Antlr, System.\*)

│     │

│     ├─ Scripts/

│     │  └─ ThinkEngineTrigger.cs     # optional custom triggers

│     │

│     └─ Templates/                   # auto-generated ASP templates (editor-side)

│        ├─ AgentReactiveBrain0Template.asp

│        ├─ AgentPlannerBrain0Template.asp

│        └─ ...

│

└─ Scripts/                            # your gameplay scripts

&nbsp;  ├─ Actions/

&nbsp;  ├─ Script1.cs

&nbsp;  └─ Script2.cs

```



This layout ensures:



- The solver binary (`dlv2.exe` on Windows) is accessible at runtime.  

- ThinkEngine’s DLL is correctly loaded during both play mode and build.  

- The `.asp` files are placed in a persistent and accessible folder.  

- Templates and facts files are clearly separated.



**Key rules:**



- Runtime ASP files must live under  

&nbsp; `Assets/StreamingAssets/ThinkEngineer/ThinkEngine/`  

&nbsp; so they’re included in builds.



- Templates are **not** runtime rules. They live under  

&nbsp; `Assets/ThinkEngineer/ThinkEngine/Templates/`  

&nbsp; and are editor helpers.



- The solver must be under  

&nbsp; `Assets/StreamingAssets/ThinkEngineer/ThinkEngine/lib/`  

&nbsp; and be executable.



- The ThinkEngine DLLs live under  

&nbsp; `Assets/ThinkEngineer/ThinkEngine/Plugins/…` (runtime) and  

&nbsp; `…/Plugins/ThinkEngineDLL/` (editor).



---



## 2.2 What You Need (Components Checklist)



### ThinkEngine DLLs



- `Assets/ThinkEngineer/ThinkEngine/Plugins/ThinkEngine.dll` (runtime)  

- `Assets/ThinkEngineer/ThinkEngine/Plugins/ThinkEngineDLL/ThinkEngine.dll` (editor/design-time)  

- plus supporting DLLs in `ThinkEngineDLL/` (Antlr, System.\*)



### Solver (DLV2)



- **Windows:**  

&nbsp; `Assets/StreamingAssets/ThinkEngineer/ThinkEngine/lib/dlv2.exe`



- **macOS/Linux:**  

&nbsp; `…/lib/dlv2` \*(make it executable)\*



### ASP rule files (runtime)



- Example:  

&nbsp; `Assets/StreamingAssets/ThinkEngineer/ThinkEngine/ReactiveFollowTarget.asp`  

- (You may organize in subfolders; see §2.6)



### Optional editor-side files



- `Assets/ThinkEngineer/ThinkEngine/Templates/\*.asp` (auto-generated templates)  

- `Assets/ThinkEngineer/ThinkEngine/Scripts/ThinkEngineTrigger.cs` (your triggers)



---



## 2.3 Unity Import Settings (DLLs)



Open each `ThinkEngine.dll` in the Inspector and ensure:



### Runtime DLL  

`Assets/.../Plugins/ThinkEngine.dll`



- ✔ Any Platform  

- ✔ Standalone (and any target platforms you build for)  

- (Editor is optional for this one)



### Editor/Design-time DLL  

`Assets/.../Plugins/ThinkEngineDLL/ThinkEngine.dll`



- ✔ Editor  

- (Usually not included in Standalone)



Avoid overlapping platform selections between these two so Unity picks the right one in **Editor vs Build**.



---



## 2.4 Make the Solver Executable



### Windows (PowerShell)



```powershell

cd Assets\\StreamingAssets\\ThinkEngineer\\ThinkEngine\\lib

Unblock-File .\\dlv2.exe



# verify it’s unblocked:

Get-Item -Stream Zone.Identifier .\\dlv2.exe

# should say the stream is not found

```



### macOS/Linux (Terminal)



```bash

chmod +x Assets/StreamingAssets/ThinkEngineer/ThinkEngine/lib/dlv2

```



If DLV2 is blocked or missing, the brain won’t start reasoning.



---



## 2.5 Where to Put ASP Files (Runtime)



Place all rule files that must run under:



```plaintext
Assets/StreamingAssets/ThinkEngineer/ThinkEngine/

```



You can keep them flat or organize into subfolders, for example:



```prolog
Assets/StreamingAssets/ThinkEngineer/ThinkEngine/

└─ Reasoning/

&nbsp;  ├─ reactive/

&nbsp;  │  ├─ ReactiveFollowTarget.asp

&nbsp;  │  └─ ...

&nbsp;  └─ planner/

&nbsp;     ├─ PlannerTest.asp

&nbsp;     └─ ...

```



---



## 2.6 Connecting Brains to ASP Files (Overview Only)



Brains are the ThinkEngine components that run reasoning using your `.asp` rules.  

Each Brain references one or more `.asp` files through the **AI Files Prefix** field in its Inspector.



💡 Important: at this stage, you don’t need to configure this yet. The actual setup will happen after sensors and actuators are in place (see **Section 4**).



### Where to place Brains



- You can attach a Brain directly to the Agent GameObject.  

- Alternatively, you can create an empty GameObject (e.g., `BrainHost`) and place the PlannerBrain and/or ReactiveBrain there.



This is often cleaner for larger projects because logic and movement are decoupled.



### Best practices



- Keep ReactiveBrain and PlannerBrain on separate objects if you use both.  

- Only one Brain should be active at runtime (or coordinate carefully with a toggle).  

- Runtime `.asp` files must live under  

&nbsp; `Assets/StreamingAssets/ThinkEngineer/ThinkEngine/...`.



👉 How to set the actual file path or pattern will be explained later in **Section 4 — Configuring Brains**.



---



## 2.7 Runtime Facts Dumps



During reasoning, ThinkEngine internally generates ASP fact files that are passed to the solver.  

If you enable **Maintain input file** in a Brain, these facts can be saved locally for inspection.



📌 The detailed location and usage of these debug files will be explained in **Section 4.4 — Debugging Brains**.



---



## 2.8 Recommended Organization Patterns



- Keep **reactive** and **planner** rule sets in separate subfolders; it prevents accidental co-loading.  

- Use clear, explicit prefixes (e.g., `Reasoning/reactive/FollowTarget`) or patterns (`reactive/\*.asp`) depending on whether you want to load a bundle of rules.  

- Keep gameplay C# in `Assets/Scripts/...` and ThinkEngine editor assets in `Assets/ThinkEngineer/ThinkEngine/...`.



---



## 2.9 Quick Sanity Checklist



- ✅ `dlv2(.exe)` exists at  

&nbsp; `Assets/StreamingAssets/ThinkEngineer/ThinkEngine/lib/`  

&nbsp; and is executable.



- ✅ Your runtime `.asp` rules live under  

&nbsp; `Assets/StreamingAssets/ThinkEngineer/ThinkEngine/...`.



- ✅ ThinkEngine DLLs:

&nbsp; - Runtime DLL in `.../Plugins/` has **Standalone** checked.

&nbsp; - Editor DLL in `.../Plugins/ThinkEngineDLL/` has **Editor** checked.



- ✅ In the Brain:

&nbsp; - `AI Files Prefix` points to the correct path/pattern under `StreamingAssets`.  

&nbsp; - `Maintain input file` (optional) writes to `ThinkEngineFacts` in your temp folder.  

&nbsp; - A Trigger is selected that actually fires (e.g., `AlwaysTrue` for debugging).


