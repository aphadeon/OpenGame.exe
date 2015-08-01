using OpenTK.Graphics.OpenGL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Viewport
{
    public bool visible = true;
    private bool disposed = false;
    public int z = 0;
    public int ox = 0;
    public int oy = 0;
    public Tone tone = new Tone();
    public Color color = new Color();
    public DateTime created_at;
    internal bool is_default = false;

    private int x, y, width, height;

    public Rect rect
    {
        get { return new Rect(x, y, width, height); }
        set {
            if (x == rect.x && y == rect.y && width == rect.width && height == rect.height) return;
            x = rect.x;
            y = rect.y;
            width = rect.width;
            height = rect.height;
            rebuild();
        }
    }

    internal int texture = 0;
    internal int fbo = 0;
    internal List<OpenGame.Runtime.Drawable> sprites = new List<OpenGame.Runtime.Drawable>();

    public Viewport()
    {
        initialize(0, 0, Graphics.width, Graphics.height);
    }
    public Viewport(int ax, int ay, int aw, int ah)
    {
        initialize(ax, ay, aw, ah);
    }

    private void initialize(int ax, int ay, int aw, int ah)
    {
        created_at = DateTime.Now;
        x = ax;
        y = ay;
        width = aw;
        height = ah;
        Graphics.viewports.Add(this);
        rebuild();
    }

    public void sort()
    {
        sprites = sprites.OrderBy(s => s.z).ThenBy(s => s.created_at).ToList();
    }

    private void rebuild()
    {
        GL.Enable(EnableCap.Texture2D);
        if (texture != 0)
        {
            GL.DeleteTextures(1, ref texture);
            texture = 0;
        }
        if (fbo != 0)
        {
            GL.DeleteFramebuffers(1, ref fbo);
            fbo = 0;
        }
        //generate texture
        GL.GenTextures(1, out texture);
        GL.BindTexture(TextureTarget.Texture2D, texture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
        //setup the fbo
        fbo = GL.GenFramebuffer();// GL.GenFramebuffers(1, out fbo);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
        GL.BindTexture(TextureTarget.Texture2D, texture);
        GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, texture, 0);
        GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
    }

    public void dispose()
    {
        Graphics.viewports.Remove(this);
        disposed = true;
    }

    internal void draw()
    {
        if (!visible || disposed || fbo == 0 || texture == 0) return;
        if (sprites.Count <= 0)
        {
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.ClearColor(0, 0, 0, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            return;
        }
        //re-render the render-to-texture here from all sprites/planes
        GL.BindTexture(TextureTarget.Texture2D, texture);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
        GL.ClearColor(0, 0, 0, 0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Viewport(0, 0, width, height);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(0, width, 0, height, -1f, 1f);
        if (ox != 0 || oy != 0)
        {
            //Console.WriteLine("Transforming viewport: " + ox + ":" + oy);
            GL.Translate(0 - ox, 0 - oy, 0); //untested
        }

        sort();
        foreach (OpenGame.Runtime.Drawable s in sprites)
        {
            //Console.WriteLine("drawing sprite: " + s.z);
            s.draw();
        }

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.Viewport(0, 0, Graphics.width, Graphics.height);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(0, Graphics.width, Graphics.height, 0, -1, 1);

        GL.Enable(EnableCap.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, texture);

        GL.Color3(1.0f, 1.0f, 1.0f);

        GL.Begin(BeginMode.Quads);

        GL.TexCoord2(0f, 0f);
        GL.Vertex3(x, y, 0.5f);
        GL.TexCoord2(1f, 0f);
        GL.Vertex3(x + width, y, 0.5f);
        GL.TexCoord2(1f, 1f);
        GL.Vertex3(x + width, y + height, 0.5f);
        GL.TexCoord2(0f, 1f);
        GL.Vertex3(x, y + height, 0.5f);

        GL.End();

        //GL.Disable(EnableCap.Texture2D);
    }

    public bool is_disposed()
    {
        return disposed;
    }

    public static string ruby_helper()
    {
        return @"
            class Viewport
                def disposed?
                    return is_disposed
                end
                def flash(color, duration)
                end
                def update
                end
            end
        ";
    }
}

