using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGame
{
    public class RTP
    {
        private string Path = "";

        public RTP(string DefaultLocation)
        {
            //TODO: Find other RTPs
            //Find RPG Maker VX Ace's RTP using the Windows registry
            if (Program.GetRGSSVersion() == 3)
            {
                string key = @"HKEY_LOCAL_MACHINE\Software\Enterbrain\rgss3\rtp";
                string rtp = (string)Microsoft.Win32.Registry.GetValue(key, "rpgvxace", DefaultLocation);
                rtp += @"\";
                Path = rtp;
            }
        }

        public string GetPath()
        {
            return Path;
        }
    }
}
