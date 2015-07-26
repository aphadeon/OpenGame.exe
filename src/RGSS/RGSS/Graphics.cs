using OpenTK.Graphics.OpenGL;
using RGSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class Graphics
{
    public static int frame_rate = 60;
    public static int frame_count = 0;
    public static int brightness = 255;
    public static int width = 0;
    public static int height = 0;
    private static DateTime last_frame_time;
    private static TimeSpan frame_time;
    public static bool frozen = false;
    internal static List<Viewport> viewports = new List<Viewport>();
    public static Viewport default_viewport;

    public static void initialize()
    {
        width = (int)Utils.Resolution.X;
        height = (int)Utils.Resolution.Y;
        last_frame_time = DateTime.Now;
        frame_time = new TimeSpan(0, 0, 0, 0, 1000 / frame_rate);
        default_viewport = new Viewport(0, 0, width, height);
        default_viewport.is_default = true;
    }

    public static void sort()
    {
        //viewports = viewports.OrderBy(v => v.z).ToList();
        viewports = viewports.OrderBy(v => v.z).ThenByDescending(v => v.created_at).ToList();
    }

    public static void update()
    {
        TimeSpan span = DateTime.Now.Subtract(Graphics.last_frame_time);
        if (span < frame_time)
        {
            Thread.Sleep(frame_time.Subtract(span));
        }
        Graphics.last_frame_time = DateTime.Now;

        if (!Utils.Window_.IsExiting) Utils.Window_.ProcessEvents();

        frame_count++;
        if (frozen) return;

        
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.ClearColor(0f, 0f, 0f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        sort();
        //Console.WriteLine("new frame ");
        default_viewport.draw();
        foreach (Viewport vp in viewports)
        {
            //Console.WriteLine("drawing viewport: " + vp.z);
            //Console.WriteLine("Viewport z: " + vp.z + (vp.is_default ? " (default)" : ""));
            vp.draw();
        }

        //brightness quad
        if (brightness < 255)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, Graphics.width, Graphics.height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Graphics.width, Graphics.height, 0, -1, 1);

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Color4(0.0f, 0.0f, 0.0f, 1f - ((1f / 255f) * (float)brightness));

            GL.Begin(BeginMode.Quads);

            GL.Vertex3(0, 0, 0.1f);
            GL.Vertex3(width, 0, 0.1f);
            GL.Vertex3(width, height, 0.1f);
            GL.Vertex3(0, height, 0.1f);

            GL.End();
        }

        if (!Utils.Window_.IsExiting) Utils.Window_.SwapBuffers();
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
        width = w; height = h;
        Utils.Resolution.X = w;
        Utils.Resolution.Y = h;
        int x = Utils.Window_.ClientRectangle.Top;
        int y = Utils.Window_.ClientRectangle.Left;
        Utils.Window_.Width = width;
        Utils.Window_.Height = height;
        Utils.Window_.X = x;
        Utils.Window_.Y = y;
        GL.Viewport(0, 0, Utils.Window_.Width, Utils.Window_.Height);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(0, Utils.Resolution.X, Utils.Resolution.Y, 0, -1, 1);
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
        int addition_per_frame = (255 - brightness) / duration_frames;
        int counter = duration_frames;
        while (counter >= 0)
        {
            brightness += addition_per_frame;
            update(); //waits for frame advance already
            counter--;
        }
    }

    public static void reset()
    {
        viewports = new List<Viewport>();
        default_viewport.sprites = new List<Drawable>();
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
        fadein(duration_frames);
    }

    public static Bitmap snap_to_bitmap()
    {
        //temp fix
        return new Bitmap(Math.Max(1, width), Math.Max(1, height));
    }

    public static void play_movie(string filename)
    {

    }
}