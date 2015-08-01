using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK.Audio;
using System.Threading;
using System.Diagnostics;
using OpenTK;

namespace OpenGame.Audio
{
    public class AudioOpenAl
    {
        public AudioOpenAl()
        {
            try
            {
                IList<string> x = AudioContext.AvailableDevices; //only with this line an exception can be catched.
                context = new AudioContext();
            }
            catch (Exception e)
            {
                string oalinst = "oalinst.exe";
                if (File.Exists(oalinst))
                {
                    try
                    {
                        Process.Start(oalinst, "/s");
                    }
                    catch
                    {
                    }
                }
                Console.WriteLine(e);
            }
        }

        private AudioContext context;
        // Loads a wave/riff audio file.
        public static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int data_chunk_size = reader.ReadInt32();

                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;

                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        public static OpenTK.Audio.OpenAL.ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1:
                    return bits == 8 ? OpenTK.Audio.OpenAL.ALFormat.Mono8 : OpenTK.Audio.OpenAL.ALFormat.Mono16;
                case 2:
                    return bits == 8 ? OpenTK.Audio.OpenAL.ALFormat.Stereo8 : OpenTK.Audio.OpenAL.ALFormat.Stereo16;
                default:
                    throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

        private Dictionary<string, AudioSample> cache = new Dictionary<string, AudioSample>();

        public class AudioTask
        {
            public AudioTask(AudioSample sample, AudioOpenAl audio, float x, float y, float z)
            {
                this.sample = sample;
                this.audio = audio;
                this.position = new Vector3(x, y, z);
            }

            private AudioOpenAl audio;
            private AudioSample sample;
            public Vector3 position;

            public void Play()
            {
                if (started)
                {
                    shouldplay = true;
                    return;
                }
                started = true;
                ThreadPool.QueueUserWorkItem(delegate { play(); });
            }

            //bool resume = true;
            private bool started = false;

            private void play()
            {
                try
                {
                    DoPlay();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            private void DoPlay()
            {
                int source = OpenTK.Audio.OpenAL.AL.GenSource();
                int state;
                //using ()
                {
                    //Trace.WriteLine("Testing WaveReader({0}).ReadToEnd()", filename);

                    int buffer = OpenTK.Audio.OpenAL.AL.GenBuffer();

                    OpenTK.Audio.OpenAL.AL.BufferData(buffer, GetSoundFormat(sample.Channels, sample.BitsPerSample),
                        sample.Pcm, sample.Pcm.Length, sample.Rate);
                    //audiofiles[filename]=buffer;

                    OpenTK.Audio.OpenAL.AL.DistanceModel(OpenTK.Audio.OpenAL.ALDistanceModel.InverseDistance);
                    OpenTK.Audio.OpenAL.AL.Source(source, OpenTK.Audio.OpenAL.ALSourcef.RolloffFactor, 0.3f);
                    OpenTK.Audio.OpenAL.AL.Source(source, OpenTK.Audio.OpenAL.ALSourcef.ReferenceDistance, 1);
                    OpenTK.Audio.OpenAL.AL.Source(source, OpenTK.Audio.OpenAL.ALSourcef.MaxDistance, (int)(64 * 1));
                    OpenTK.Audio.OpenAL.AL.Source(source, OpenTK.Audio.OpenAL.ALSourcei.Buffer, buffer);
                    OpenTK.Audio.OpenAL.AL.Source(source, OpenTK.Audio.OpenAL.ALSource3f.Position, position.X,
                        position.Y, position.Z);
                    OpenTK.Audio.OpenAL.AL.SourcePlay(source);

                    // Query the source to find out when it stops playing.
                    for (; ; )
                    {
                        OpenTK.Audio.OpenAL.AL.GetSource(source, OpenTK.Audio.OpenAL.ALGetSourcei.SourceState, out state);

                        if ((!loop) &&
                            (OpenTK.Audio.OpenAL.ALSourceState)state != OpenTK.Audio.OpenAL.ALSourceState.Playing)
                        {
                            break;
                        }
                        if (loop)
                        {
                            
                            if (state == (int)OpenTK.Audio.OpenAL.ALSourceState.Playing && (!shouldplay))
                            {
                                OpenTK.Audio.OpenAL.AL.SourcePause(source);
                            }
                            if (state != (int)OpenTK.Audio.OpenAL.ALSourceState.Playing && (shouldplay))
                            {
                                if (restart)
                                {
                                    OpenTK.Audio.OpenAL.AL.SourceRewind(source);
                                    restart = false;
                                }
                                OpenTK.Audio.OpenAL.AL.SourcePlay(source);
                            }
                            
                        }
                        Thread.Sleep(1);
                    }
                    OpenTK.Audio.OpenAL.AL.SourceStop(source);
                    OpenTK.Audio.OpenAL.AL.DeleteSource(source);
                    OpenTK.Audio.OpenAL.AL.DeleteBuffer(buffer);
                }
            }

            public bool loop = false;
            public bool shouldplay;
            public bool restart;

            public void Restart()
            {
                restart = true;
            }

            public void Stop()
            {
                shouldplay = false;
            }
        }

        public AudioSample GetSampleFromArray(byte[] data, float volume, float pitch)
        {
            Stream stream = new MemoryStream(data);
            if (stream.ReadByte() == 'R'
                && stream.ReadByte() == 'I'
                && stream.ReadByte() == 'F'
                && stream.ReadByte() == 'F')
            {
                stream.Position = 0;
                int channels, bits_per_sample, sample_rate;
                byte[] sound_data = LoadWave(stream, out channels, out bits_per_sample, out sample_rate);
                AudioSample sample = new AudioSample()
                {
                    Pcm = sound_data,
                    BitsPerSample = bits_per_sample,
                    Channels = channels,
                    Rate = sample_rate,
                };
                return sample;
            }
            else
            {
                stream.Position = 0;
                AudioSample sample = new OggDecoder().OggToWav(stream, volume, pitch);
                return sample;
            }
        }

        public void Play(AudioSample sample, float x, float y, float z)
        {
            if (sample == null)
            {
                return;
            }
            if (context == null)
            {
                return;
            }
            new AudioTask(sample, this, x, y, z).Play();
        }

        public Dictionary<AudioSample, AudioTask> soundsplaying = new Dictionary<AudioSample, AudioTask>();

        public void PlayLoop(AudioSample sample, float x, float y, float z)
        {
            PlayLoop(sample, true, false, x, y, z);
        }

        public void PlayLoop(AudioSample sample, bool play, bool restart, float x, float y, float z)
        {
            if (sample == null)
            {
                return;
            }
            if (context == null)
            {
                return;
            }
            if (play)
            {
                if (!soundsplaying.ContainsKey(sample))
                {
                    soundsplaying[sample] = new AudioTask(sample, this, x, y, z);
                    soundsplaying[sample].loop = true;
                }
                if (restart)
                {
                    soundsplaying[sample].Restart();
                }
                soundsplaying[sample].shouldplay = true;
                soundsplaying[sample].Play();
            }
            else
            {
                if (soundsplaying.ContainsKey(sample))
                {
                    soundsplaying[sample].shouldplay = false;
                }
            }
        }
    }

    public class AudioSample
    {
        public byte[] Pcm;
        public int Channels;
        public int Rate;
        public int BitsPerSample = 16;
    }


}
