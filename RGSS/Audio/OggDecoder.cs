using csogg;
using csvorbis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGX.Audio
{
    public class OggDecoder
    {
        private static int convsize = 4096 * 2;
        private static byte[] convbuffer = new byte[convsize];

        public AudioSample OggToWav(Stream ogg, float volume, float pitch)
        {
            AudioSample sample = new AudioSample();
            TextWriter s_err = new StringWriter();
            Stream input = null;
            MemoryStream output = null;

            input = ogg;
            output = new MemoryStream();

            SyncState oy = new SyncState();
            StreamState os = new StreamState();
            Page og = new Page();
            Packet op = new Packet();

            Info vi = new Info();
            Comment vc = new Comment();
            DspState vd = new DspState();
            Block vb = new Block(vd);

            byte[] buffer;
            int bytes = 0;
            oy.init();
            while (true)
            {
                int eos = 0;
                int index = oy.buffer(4096);
                buffer = oy.data;
                try
                {
                    bytes = input.Read(buffer, index, 4096);
                }
                catch (Exception e)
                {
                    s_err.WriteLine(e);
                }
                oy.wrote(bytes);
                if (oy.pageout(og) != 1)
                {
                    if (bytes < 4096) break;
                    s_err.WriteLine("Input does not appear to be an Ogg bitstream.");
                }
                os.init(og.serialno());
                vi.init();
                vc.init();
                if (os.pagein(og) < 0)
                {
                    s_err.WriteLine("Error reading first page of Ogg bitstream data.");
                }

                if (os.packetout(op) != 1)
                {
                    s_err.WriteLine("Error reading initial header packet.");
                }

                if (vi.synthesis_headerin(vc, op) < 0)
                {
                    s_err.WriteLine("This Ogg bitstream does not contain Vorbis audio data.");
                }

                int i = 0;
                while (i < 2)
                {
                    while (i < 2)
                    {
                        int result = oy.pageout(og);
                        if (result == 0) break;
                        if (result == 1)
                        {
                            os.pagein(og);
                            while (i < 2)
                            {
                                result = os.packetout(op);
                                if (result == 0) break;
                                vi.synthesis_headerin(vc, op);
                                i++;
                            }
                        }
                    }
                    index = oy.buffer(4096);
                    buffer = oy.data;
                    try
                    {
                        bytes = input.Read(buffer, index, 4096);
                    }
                    catch (Exception e)
                    {
                        s_err.WriteLine(e);
                    }
                    oy.wrote(bytes);
                }
                sample.Channels = vi.channels;
                sample.Rate = (int)((float)vi.rate * pitch);
                convsize = 4096 / vi.channels;
                vd.synthesis_init(vi);
                vb.init(vd);
                float[][][] _pcm = new float[1][][];
                int[] _index = new int[vi.channels];
                while (eos == 0)
                {
                    while (eos == 0)
                    {
                        int result = oy.pageout(og);
                        if (result == 0) break;
                        if (result != -1)
                        {
                            os.pagein(og);
                            while (true)
                            {
                                result = os.packetout(op);
                                if (result == 0) break;
                                if (result != -1)
                                {
                                    int samples;
                                    if (vb.synthesis(op) == 0)
                                    {
                                        vd.synthesis_blockin(vb);
                                    }
                                    while ((samples = vd.synthesis_pcmout(_pcm, _index)) > 0)
                                    {
                                        float[][] pcm = _pcm[0];
                                        int bout = (samples < convsize ? samples : convsize);
                                        for (i = 0; i < vi.channels; i++)
                                        {
                                            int ptr = i * 2;
                                            int mono = _index[i];
                                            for (int j = 0; j < bout; j++)
                                            {
                                                int val = (int)(pcm[i][mono + j] * 32767.0);
                                                if (val > 32767)
                                                {
                                                    val = 32767;
                                                }
                                                if (val < -32768)
                                                {
                                                    val = -32768;
                                                }
                                                val = (int)((float)val * volume);
                                                if (val < 0) val = val | 0x8000;
                                                convbuffer[ptr] = (byte)(val);
                                                convbuffer[ptr + 1] = (byte)((uint)val >> 8);
                                                ptr += 2 * (vi.channels);
                                            }
                                        }
                                        output.Write(convbuffer, 0, 2 * vi.channels * bout);
                                        vd.synthesis_read(bout);
                                    }
                                }
                            }
                            if (og.eos() != 0) eos = 1;
                        }
                    }
                    if (eos == 0)
                    {
                        index = oy.buffer(4096);
                        buffer = oy.data;
                        try
                        {
                            bytes = input.Read(buffer, index, 4096);
                        }
                        catch (Exception e)
                        {
                            s_err.WriteLine(e);
                        }
                        oy.wrote(bytes);
                        if (bytes == 0) eos = 1;
                    }
                }
                os.clear();
                vb.clear();
                vd.clear();
                vi.clear();
                break;
            }

            oy.clear();
            input.Close();
            sample.Pcm = output.ToArray();
            return sample;
        }
    }
}
