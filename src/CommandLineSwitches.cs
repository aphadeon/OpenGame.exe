using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGame
{
    class CommandLineSwitches
    {
        private bool EnableConsole = false;
        private bool PlayTest = false;
        private bool BattleTest = false;
        private int ForcedRgss = 0;

        public CommandLineSwitches(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg == "console") EnableConsole = true;
                if (arg == "test") PlayTest = true;
                if (arg == "btest") BattleTest = true;
                if (arg == "rgss1") ForcedRgss = 1;
                if (arg == "rgss2") ForcedRgss = 2;
                if (arg == "rgss3") ForcedRgss = 3;
            }
            if (EnableConsole) ConsoleWindow.Show();
        }

        public int GetForcedRGSSVersion()
        {
            return ForcedRgss;
        }
    }
}
