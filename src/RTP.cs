using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGame
{
    class RTP
    {
        private string Path = "";

        public RTP(string DefaultLocation)
        {
            //Find RPG Maker VX Ace's RTP using the Windows registry
            if (Program.GetRGSSVersion() == 1)
            {
                string key = @"HKEY_LOCAL_MACHINE\Software\Enterbrain\RGSS\RTP";
                string rtp = (string)Microsoft.Win32.Registry.GetValue(key, "Standard", DefaultLocation);
                if (String.IsNullOrWhiteSpace(rtp))
                {
                    key = @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Enterbrain\RGSS\RTP";
                    rtp = (string)Microsoft.Win32.Registry.GetValue(key, "Standard", DefaultLocation);
                }
                rtp += @"\";
                Path = rtp;
            }
            else if (Program.GetRGSSVersion() == 2)
            {
                string key = @"HKEY_LOCAL_MACHINE\Software\Enterbrain\RGSS2\RTP";
                string rtp = (string)Microsoft.Win32.Registry.GetValue(key, "RPGVX", DefaultLocation);
                if (String.IsNullOrWhiteSpace(rtp))
                {
                    key = @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Enterbrain\RGSS2\RTP";
                    rtp = (string)Microsoft.Win32.Registry.GetValue(key, "RPGVX", DefaultLocation);
                }
                rtp += @"\";
                Path = rtp;
            }
            else if (Program.GetRGSSVersion() == 3)
            {
                string key = @"HKEY_LOCAL_MACHINE\Software\Enterbrain\rgss3\rtp";
                string rtp = (string)Microsoft.Win32.Registry.GetValue(key, "rpgvxace", DefaultLocation);
                if (String.IsNullOrWhiteSpace(rtp))
                {
                    key = @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Enterbrain\rgss3\rtp";
                    rtp = (string)Microsoft.Win32.Registry.GetValue(key, "rpgvxace", DefaultLocation);
                }
                rtp += @"\";
                Path = rtp;
            }
            else
            {
                Path = DefaultLocation;
            }
        }

        public string GetPath()
        {
            return Path;
        }
    }
}
