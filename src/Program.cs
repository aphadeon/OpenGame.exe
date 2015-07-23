using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenGame
{
    class Program
    {
        //A helper class to translate our commandline switches
        private static CommandLineSwitches Switches;

        //A helper class to track down those pesky RPG Maker RTPs.
        private static RTP Rtp;

        private static int RGSSVersion;

        //A special pseudo-variable that reliably gets the executable's file location
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        //The program's entry point - command line switches are loaded as arguments here
        public static void Main(string[] args)
        {
            //Always enable the console window if we are in DEBUG configuration
            #if DEBUG
                ConsoleWindow.Show();
            #endif

            //Setup DLL resolution (so that it can see DLLs in the /System/ directory)
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveDLL);

            //Read which version of RGSS we are aiming to emulate
            ReadRGSSVersion();

            //Store the command-line switches
            Switches = new CommandLineSwitches(args);

            //Setup the RTP
            Rtp = new RTP(AssemblyDirectory);
        }

        //An accessor method for the RTP instance
        public static RTP GetRtp()
        {
            return Rtp;
        }

        //An accessor method for the CommandLineSwitches instance
        public static CommandLineSwitches GetCommandLineSwitches()
        {
            return Switches;
        }

        //An accessor method for the RGSS Version int
        public static int GetRGSSVersion()
        {
            return RGSSVersion;
        }

        //This function reads the Game.ini to determine which RGSS DLL the
        //game is trying to use, ergo which one we should emulate.
        private static void ReadRGSSVersion()
        {
            string s = "";
            try
            {
                s = File.ReadAllText("Game.ini");
            }
            catch
            {
                Program.Error("Could not load Game.ini");
            }

            int i = 0;

            try
            {
                i = s.IndexOf("RGSS") + "RGSS".Length;
                s = s.Substring(i, s.LastIndexOf(".") - i);
                s = s.Substring(0, 1);
                i = Convert.ToInt32(s);
            }
            catch
            {
                Program.Error("Could not retrieve RGSS version from Game.ini");
            }
            if(i < 1 || i > 3) Program.Error("Unsupported RGSS version");
            RGSSVersion = i;
        }

        //A temporary error method until a better logger/crash handler is provided
        public static void Error(string message)
        {
            Console.WriteLine("[ERROR] " + message);
            MessageBox.Show("Error: " + message);
            Environment.Exit(-1);
        }

        //This is used to load assemblies manually, so that we can keep our output directory
        //clean by stashing the DLLs in /System/, akin to vanilla RGSS DLL location.
        public static Assembly ResolveDLL(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyPath = Path.Combine(folderPath + @"\System\", new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(assemblyPath) == false) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
    }
}
