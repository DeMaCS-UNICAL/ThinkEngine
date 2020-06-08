using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Mono.Cecil;



public class testCecil
{
    // Start is called before the first frame update


    //[MenuItem("Assets/Run Cecil")]
    //[PostProcessScene(0)]
    public static void Run()
    {
        using (var assembly = AssemblyDefinition.ReadAssembly("./Library/ScriptAssemblies", new ReaderParameters { ReadWrite = true }))
        {
            // ... Injection code goes here

            Debug.Log(assembly.FullName);
        }
    }
}
