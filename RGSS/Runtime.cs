using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class Runtime
{
    public static int RGSSVersion;
    public static string[] ResourcePaths;
    public static int DefaultResolutionWidth;
    public static int DefaultResolutionHeight;

    public static string FindImageResource(string name)
    {
        string[] formats = {".jpg",".jpeg",".png",".gif"};
        foreach (string path in ResourcePaths)
        {
            foreach (string format in formats)
            {
                //Console.WriteLine("Searching for: " + path + name + format);
                if (File.Exists(path + name + format))
                {
                    //Console.WriteLine("Found: " + path + name + format);
                    return path + name + format;
                }
            }
        }
        return null;
    }

    public string FindAudioResource(string name)
    {
        string[] formats = { ".ogg", ".mid", ".midi", ".mp3", ".wav" };
        foreach (string path in ResourcePaths)
        {
            foreach (string format in formats)
            {
                if (File.Exists(path + name + format))
                {
                    return path + name + format;
                }
            }
        }
        return null;
    }
}
