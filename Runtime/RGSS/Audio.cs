using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Audio
{
    private static bool ready = false;

    private static WaveOutEvent bgmChannel;
    private static WaveOutEvent seChannel1;
    private static WaveOutEvent seChannel2;
    private static WaveOutEvent seChannel3;
    private static WaveOutEvent seChannel4; // up to 4 SEs at once, for now
    private static int se_channel = 0;
    private static WaveStream bgm_stream;
    private static WaveStream se_stream;

    private static string bgm_file;

    public static void setup()
    {
        ready = true;
        //determine the default device number for audio
        var enumerator = new MMDeviceEnumerator();
        MMDevice def = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        Console.WriteLine("Audio device: " + def.FriendlyName);

        //create devices
        bgmChannel = new WaveOutEvent();
        seChannel1 = new WaveOutEvent();
        seChannel2 = new WaveOutEvent();
        seChannel3 = new WaveOutEvent();
        seChannel4 = new WaveOutEvent();
    }

    private enum Format
    {
        OGG, MP3, WAV, MIDI
    }

    private static Format get_format(string file)
    {
        if (file.EndsWith(".mid") || file.EndsWith(".midi")) return Format.MIDI;
        if (file.EndsWith(".ogg")) return Format.OGG;
        if (file.EndsWith(".mp3")) return Format.MP3;
        return Format.WAV;
    }

    //BACKGROUND MUSIC
    public static void bgm_play(string filename, int volume = 100, int pitch = 100, int pos = 0)
    {
        if (!ready) setup();
        if (String.IsNullOrEmpty(filename)) return;
        string file = OpenGame.Runtime.Runtime.FindAudioResource(filename);
        if (file == null)
        {
            Console.WriteLine("Could not load audio resource: " + filename);
            return;
        }
        Format file_format = get_format(file);
        //Console.WriteLine("Playing file " + file + " of format: " + (int)file_format);
        if (file != bgm_file)
        {
            switch (file_format)
            {
                //no midi yet
                case Format.OGG:
                    bgm_stream = new NAudio.Vorbis.VorbisWaveReader(file);
                    break;
                case Format.MP3:
                    bgm_stream = new Mp3FileReader(file);
                    break;
                case Format.WAV:
                    bgm_stream = new WaveFileReader(file);
                    break;
            }
            if (bgmChannel.PlaybackState == PlaybackState.Playing) bgmChannel.Stop();
            bgmChannel.Init(new WaveChannel32(bgm_stream));
            bgmChannel.Play();
            bgm_file = file;
        }
        bgmChannel.Volume = (float)volume / 200f; //we divide by 200 because volume here is dramatically louder than nilla RGSS
        //pitch is not yet supported
    }

    public static void bgm_stop()
    {
        if (!ready) setup();
        if(bgmChannel != null)
        {
            if (bgmChannel.PlaybackState == PlaybackState.Playing)
            {
                bgmChannel.Stop();
            }
        }
    }

    public static void bgm_fade(int time)
    {
        if (!ready) setup();
    }

    public static int bgm_pos()
    {
        if (!ready) setup();
        return 0;
    }

    //BACKGROUND SOUNDS
    public static void bgs_play(string filename, int volume = 100, int pitch = 100, int pos = 0)
    {
        if (!ready) setup();
        if (String.IsNullOrEmpty(filename)) return;
        string file = OpenGame.Runtime.Runtime.FindAudioResource(filename);
        if (file == null) return;
    }

    public static void bgs_stop()
    {
        if (!ready) setup();
    }

    public static void bgs_fade(int time)
    {
        if (!ready) setup();
    }

    public static int bgs_pos()
    {
        if (!ready) setup();
        return 0;
    }

    //MUSIC EFFECTS
    public static void me_play(string filename, int volume = 100, int pitch = 100)
    {
        if (!ready) setup();
        if (String.IsNullOrEmpty(filename)) return;
        string file = OpenGame.Runtime.Runtime.FindAudioResource(filename);
        if (file == null) return;
    }

    public static void me_stop()
    {
        if (!ready) setup();
    }

    public static void me_fade(int time)
    {
        if (!ready) setup();
    }

    //SOUND EFFECTS
    public static void se_play(string filename, int volume = 100, int pitch = 100)
    {
        if (!ready) setup();
        if (String.IsNullOrEmpty(filename)) return;
        string file = OpenGame.Runtime.Runtime.FindAudioResource(filename);
        if (file == null)
        {
            Console.WriteLine("Could not load audio resource: " + filename);
            return;
        }
        Format file_format = get_format(file);
        //Console.WriteLine("Playing file " + file + " of format: " + (int)file_format);
        switch (file_format)
        {
            //no midi yet
            case Format.OGG:
                se_stream = new NAudio.Vorbis.VorbisWaveReader(file);
                break;
            case Format.MP3:
                se_stream = new Mp3FileReader(file);
                break;
            case Format.WAV:
                se_stream = new WaveFileReader(file);
                break;
        }
        se_channel++;
        if (se_channel > 4) se_channel = 1;
        WaveOutEvent seChannel;
        switch (se_channel) {
            case 2: seChannel = seChannel2; break;
            case 3: seChannel = seChannel3; break;
            case 4: seChannel = seChannel4; break;
            default: seChannel = seChannel1; break;
        }
        if (seChannel.PlaybackState == PlaybackState.Playing) seChannel.Stop();
        seChannel.Init(new WaveChannel32(se_stream));
        seChannel.Volume = (Math.Max(100f,volume) / 200f); //we divide by 200 because volume here is dramatically louder than nilla RGSS
        //pitch is not yet supported
        seChannel.Play();

    }

    public static void se_stop()
    {
        if (!ready) setup();
    }

}