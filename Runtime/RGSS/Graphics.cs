using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

public class OG_Graphics
{
    public static int frame_rate = OpenGame.Runtime.Runtime.RGSSVersion == 1 ? 40 : 60;
    public static int frame_count = 0;
    public static int brightness = 255;
    public static int width = 0;
    public static int height = 0;
    private static DateTime last_frame_time;
    private static TimeSpan frame_time;
    public static bool frozen = false;
    internal static List<OpenGame.Runtime.Drawable> drawables = new List<OpenGame.Runtime.Drawable>();
    //public static Viewport default_viewport;
    public static GameWindow Window;
    public static float dpi_scale = 1f;

    internal static ConcurrentQueue<Action> deferredActions = new ConcurrentQueue<Action>();
    internal static bool hasDeferredActions = false;
    internal static int mainThreadId;

    public static void initialize(GameWindow win)
    {
        mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

        Window = win;
        width = OpenGame.Runtime.Runtime.DefaultResolutionWidth;
        height = OpenGame.Runtime.Runtime.DefaultResolutionHeight;
        last_frame_time = DateTime.Now;
        frame_time = new TimeSpan(0, 0, 0, 0, 1000 / frame_rate);

    }

    public static void set_frame_rate(int fps)
    {
        frame_rate = fps;
        frame_time = new TimeSpan(0, 0, 0, 0, 1000 / frame_rate);
    }

    public static void sort()
    {
        //viewports = viewports.OrderBy(v => v.z).ToList();
        drawables = drawables.OrderBy(v => v.z).ThenBy(v => v.created_at).ToList();
    }

    //Internal method that runs all pending deferred actions
    internal static void deferred_action_run()
    {
        if (hasDeferredActions)
        {
            Action action;
            while (deferredActions.TryDequeue(out action))
            {
                action();
            }

            hasDeferredActions = false;
        }
    }

    //Actions to happen on Graphics.update added here
    public static void deferred_action_add(Action action)
    {
        if (System.Threading.Thread.CurrentThread.ManagedThreadId == mainThreadId)
        {
            action(); //Run the action if we're already on Main thread
        }
        else
        {
            deferredActions.Enqueue(action);
            hasDeferredActions = true;
        }
    }

    public static void update()
    {
        deferred_action_run(); //Run deferred actions

        TimeSpan span = DateTime.Now.Subtract(OG_Graphics.last_frame_time);
        if (span < frame_time)
        {
            Thread.Sleep(frame_time.Subtract(span));
        }
        OG_Graphics.last_frame_time = DateTime.Now;

        if (!Window.IsExiting) Window.ProcessEvents();

        frame_count++;
        if (frozen) return;


        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.ClearColor(0f, 0f, 0f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        sort();

        foreach (OpenGame.Runtime.Drawable drawable in drawables)
        {
            drawable.draw();
        }


        //brightness quad
        if (brightness < 255)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, OG_Graphics.width, OG_Graphics.height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, OG_Graphics.width, OG_Graphics.height, 0, -1, 1);

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Color4(0.0f, 0.0f, 0.0f, 1f - ((1f / 255f) * (float)brightness));

            GL.Begin(PrimitiveType.Quads);

            GL.Vertex3(0, 0, 0.1f);
            GL.Vertex3(width, 0, 0.1f);
            GL.Vertex3(width, height, 0.1f);
            GL.Vertex3(0, height, 0.1f);

            GL.End();
        }

