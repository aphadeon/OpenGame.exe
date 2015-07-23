using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenGame
{
    class ConsoleWindow
    {
        public static bool ConsoleVisible = false;

        public static void Show()
        {
            if (ConsoleVisible) return;
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
            ConsoleVisible = true;
        }

        public static void Hide()
        {
            if (!ConsoleVisible) return;
            var handle = GetConsoleWindow();

            ShowWindow(handle, SW_HIDE);
            ConsoleVisible = false;
        }

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
