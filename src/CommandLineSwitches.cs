using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGame
{
    class CommandLineSwitches
    {
        private enum Key
        {
            NONE,
            GAME
        }

        private bool EnableConsole = false;
        private bool PlayTest = false;
        private bool BattleTest = false;
        private int ForcedRgss = 0;
        private string GameDirectory = ".";

        public CommandLineSwitches(string[] args)
        {
            Key key = Key.NONE;

            foreach (string arg in args)
            {
                if (key != Key.NONE)
                {
                    switch (key)
                    {
                        case Key.GAME:
                            GameDirectory = ResolvePath(arg);
                            break;
                    }

                    key = Key.NONE;
                }
                else
                {
                    //Key-pair switches denoted with - sign
                    //key is used to evaluate the usage of the next argument
                    if (arg == "-game") key = Key.GAME;

                    if (arg == "console") EnableConsole = true;
                    if (arg == "test") PlayTest = true;
                    if (arg == "btest") BattleTest = true;
                    if (arg == "rgss1") ForcedRgss = 1;
                    if (arg == "rgss2") ForcedRgss = 2;
                    if (arg == "rgss3") ForcedRgss = 3;
                }               
            }

            
            
            if (EnableConsole) ConsoleWindow.Show();
        }

        public int GetForcedRGSSVersion()
        {
            return ForcedRgss;
        }

        public string GetGameDirectory()
        {
            return GameDirectory;
        }

        private static string ResolvePath(string path)
        {
            string s = path;

            //Remove all quotes from the given path
            int q = s.IndexOf('\u0022');
            while (q > 0)
            {
                s.Remove(q);
                q = s.IndexOf('\u0022');
            }

            return s;
        }
    }
}
