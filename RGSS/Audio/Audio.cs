using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio;
using OpenTK;
using System.IO;


namespace RPGX.Audio
{
    class Audio
    {
        private static AudioOpenAl audio;
        private static bool setup = false;
        private static Dictionary<string, AudioSample> audiosamples = new Dictionary<string, AudioSample>();

        public static AudioOpenAl GetAudio()
        {
            return audio;
        }

        private static void Setup()
        {
            if (audio == null)
            {
                audio = new AudioOpenAl();
            }
            setup = true;
        }

        private static AudioSample Load(byte[] data, float volume, float pitch)
        {
            if(!setup) Setup();
            return audio.GetSampleFromArray(data, volume, pitch);
        }

        private static AudioSample GetAudioSample(string file, float volume, float pitch)
        {
            string key = file + "." + volume.ToString("0.00") + "." + pitch.ToString("0.00");
            if (!audiosamples.ContainsKey(key))
            {
                byte[] data = GetFile(file);
                AudioSample sample = Load(data, volume, pitch);
                audiosamples[key] = sample;
            }

            AudioSample ret = audiosamples[key];
            return ret;
        }

        private static byte[] GetFile(string p)
        {
            return File.ReadAllBytes(p);
        }


        public static AudioSample Play(string f, bool loop, float volume, float pitch, float x = 0f, float y = 0f, float z = 0f)
        {
            if (!setup) Setup();
            AudioSample aas = GetAudioSample(f, volume, pitch);
            if (loop) audio.PlayLoop(aas, x, y, z);
            else audio.Play(aas, x, y, z);
            return aas;
        }
    }
}
