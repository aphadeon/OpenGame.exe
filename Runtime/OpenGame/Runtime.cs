using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenGame.Runtime
{
    public class Runtime
    {
        public static int RGSSVersion;
        public static string[] ResourcePaths;
        public static int DefaultResolutionWidth;
        public static int DefaultResolutionHeight;

        public static string FindImageResource(string name)
        {
            //test if the filename already has an extension
            bool hasExtension = false;
            if (name.EndsWith(".jpg") || name.EndsWith(".jpeg") || name.EndsWith(".png") || name.EndsWith(".gif")) hasExtension = true;
            string[] formats = { ".jpg", ".jpeg", ".png", ".gif" };
            foreach (string path in ResourcePaths)
            {
                if (hasExtension)
                {
                    //Console.WriteLine("Searching for: " + path + name + format);
                    if (File.Exists(path + name))
                    {
                        //Console.WriteLine("Found: " + path + name + format);
                        return path + name;
                    }
                }
                else
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
            }
            return null;
        }

        public static string FindAudioResource(string name)
        {
            string[] formats = { ".ogg", ".mid", ".midi", ".mp3", ".wav" };
            string[] supported_formats = { ".ogg" };
            foreach (string path in ResourcePaths)
            {
                foreach (string format in formats)
                {
                    if (File.Exists(path + name + format))
                    {
                        if (!supported_formats.Contains(format))
                        {
                            Console.WriteLine("Unsupported audio format: " + path + name + format);
                            return null;
                        }
                        else
                        {
                            return path + name + format;
                        }
                    }
                }
            }
            return null;
        }
    }
}