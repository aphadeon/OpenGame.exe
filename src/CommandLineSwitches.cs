using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGame
{
    public class CommandLineSwitches
    {
        public bool EnableConsole = false;
        public bool PlayTest = false;
        public bool BattleTest = false;

        public CommandLineSwitches(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg == "console") EnableConsole = true;
                if (arg == "test") PlayTest = true;
                if (arg == "btest") BattleTest = true;
            }
            if (EnableConsole) ConsoleWindow.Show();
        }
    }
}
