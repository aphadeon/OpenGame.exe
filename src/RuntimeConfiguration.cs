using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenGame
{
    class RuntimeConfiguration
    {
        private bool Debug;
        private bool WindowsOS;
        private bool PlayTest;
        private bool BattleTest;
        private string DataPath;
        private string GameTitle;
        private int RGSSVersion;
        private int DefaultResolutionWidth;
        private int DefaultResolutionHeight;
        private List<String> ResourcePaths;
        private string PathDelimiter;

        public RuntimeConfiguration(string[] parameters)
        {
            // Is this a Debug targeted build?
            Debug = false;
            #if DEBUG
                Debug = true;
            #endif
            
            // Is this a Windows targeted build?
            WindowsOS = CheckIfWindowsOS();
            
            if (WindowsOS)
            {
                PathDelimiter = "\\";

                // Enable the console if applicable
                if (Debug)
                {
                    var handle = GetConsoleWindow();
                    if (handle == IntPtr.Zero)
                    {
                        AllocConsole();
                    }
                    else
                    {
                        ShowWindow(handle, SW_SHOW);
                    }
                }                
            }
            else
            {
                PathDelimiter = "/";
            }

            // Setup command-line parameter defaults
            DataPath = GetAssemblyLocation();
            PlayTest = false;
            BattleTest = false;
            RGSSVersion = 0;

            string OverrideRtp = null;
            List<string> ExtraRtps = new List<string>();
            // Parse command-line parameters
            for (int i = 0; i < parameters.Count(); i++)
            {
                string value = parameters[i];
                switch (value)
                {
                    case "-game":
                        i++; //pre-emptively increment to the accompanying parameter
                        //Set the DataPath where we will look for game files
                        DataPath = ResolvePath(parameters[i].Replace("\"", ""));
                        break;
                    case "-rtp":
                        i++; //pre-emptively increment to the accompanying parameter
                        //Set the RTP override path
                        OverrideRtp = parameters[i].Replace("\"", "");
                        break;
                    case "-addrtp":
                        i++; //pre-emptively increment to the accompanying parameter
                        // Add this RTP as an extra resource path
                        ExtraRtps.Add(parameters[i].Replace("\"", ""));
                        break;
                    case "test": //rgss 2|3  TODO: Make sure these are correct. Can't find the relative info to confirm.
                    case "debug": //rgss 1
                        // Only allow PlayTest on Debug builds
                        if (Debug) PlayTest = true;
                        break;
                    case "btest":
                        // Only allow BattleTest on Debug builds
                        if (Debug) BattleTest = true;
                        break;
                    //RGSS Version override flags
                    case "rgss1":
                        RGSSVersion = 1;
                        break;
                    case "rgss2":
                        RGSSVersion = 2;
                        break;
                    case "rgss3":
                        RGSSVersion = 3;
                        break;
                }
            }

            // Setup resource paths
            ResourcePaths = new List<string>();
            // Add the game directory
            ResourcePaths.Add(DataPath);

            GameTitle = ReadGameTitle();
            if(RGSSVersion == 0) RGSSVersion = ReadRGSSVersion();

            // Set default resolution according to RGSS version
            if (RGSSVersion == 1)
            {
                DefaultResolutionWidth = 640;
                DefaultResolutionHeight = 480;
            }
            else
            {
                DefaultResolutionWidth = 544;
                DefaultResolutionHeight = 416;
            }

            // Add Extra RTPs
            ResourcePaths.AddRange(ExtraRtps);
            // Add default RTP(s) last
            if (OverrideRtp != null)
            {
                ResourcePaths.Add(OverrideRtp);
            }
            else if (WindowsOS)
            {
                FindRTPs();
            }
        }

        private void FindRTPs()
        {
            if (RGSSVersion == 1)
            {
                //TODO: RPG Maker XP supported multiple RTPs.  All that needs to change is the key name,
                // where you see "Standard" below.  The names of additional RTPs would be listed in Game.ini .
                string key = @"HKEY_LOCAL_MACHINE\Software\Enterbrain\RGSS\RTP";
                string rtp = (string)Microsoft.Win32.Registry.GetValue(key, "Standard", "");
                if (String.IsNullOrWhiteSpace(rtp))
                {
                    key = @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Enterbrain\RGSS\RTP";
                    rtp = (string)Microsoft.Win32.Registry.GetValue(key, "Standard", "");
                }
                if (!String.IsNullOrWhiteSpace(rtp))
                {
                    ResourcePaths.Add(rtp + @"\");
                }
                return;
            }
            else if (RGSSVersion == 2)
            {
                string key = @"HKEY_LOCAL_MACHINE\Software\Enterbrain\RGSS2\RTP";
                string rtp = (string)Microsoft.Win32.Registry.GetValue(key, "RPGVX", "");
                if (String.IsNullOrWhiteSpace(rtp))
                {
                    key = @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Enterbrain\RGSS2\RTP";
                    rtp = (string)Microsoft.Win32.Registry.GetValue(key, "RPGVX", "");
                }
                if (!String.IsNullOrWhiteSpace(rtp))
                {
                    ResourcePaths.Add(rtp + @"\");
                }
                return;
            }
            else if (RGSSVersion == 3)
            {
                string key = @"HKEY_LOCAL_MACHINE\Software\Enterbrain\rgss3\rtp";
                string rtp = (string)Microsoft.Win32.Registry.GetValue(key, "rpgvxace", "");
                if (String.IsNullOrWhiteSpace(rtp))
                {
                    key = @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Enterbrain\rgss3\rtp";
                    rtp = (string)Microsoft.Win32.Registry.GetValue(key, "rpgvxace", "");
                }
                if (!String.IsNullOrWhiteSpace(rtp))
                {
                    ResourcePaths.Add(rtp + @"\");
                }
                return;
            }
        }

        //This function reads the Game.ini to determine which RGSS DLL the
        //game is trying to use, ergo which one we should emulate.
        private int ReadRGSSVersion()
        {
            string s = "";
            try
            {
                s = File.ReadAllText(ResourcePaths[0] + "Game.ini");
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
            if (i < 1 || i > 3) Program.Error("Unsupported RGSS version");
            return i;
        }

        // Reads Game.ini to retrieve the game title
        private string ReadGameTitle()
        {
            string s = "";
            try
            {
                s = File.ReadAllText(ResourcePaths[0] + "Game.ini");
            }
            catch
            {
                Program.Error("Could not load Game.ini");
            }

            string t = "";
            int i, r, n; //Start index, index of \r, index of \n
            try
            {
                i = s.IndexOf("Title=") + "Title=".Length;

                r = s.IndexOf('\u000D', i); //Search for \r
                n = s.IndexOf('\u000A', i); //Search for \n

                //Length of the sub-string is up to the next occurance of \r or \n after i
                t = s.Substring(i, (r < n ? r : n) - i);
            }
            catch
            {
                Program.Error("Could not retrieve Title from Game.ini");
            }

            string title = t;

            if (Debug)
            {
                title += " - Debug";
            }

            return title;
        }

        // Here to make sure any OS paths are correctly formatted (ending with delimiter)
        private string ResolvePath(string path)
        {
            string s;

            //Windows software usually supports / delimiters, so we can convert any of those to \\
            if (WindowsOS && path.EndsWith("/"))
            {
                s = path.Replace("/", PathDelimiter);
            }
            else
            {
                s = path;
            }

            //Make sure the path correctly ends with the OS' path delimiter
            if (!s.EndsWith(PathDelimiter))
            {
                s += PathDelimiter;
            }

            return s;
        }

        // These tests are run once at startup to determine whether the Operating System is Windows.
        private bool CheckIfWindowsOS()
        {
            // Are we running on the Mono runtime?
            if (Type.GetType("Mono.Runtime") != null)
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Unix:
                        if (Directory.Exists("/Applications")
                            & Directory.Exists("/System")
                            & Directory.Exists("/Users")
                            & Directory.Exists("/Volumes"))
                            return false; // Running Earlier Mac
                        else
                            return false; // Running Linux

                    case PlatformID.MacOSX:
                        return false; // Running Newer Mac

                    default:
                        return true; // Not running Mac or Linux, must be Windows
                }
            }
            else
            {
                return true; // We're on the MS runtime, must be Windows
            }
        }

        private string GetAssemblyLocation()
        {
            Console.WriteLine("Assembly Location: " + Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\");
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/";
        }

        public string[] GetResourcePaths()
        {
            return ResourcePaths.ToArray();
        }

        public int GetRGSSVersion()
        {
            return RGSSVersion;
        }

        public bool IsDebug()
        {
            return Debug;
        }

        public bool IsWindowsOS()
        {
            return WindowsOS;
        }

        public string GetDataPath()
        {
            return DataPath;
        }

        public string GetGameTitle()
        {
            return GameTitle;
        }

        public int GetDefaultResolutionWidth()
        {
            return DefaultResolutionWidth;
        }

        public int GetDefaultResolutionHeight()
        {
            return DefaultResolutionHeight;
        }

        // Windows only console allocation imports
        // Do not call from non-Windows OS! (will crash)
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
    }
}
