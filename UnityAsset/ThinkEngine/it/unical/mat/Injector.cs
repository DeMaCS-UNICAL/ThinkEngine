using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat
{
    public class Injector
    {
        public static void install(object invoker, Collider2D injectIn, string toInject)
        {
            unsafe
            {
                Debug.Log(injectIn.GetType() + " collider type");
                MethodInfo injectInMethod = injectIn.GetType().GetMethod("OnTriggerEnter",BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                Debug.Log(invoker.GetType() + " invoker type, method is "+injectInMethod);
                MethodInfo toInjectMethod = invoker.GetType().GetMethod(toInject, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                Debug.Log(invoker.GetType() + " invoker type, method is " + toInjectMethod);
                if (IntPtr.Size == 4)
                {
                    int* inj = (int*)toInjectMethod.MethodHandle.Value.ToPointer() + 2;
                    int* tar = (int*)injectInMethod.MethodHandle.Value.ToPointer() + 2;
#if DEBUG
                    Console.WriteLine("\nVersion x86 Debug\n");

                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;

                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);

                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                    Console.WriteLine("\nVersion x86 Release\n");
                    *tar = *inj;
#endif
                }
                else
                {

                    long* inj = (long*)toInjectMethod.MethodHandle.Value.ToPointer() + 1;
                    long* tar = (long*)injectInMethod.MethodHandle.Value.ToPointer() + 1;
#if DEBUG
                    Console.WriteLine("\nVersion x64 Debug\n");
                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;


                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);

                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                    Console.WriteLine("\nVersion x64 Release\n");
                    *tar = *inj;
#endif
                }
            }
        }
    }
}
