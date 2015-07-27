using System;
using System.IO;
using System.Reflection;

namespace OpenGame
{
    class Launcher
    {
        //This class is the program's entry point, which uses no DLL references so that we can
        //setup DLL resolution prior to starting the program.
        public static void Main(string[] args)
        {
            //Setup DLL resolution (so that it can see DLLs in the /System/ directory)
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveDLL);
            Program.Main(args);
        }

        //This is used to load assemblies manually, so that we can keep our output directory
        //clean by stashing the DLLs in /System/, akin to vanilla RGSS DLL location.
        public static Assembly ResolveDLL(object sender, ResolveEventArgs args)
        {
            Console.WriteLine("Resolving dll " + args.Name);
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyPath = Path.Combine(folderPath + @"\System\", new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(assemblyPath) == false) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
    }
}
