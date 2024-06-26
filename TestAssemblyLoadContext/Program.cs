﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
 

namespace example
{
    class TestAssemblyLoadContext : AssemblyLoadContext
    {
        public TestAssemblyLoadContext() : base(true)
        {
        }
        protected override Assembly? Load(AssemblyName name)
        {
            return null;
        }
    }

    class TestInfo
    {
        public TestInfo(MethodInfo? mi)
        {
            _entryPoint = mi;
        }
  
        MethodInfo? _entryPoint;
    }

    class Program
    {
        static TestInfo? entryPoint;


        [MethodImpl(MethodImplOptions.NoInlining)]
        static int ExecuteAndUnload(string assemblyPath, out WeakReference testAlcWeakRef, out MethodInfo? testEntryPoint)
        {
            var alc = new TestAssemblyLoadContext();
            testAlcWeakRef = new WeakReference(alc);

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(assemblyPath)))
            {
                Assembly a = alc.LoadFromStream(ms); //.LoadFromAssemblyPath(assemblyPath);
                if (a == null)
                {
                    testEntryPoint = null;
                    Console.WriteLine("Loading the test assembly failed");
                    return -1;
                }


                var args = new object[1] { new string[] { "Hello" } };

                // Issue preventing unloading #1 - we keep MethodInfo of a method
                // for an assembly loaded into the TestAssemblyLoadContext in a static variable.
                entryPoint = new TestInfo(a.EntryPoint);
                testEntryPoint = a.EntryPoint;

                var oResult = a.EntryPoint?.Invoke(null, args);
                alc.Unload();

                return (oResult is int result) ? result : -1;
            }
         
        }
    

        [STAThread]

        static void Main(string[] args)
        {
            WeakReference testAlcWeakRef;
            // Issue preventing unloading #2 - we keep MethodInfo of a method for an assembly loaded into the TestAssemblyLoadContext in a local variable
            MethodInfo? testEntryPoint;
            int result = ExecuteAndUnload(@"..\..\..\..\TestDllIsLoaded\bin\Debug\netcoreapp3.1\TestDllIsLoaded.dll", out testAlcWeakRef, out testEntryPoint);
          //  int result = ExecuteAndUnload(@"E:\HYTec\Speedre\UVDebugTools\UVDebugTools\confuser_test\bin\Debug\netcoreapp3.1\testA.dll", out testAlcWeakRef, out testEntryPoint); 
        
            for (int i = 0; testAlcWeakRef.IsAlive && (i < 10); i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            System.Diagnostics.Debugger.Break();

            //Console.WriteLine($"Test completed, result={result}, entryPoint: {testEntryPoint} unload success: {!testAlcWeakRef.IsAlive}");
        }
    }
}
