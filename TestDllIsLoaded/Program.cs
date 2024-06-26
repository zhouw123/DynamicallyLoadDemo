﻿using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using TestDllIsLoaded;

namespace test
{
    class Test
    { 
         

    }

    class Program
    {
        public static void ThreadProc()
        {
            // Issue preventing unloading #4 - a thread running method inside of the TestAssemblyLoadContext at the unload time
            Thread.Sleep(Timeout.Infinite);
        }

        static GCHandle handle;

        [STAThread]
        static int Main(string[] args)
        {

 
            App app = new App();
            app.Run();
            // Issue preventing unloading #3 - normal GC handle
            handle = GCHandle.Alloc(new Test());
            Thread t = new Thread(new ThreadStart(ThreadProc));
            t.IsBackground = true;
            t.Start();
            Console.WriteLine($"Hello from the test: args[0] = {args[0]}");

            return 1;
        }
    }
}