        if (!Window.IsExiting) Window.SwapBuffers();
    }

    public static void wait(int duration_frames)
    {
        int counter = duration_frames;
        while (counter >= 0)
        {
            update(); //waits for frame advance already
            counter--;
        }
    }

    public static void resize_screen(int w, int h)
    {
        width = w;
        height = h;
        int x = Window.ClientRectangle.Top;
        int y = Window.ClientRectangle.Left;
        Window.Width = width;
        Window.Height = height;
        System.Drawing.Size client_size = Window.ClientSize;
        client_size.Width = width;
        client_size.Height = height;
        Window.ClientSize = client_size;
        Window.X = x;
        Window.Y = y;
        Console.WriteLine("Screen size after resize: " + Window.ClientSize.Width + "x" + Window.ClientSize.Height);
        GL.Viewport(0, 0, width, height);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(0, width, height, 0, -1, 1);
    }

    public static void frame_reset()
    {
        last_frame_time = DateTime.Now;
    }

    public static void fadeout(int duration_frames)
    {
        int reduction_per_frame = brightness / duration_frames;
        int counter = duration_frames + 1;
        while (counter >= 0)
        {
            brightness -= reduction_per_frame;
            if (brightness < 0)
            {
                brightness = 0;
                update();
                break;
            }
            update(); //waits for frame advance already
            counter--;
        }
    }

    public static void fadein(int duration_frames)
    {
        if (duration_frames == 0)
        {
            brightness = 255;
        }
        else
        {
            int addition_per_frame = (255 - brightness) / duration_frames;
            int counter = duration_frames;
            while (counter >= 0)
            {
                brightness += addition_per_frame;
                update(); //waits for frame advance already
                counter--;
            }
        }
    }

    public static void reset()
    {
        drawables = new List<OpenGame.Runtime.Drawable>();
    }

    public static void freeze()
    {
        frozen = true;
    }

    public static void transition()
    {
        transition(10);
    }

    public static void transition(int duration_frames)
    {
        transition(duration_frames, null);
    }

    public static void transition(int duration_frames, string filename)
    {
        transition(duration_frames, filename, 40);
    }

    //TODO - HERE BELOW
    public static void transition(int duration_frames, string filename, int vague)
    {
        //perform transition here
        frozen = false;
        //temp "transitionish"
        //brightness = 255;
        if(duration_frames > 0) fadein(duration_frames);
    }

    public static Bitmap snap_to_bitmap()
    {
        //generate a new bitmap
        Bitmap bmp = new Bitmap(Math.Max(1, width), Math.Max(1, height));
        //read the screen to the bitmap's texture
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        GL.ReadBuffer(ReadBufferMode.Front);
        GL.Enable(EnableCap.Texture2D);

        GL.BindTexture(TextureTarget.Texture2D, bmp.txid);
        GL.CopyTexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, 0, 0, bmp.width(), bmp.height());
        GL.BindTexture(TextureTarget.Texture2D, 0);

        GL.Disable(EnableCap.Texture2D);
        GL.ReadBuffer(ReadBufferMode.None);
        
        //read the bitmap's texture to the bitmap itself
        int fboId;
        GL.Ext.GenFramebuffers(1, out fboId);
        GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboId);
        GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, bmp.txid, 0);
        var bits = bmp.bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.width(), bmp.height()), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        GL.ReadPixels(0, 0, bmp.width(), bmp.height(), OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bits.Scan0);
        GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
        GL.Ext.DeleteFramebuffers(1, ref fboId);
        bmp.bmp.UnlockBits(bits);

        //flip and sync
        bmp.bmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
        OG_Graphics.deferred_action_add(bmp.syncBitmap);

        //return the bmp
        return bmp;
    }

    public static void play_movie(string filename)
    {

    }

    public static string ruby_helper()
    {
        return @"
            module Graphics
                class << self
                    def frame_rate
                        OG_Graphics.frame_rate
                    end
                    def frame_rate= val
                        OG_Graphics.set_frame_rate(val)
                    end
                    def frame_count
                        OG_Graphics.frame_count
                    end
                    def frame_count= val
                        OG_Graphics.frame_count = val
                    end
                    def brightness
                        OG_Graphics.brightness
                    end
                    def brightness= val
                        OG_Graphics.brightness = val
                    end
                    def update
                        OG_Graphics.update
                    end
                    def wait(dur)
                        OG_Graphics.wait(dur)
                    end
                    def fadeout(dur)
                        OG_Graphics.fadeout(dur)
                    end
                    def fadein(dur)
                        OG_Graphics.fadein(dur)
                    end
                    def freeze
                        OG_Graphics.freeze
                    end
                    def transition(duration = ($RGSS_VERSION == 1 ? 8 : 10), filename = nil, vague = nil)
                        if(!vague.nil?)
                            OG_Graphics.transition(duration, filename, vague)
                        elsif(!filename.nil?)
                            OG_Graphics.transition(duration, filename)
                        else
                            OG_Graphics.transition(duration)
                        end
                    end
                    def snap_to_bitmap
                        OG_Graphics.snap_to_bitmap
                    end
                    def frame_reset
                        OG_Graphics.frame_reset
                    end
                    def width
                        OG_Graphics.width
                    end
                    def height
                        OG_Graphics.height
                    end
                    def resize_screen(w, h)
                        OG_Graphics.resize_screen(w, h)
                    end
                    def play_movie(f)
                        OG_Graphics.play_movie(f)
                    end
                end
            end
        ";
    }
}