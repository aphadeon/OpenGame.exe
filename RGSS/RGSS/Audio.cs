using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Audio
{
    public static RPGX.Audio.AudioSample bgm = null;
    public static RPGX.Audio.AudioSample bgs = null;

    public static void setup_mdi()
    {

    }

    public static void bgm_play(string filename, int volume = 100, int pitch = 100, int pos = 0)
    {
        if (String.IsNullOrEmpty(filename)) return;
        string file = Runtime.FindAudioResource(filename);
        if (file == null) return;
        bgm = RPGX.Audio.Audio.Play(file, /* loop */ true, (float) volume / 100f, (float) pitch / 100f);
    }

    public static void bgm_stop()
    {
        if (bgm == null) return;
        if (RPGX.Audio.Audio.GetAudio().soundsplaying.ContainsKey(bgm))
        {
            RPGX.Audio.Audio.GetAudio().soundsplaying[bgm].Stop();
        }
    }

    public static void bgm_fade(int time)
    {

    }

    public static int bgm_pos()
    {
        return 0;
    }

    public static void bgs_play(string filename, int volume = 100, int pitch = 100, int pos = 0)
    {
        if (String.IsNullOrEmpty(filename)) return;
        string file = Runtime.FindAudioResource(filename);
        if (file == null) return;
        bgs = RPGX.Audio.Audio.Play(file, /* loop */ true, (float)volume / 100f, (float)pitch / 100f);
    }

    public static void bgs_stop()
    {
        if (bgs == null) return;
        if (RPGX.Audio.Audio.GetAudio().soundsplaying.ContainsKey(bgs))
        {
            RPGX.Audio.Audio.GetAudio().soundsplaying[bgs].Stop();
        }
    }

    public static void bgs_fade(int time)
    {

    }

    public static int bgs_pos()
    {
        return 0;
    }

    public static void me_play(string filename, int volume = 100, int pitch = 100)
    {
        if (String.IsNullOrEmpty(filename)) return;
        string file = Runtime.FindAudioResource(filename);
        if (file == null) return;
        RPGX.Audio.Audio.Play(file, /* loop */ false, (float)volume / 100f, (float)pitch / 100f);
    }

    public static void me_stop()
    {

    }

    public static void me_fade(int time)
    {

    }

    public static void se_play(string filename, int volume = 100, int pitch = 100)
    {
        if (String.IsNullOrEmpty(filename)) return;
        string file = Runtime.FindAudioResource(filename);
        if (file == null) return;
        RPGX.Audio.Audio.Play(file, /* loop */ false, (float)volume / 100f, (float)pitch / 100f);
    }

    public static void se_stop()
    {

    }

}