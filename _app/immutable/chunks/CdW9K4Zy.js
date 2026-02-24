import"./DsnmJJEf.js";import"./B8phH7r5.js";import{O as k,P as e,Q as T,R as E,S as n,T as i,U as f}from"./m6VW359Z.js";import{h as s}from"./BGy7xXYn.js";const v={title:"Installation and Setup",description:"How to install and set up ThinkEngine, configure the project structure, and ensure the reasoning engine runs correctly.",order:2,tags:["ThinkEngine","installation","setup","Unity","DLV2","ASP"]},{title:w,description:x,order:P,tags:D}=v;var b=k('<h1 id="2-installation-and-setup">2. Installation and Setup</h1> <p>This section explains how to correctly install ThinkEngine, place its files in the Unity project, and configure the environment so that the reasoning engine (<strong>DLV2</strong>) runs smoothly both in the Editor and at runtime.</p> <hr/> <h2 id="21-project-structure-overview">2.1 Project Structure Overview</h2> <p>A typical ThinkEngine project follows a clear folder structure to ensure both the Unity Editor and the build can find the necessary components:</p> <pre class="language-plaintext"><!></pre> <p>This layout ensures:</p> <ul><li><p>The solver binary (<code>dlv2.exe</code> on Windows) is accessible at runtime.</p></li> <li><p>ThinkEngine’s DLL is correctly loaded during both play mode and build.</p></li> <li><p>The <code>.asp</code> files are placed in a persistent and accessible folder.</p></li> <li><p>Templates and facts files are clearly separated.</p></li></ul> <p><strong>Key rules:</strong></p> <ul><li>Runtime ASP files must live under</li></ul> <p>  <code>Assets/StreamingAssets/ThinkEngineer/ThinkEngine/</code></p> <p>  so they’re included in builds.</p> <ul><li>Templates are <strong>not</strong> runtime rules. They live under</li></ul> <p>  <code>Assets/ThinkEngineer/ThinkEngine/Templates/</code></p> <p>  and are editor helpers.</p> <ul><li>The solver must be under</li></ul> <p>  <code>Assets/StreamingAssets/ThinkEngineer/ThinkEngine/lib/</code></p> <p>  and be executable.</p> <ul><li>The ThinkEngine DLLs live under</li></ul> <p>  <code>Assets/ThinkEngineer/ThinkEngine/Plugins/…</code> (runtime) and</p> <p>  <code>…/Plugins/ThinkEngineDLL/</code> (editor).</p> <hr/> <h2 id="22-what-you-need-components-checklist">2.2 What You Need (Components Checklist)</h2> <h3 id="thinkengine-dlls">ThinkEngine DLLs</h3> <ul><li><p><code>Assets/ThinkEngineer/ThinkEngine/Plugins/ThinkEngine.dll</code> (runtime)</p></li> <li><p><code>Assets/ThinkEngineer/ThinkEngine/Plugins/ThinkEngineDLL/ThinkEngine.dll</code> (editor/design-time)</p></li> <li><p>plus supporting DLLs in <code>ThinkEngineDLL/</code> (Antlr, System.*)</p></li></ul> <h3 id="solver-dlv2">Solver (DLV2)</h3> <ul><li><strong>Windows:</strong></li></ul> <p>  <code>Assets/StreamingAssets/ThinkEngineer/ThinkEngine/lib/dlv2.exe</code></p> <ul><li><strong>macOS/Linux:</strong></li></ul> <p>  <code>…/lib/dlv2</code> *(make it executable)*</p> <h3 id="asp-rule-files-runtime">ASP rule files (runtime)</h3> <ul><li>Example:</li></ul> <p>  <code>Assets/StreamingAssets/ThinkEngineer/ThinkEngine/ReactiveFollowTarget.asp</code></p> <ul><li>(You may organize in subfolders; see §2.6)</li></ul> <h3 id="optional-editor-side-files">Optional editor-side files</h3> <ul><li><p><code>Assets/ThinkEngineer/ThinkEngine/Templates/\\*.asp</code> (auto-generated templates)</p></li> <li><p><code>Assets/ThinkEngineer/ThinkEngine/Scripts/ThinkEngineTrigger.cs</code> (your triggers)</p></li></ul> <hr/> <h2 id="23-unity-import-settings-dlls">2.3 Unity Import Settings (DLLs)</h2> <p>Open each <code>ThinkEngine.dll</code> in the Inspector and ensure:</p> <h3 id="runtime-dll">Runtime DLL</h3> <p><code>Assets/.../Plugins/ThinkEngine.dll</code></p> <ul><li><p>✔ Any Platform</p></li> <li><p>✔ Standalone (and any target platforms you build for)</p></li> <li><p>(Editor is optional for this one)</p></li></ul> <h3 id="editordesign-time-dll">Editor/Design-time DLL</h3> <p><code>Assets/.../Plugins/ThinkEngineDLL/ThinkEngine.dll</code></p> <ul><li><p>✔ Editor</p></li> <li><p>(Usually not included in Standalone)</p></li></ul> <p>Avoid overlapping platform selections between these two so Unity picks the right one in <strong>Editor vs Build</strong>.</p> <hr/> <h2 id="24-make-the-solver-executable">2.4 Make the Solver Executable</h2> <h3 id="windows-powershell">Windows (PowerShell)</h3> <pre class="language-powershell"><!></pre> <h3 id="macoslinux-terminal">macOS/Linux (Terminal)</h3> <pre class="language-bash"><!></pre> <p>If DLV2 is blocked or missing, the brain won’t start reasoning.</p> <hr/> <h2 id="25-where-to-put-asp-files-runtime">2.5 Where to Put ASP Files (Runtime)</h2> <p>Place all rule files that must run under:</p> <pre class="language-plaintext"><!></pre> <p>You can keep them flat or organize into subfolders, for example:</p> <pre class="language-prolog"><!></pre> <hr/> <h2 id="26-connecting-brains-to-asp-files-overview-only">2.6 Connecting Brains to ASP Files (Overview Only)</h2> <p>Brains are the ThinkEngine components that run reasoning using your <code>.asp</code> rules.</p> <p>Each Brain references one or more <code>.asp</code> files through the <strong>AI Files Prefix</strong> field in its Inspector.</p> <p>💡 Important: at this stage, you don’t need to configure this yet. The actual setup will happen after sensors and actuators are in place (see <strong>Section 4</strong>).</p> <h3 id="where-to-place-brains">Where to place Brains</h3> <ul><li><p>You can attach a Brain directly to the Agent GameObject.</p></li> <li><p>Alternatively, you can create an empty GameObject (e.g., <code>BrainHost</code>) and place the PlannerBrain and/or ReactiveBrain there.</p></li></ul> <p>This is often cleaner for larger projects because logic and movement are decoupled.</p> <h3 id="best-practices">Best practices</h3> <ul><li><p>Keep ReactiveBrain and PlannerBrain on separate objects if you use both.</p></li> <li><p>Only one Brain should be active at runtime (or coordinate carefully with a toggle).</p></li> <li><p>Runtime <code>.asp</code> files must live under</p></li></ul> <p>  <code>Assets/StreamingAssets/ThinkEngineer/ThinkEngine/...</code>.</p> <p>👉 How to set the actual file path or pattern will be explained later in <strong>Section 4 — Configuring Brains</strong>.</p> <hr/> <h2 id="27-runtime-facts-dumps">2.7 Runtime Facts Dumps</h2> <p>During reasoning, ThinkEngine internally generates ASP fact files that are passed to the solver.</p> <p>If you enable <strong>Maintain input file</strong> in a Brain, these facts can be saved locally for inspection.</p> <p>📌 The detailed location and usage of these debug files will be explained in <strong>Section 4.4 — Debugging Brains</strong>.</p> <hr/> <h2 id="28-recommended-organization-patterns">2.8 Recommended Organization Patterns</h2> <ul><li><p>Keep <strong>reactive</strong> and <strong>planner</strong> rule sets in separate subfolders; it prevents accidental co-loading.</p></li> <li><p>Use clear, explicit prefixes (e.g., <code>Reasoning/reactive/FollowTarget</code>) or patterns (<code>reactive/\\*.asp</code>) depending on whether you want to load a bundle of rules.</p></li> <li><p>Keep gameplay C# in <code>Assets/Scripts/...</code> and ThinkEngine editor assets in <code>Assets/ThinkEngineer/ThinkEngine/...</code>.</p></li></ul> <hr/> <h2 id="29-quick-sanity-checklist">2.9 Quick Sanity Checklist</h2> <ul><li>✅ <code>dlv2(.exe)</code> exists at</li></ul> <p>  <code>Assets/StreamingAssets/ThinkEngineer/ThinkEngine/lib/</code></p> <p>  and is executable.</p> <ul><li>✅ Your runtime <code>.asp</code> rules live under</li></ul> <p>  <code>Assets/StreamingAssets/ThinkEngineer/ThinkEngine/...</code>.</p> <ul><li>✅ ThinkEngine DLLs:</li></ul> <p>  - Runtime DLL in <code>.../Plugins/</code> has <strong>Standalone</strong> checked.</p> <p>  - Editor DLL in <code>.../Plugins/ThinkEngineDLL/</code> has <strong>Editor</strong> checked.</p> <ul><li>✅ In the Brain:</li></ul> <p>  - <code>AI Files Prefix</code> points to the correct path/pattern under <code>StreamingAssets</code>.</p> <p>  - <code>Maintain input file</code> (optional) writes to <code>ThinkEngineFacts</code> in your temp folder.</p> <p>  - A Trigger is selected that actually fires (e.g., <code>AlwaysTrue</code> for debugging).</p>',1);function B(c){var r=b(),t=e(T(r),10),d=n(t);s(d,()=>`<code class="language-plaintext">Assets/

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

│     │     └─ (other deps: Antlr, System.*)

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

&amp;nbsp;  ├─ Actions/

&amp;nbsp;  ├─ Script1.cs

&amp;nbsp;  └─ Script2.cs
</code>`),i(t);var a=e(t,88),g=n(a);s(g,()=>`<code class="language-powershell">
cd Assets\\StreamingAssets\\ThinkEngineer\\ThinkEngine\\lib

<span class="token function">Unblock-File</span> <span class="token punctuation">.</span>\\dlv2<span class="token punctuation">.</span>exe



<span class="token comment"># verify it’s unblocked:</span>

<span class="token function">Get-Item</span> <span class="token operator">-</span>Stream Zone<span class="token punctuation">.</span>Identifier <span class="token punctuation">.</span>\\dlv2<span class="token punctuation">.</span>exe

<span class="token comment"># should say the stream is not found</span>
</code>`),i(a);var o=e(a,4),u=n(o);s(u,()=>`<code class="language-bash">
<span class="token function">chmod</span> +x Assets/StreamingAssets/ThinkEngineer/ThinkEngine/lib/dlv2
</code>`),i(o);var l=e(o,10),h=n(l);s(h,()=>`<code class="language-plaintext">Assets/StreamingAssets/ThinkEngineer/ThinkEngine/
</code>`),i(l);var p=e(l,4),m=n(p);s(m,()=>`<code class="language-prolog">Assets<span class="token operator">/</span>StreamingAssets<span class="token operator">/</span>ThinkEngineer<span class="token operator">/</span>ThinkEngine<span class="token operator">/</span>

└─ Reasoning<span class="token operator">/</span>

&amp;nbsp<span class="token operator">;</span>  ├─ reactive<span class="token operator">/</span>

&amp;nbsp<span class="token operator">;</span>  │  ├─ ReactiveFollowTarget<span class="token operator">.</span>asp

&amp;nbsp<span class="token operator">;</span>  │  └─ <span class="token operator">...</span>

&amp;nbsp<span class="token operator">;</span>  └─ planner<span class="token operator">/</span>

&amp;nbsp<span class="token operator">;</span>     ├─ PlannerTest<span class="token operator">.</span>asp

&amp;nbsp<span class="token operator">;</span>     └─ <span class="token operator">...</span>
</code>`),i(p),f(68),E(c,r)}export{B as default,v as metadata};
