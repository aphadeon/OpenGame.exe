using OpenTK;
using OpenTK.Graphics.OpenGL;
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

        //An int to track the RGSS Version- will be set to 1, 2, or 3.
        private static int RGSSVersion;

        private static Ruby Ruby;

        public static GameWindow Window;

        public static int ResolutionWidth;
        public static int ResolutionHeight;

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

            //Store the command-line switches
            Switches = new CommandLineSwitches(args);

            //Read which version of RGSS we are aiming to emulate
            //or observe the forced version commandline switch
            if (Switches.GetForcedRGSSVersion() > 0)
            {
                RGSSVersion = Switches.GetForcedRGSSVersion();
            }
            else
            {
                ReadRGSSVersion();
            }
            
            //Setup the RTP
            Rtp = new RTP(AssemblyDirectory);

            //Create the Ruby instance
            Ruby = new Ruby();

            if(RGSSVersion == 1){
                ResolutionWidth = 640;
                ResolutionHeight = 480;
            } else {
                ResolutionWidth = 544;
                ResolutionHeight = 416;
            }

            Window = new GameWindow(ResolutionWidth, ResolutionHeight);

            Window.Icon = Properties.Resources.OpenGame;

            Window.Resize += (sender, e) => { OnResize(sender, e); };

            Window.Closed += (sender, e) => { OnClose(sender, e); };

            Window.Visible = true;
            OnLoad();

            //Start the game
            Ruby.Start();

            //Clean up the Ruby instance
            Ruby.Dispose();
        }

        private static void OnClose(object sender, EventArgs e)
        {
            Exit();
        }

        public static void OnLoad()
        {
            Window.VSync = VSyncMode.On;
            Window.Title = "OpenGame.exe"; //TODO: load title from ini

            //setup opengl
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, ResolutionWidth, ResolutionHeight, 0, -1, 1);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(0f, 0f, 0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Window.SwapBuffers();
        }

        public static void OnResize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, Window.Width, Window.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, ResolutionWidth, ResolutionHeight, 0, -1, 1);
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
            Exit(-1);
        }

        //Shorthand exit function without a status code
        public static void Exit()
        {
            Exit(0);
        }

        //Program safe exit handler
        public static void Exit(int code)
        {
            if (Window != null) Window.Exit();
            Environment.Exit(code);
        }
    }
}