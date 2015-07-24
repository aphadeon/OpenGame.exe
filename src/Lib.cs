using IronRuby;
using IronRuby.Builtins;
using IronRuby.Runtime;
using Microsoft.Scripting.Hosting;
using OpenGame;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RGSS
{
    public class Input
    {
        private static Dictionary<int, bool> KeyStates = new Dictionary<int, bool>();
        private static Dictionary<int, bool> LastKeyStates = new Dictionary<int, bool>();

        public static void update()
        {
            if(!Program.Window.IsExiting) Program.Window.ProcessEvents();
            LastKeyStates = new Dictionary<int, bool>(KeyStates);

            KeyboardDevice kb = Program.Window.Keyboard;
            KeyStates[0] = (kb[OpenTK.Input.Key.S] || kb[OpenTK.Input.Key.Down]); //key 0, down
            KeyStates[1] = (kb[OpenTK.Input.Key.A] || kb[OpenTK.Input.Key.Left]); //key 1, left
            KeyStates[2] = (kb[OpenTK.Input.Key.D] || kb[OpenTK.Input.Key.Right]); //key 2, right
            KeyStates[3] = (kb[OpenTK.Input.Key.W] || kb[OpenTK.Input.Key.Up]); //key 3, up
            KeyStates[4] = (kb[OpenTK.Input.Key.LShift]); //key 4, A
            KeyStates[5] = (kb[OpenTK.Input.Key.X] || kb[OpenTK.Input.Key.Escape]); //key 5, B
            KeyStates[6] = (kb[OpenTK.Input.Key.Z] || kb[OpenTK.Input.Key.Enter]); //key 6, C
            KeyStates[7] = (kb[OpenTK.Input.Key.PageUp]); //key 7, L
            KeyStates[8] = (kb[OpenTK.Input.Key.PageDown]); //key 8, R
            KeyStates[9] = (kb[OpenTK.Input.Key.LShift] || kb[OpenTK.Input.Key.RShift]); //key 9, SHIFT
            KeyStates[10] = (kb[OpenTK.Input.Key.LControl] || kb[OpenTK.Input.Key.RControl]); //key 10, CTRL
            KeyStates[11] = (kb[OpenTK.Input.Key.LAlt] || kb[OpenTK.Input.Key.RAlt]); //key 11, ALT
            KeyStates[12] = (kb[OpenTK.Input.Key.F5]); //key 12, F5
            KeyStates[13] = (kb[OpenTK.Input.Key.F6]); //key 13, F6
            KeyStates[14] = (kb[OpenTK.Input.Key.F7]); //key 14, F7
            KeyStates[15] = (kb[OpenTK.Input.Key.F8]); //key 15, F8
            KeyStates[16] = (kb[OpenTK.Input.Key.F9]); //key 16, F9
            KeyStates[17] = (kb[OpenTK.Input.Key.F2]); //key 17, SHOW_FPS
            KeyStates[18] = (kb[OpenTK.Input.Key.F12]); //key 18, RESET
        }

        public static bool is_pressed(int code)
        {
            return KeyStates[code];
        }

        public static bool is_triggered(int code)
        {
            return KeyStates[code] && !LastKeyStates[code];
        }

        public static bool is_repeat(int code)
        {
            return KeyStates[code] && LastKeyStates[code];
        }
    }
}
