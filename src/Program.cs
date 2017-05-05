using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenGame
{
    class Program
    {
        // Our runtime configuration instance.
        private static RuntimeConfiguration Runtime;

        // Our Ruby manager instance
        private static Ruby Ruby;

        public static GameWindow Window;

        // The program's start point, entered from the Launcher
        // RuntimeConfiguration is provided
        public static void Start(RuntimeConfiguration rtc)
        {
            Runtime = rtc;

            //Create the Ruby instance
            Ruby = new Ruby();

            //compensate for border and caption thickness
            int width = Runtime.GetDefaultResolutionWidth();// + (GetSystemMetrics(SystemMetric.SM_CXFIXEDFRAME) << 1);
            int height = Runtime.GetDefaultResolutionHeight();// + (GetSystemMetrics(SystemMetric.SM_CYFIXEDFRAME) << 1) + GetSystemMetrics(SystemMetric.SM_CYCAPTION);

            Window = new GameWindow(width, height);
            OG_Graphics.dpi_scale = Window.Width / (float)width;

            Window.Width = width;
            Window.Height = height;

            System.Drawing.Size client_size = Window.ClientSize;
            client_size.Width = width;
            client_size.Height = height;
            Window.ClientSize = client_size;

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

        public static RuntimeConfiguration GetRuntime(){
            return Runtime;
        }

        private static void OnClose(object sender, EventArgs e)
        {
            Exit();
        }

        public static void OnLoad()
        {
            Window.VSync = VSyncMode.On;
            Window.Title = Runtime.GetGameTitle();

            //setup opengl
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Runtime.GetDefaultResolutionWidth(), Runtime.GetDefaultResolutionHeight(), 0, -1, 1);
            GL.Enable(EnableCap.Texture2D); //TODO: Only enable textures where they are needed (fixed function performance hit)
            GL.Enable(EnableCap.Blend); //TODO: blend should only be enabled where it is needed! Needless performance hit here
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.One);
            GL.Disable(EnableCap.DepthTest); //TODO: Remove depth buffer from context if depth test is not used
            GL.ClearColor(0f, 0f, 0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Window.SwapBuffers();
        }

        public static void OnResize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, Window.Width, Window.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Runtime.GetDefaultResolutionWidth(), Runtime.GetDefaultResolutionHeight(), 0, -1, 1);
        }

        //A temporary error method until a better logger/crash handler is provided
        public static void Error(string message)
        {
            Console.WriteLine("[ERROR] " + message);
            MessageBox.Show(message);
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

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(SystemMetric smIndex);

        private enum SystemMetric
        {
            SM_CYCAPTION = 4, //window caption area height
            SM_CXFIXEDFRAME = 7, //nonresizeable window border width
            SM_CYFIXEDFRAME = 8, //nonresizeable window border height
            SM_CXSIZEFRAME = 32, //resizeable window border width
            SM_CYSIZEFRAME = 33  //resizeable window border width
        }
    }
}