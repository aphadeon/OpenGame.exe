using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Window : Drawable
{
    public int openness
    {
        get { return openni; }
        set { openni = value; }
    }
    public int openni = 255;
    private bool disposed = false;
    public int x = -1;
    public int y = -1;

    public int tw = Graphics.width;
    public int th = Graphics.height;
    public int width
    {
        get { return tw; }
        set { tw = value; move(x, y, tw, th); }
    }
    public int height
    {
        get { return th; }
        set { th = value; move(x, y, tw, th); }
    }
    public Bitmap windowskin;
    public Bitmap cbmp = null;
    public Bitmap contents
    {
        get { return cbmp; }
        set { cbmp = value; }
    }
    public Rect cursor_rect = new Rect(0, 0, 1, 1);
    public bool accti = true;
    public bool active
    {
        get { return accti; }
        set { accti = value; }
    }
    public bool vissi = true;
    public bool visible
    {
        get { return vissi; }
        set { vissi = value; }
    }
    public bool arrows_visible = true;
    public bool pause = false;
    public float ox = 0f;
    public float oy = 0f;
    private int pad = 12;
    public int padding
    {
        get { return pad; }
        set
        {
            pad = value;
            padding_bottom = value;
        }
    }
    public int padding_bottom = 12;
    public int opacity = 255;
    public int back_opacity = 192;
    public int contents_o = 255;
    public int contents_opacity
    {
        get { return contents_o; }
        set { contents_o = value; }
    }
    public Tone tone = new Tone();
    private float cursor_opacity = 0.8f;
    private bool cursor_opacity_x = true;

    private Viewport vp = null;
    public Viewport viewport
    {
        get { return vp; }
        set
        {
            if (vp != null) vp.sprites.Remove(this);
            vp = value;
            vp.sprites.Add(this);
        }
    }

    public Window()
    {
        created_at = DateTime.Now;
        z = 100;
        move(0, 0, Graphics.width, Graphics.height);
        viewport = Graphics.default_viewport;
        viewport.sprites.Add(this);
    }

    public Window(int ax, int ay, int aw, int ah)
    {
        created_at = DateTime.Now;
        z = 100;
        move(ax, ay, aw, ah);
        viewport = Graphics.default_viewport;
        viewport.sprites.Add(this);
    }

    //inheritance constructor
    public void initialize(int ax, int ay, int aw, int ah)
    {
        created_at = DateTime.Now;
        z = 100;
        move(ax, ay, aw, ah);
        viewport = Graphics.default_viewport;
        viewport.sprites.Add(this);
    }

    public void update()
    {
        if (!active) return;
        if (cursor_opacity_x)
        {
            cursor_opacity -= 0.025f;
            if (cursor_opacity < 0.2f) cursor_opacity_x = false;
        }
        else
        {
            cursor_opacity += 0.025f;
            if (cursor_opacity > 0.7f) cursor_opacity_x = true;
        }
    }

    public void move(int ax, int ay, int aw, int ah)
    {
        if (x == ax && y == ay && tw == aw && th == ah) return;
        if (cbmp == null)
        {
            x = ax; y = ay; tw = aw; th = ah;
            cbmp = new Bitmap(aw, ah);
            return;
        } else
        {
            Bitmap tmpb = new Bitmap(aw, ah);
            tmpb.blt(0, 0, cbmp, new Rect(0, 0, Math.Min(aw, cbmp.width()), Math.Min(ah, cbmp.height())));
            cbmp = tmpb;
            x = ax; y = ay; tw = aw; th = ah;
        }
    }

    public void dispose()
    {
        viewport.sprites.Remove(this);
        disposed = true;
    }

    public bool is_disposed()
    {
        return disposed;
    }

    public bool is_open()
    {
        return (openness >= 255);
    }

    public bool is_closed()
    {
        return (openness <= 0);
    }

    public override void draw()
    {
        //Console.WriteLine("Drawing window at viewport " + viewport.z + " at z " + z);
        //correct openness
        if (openness > 255) openness = 255;
        if (openness < 0) openness = 0;
        if (!visible || opacity <= 0 || disposed) return;

        //windowskin
        if (windowskin != null && windowskin.txid != 0 && back_opacity > 0 && openness >= 0)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, windowskin.txid);

            GL.Color4(1.0f, 1.0f, 1.0f, (1f / 255f) * (float)back_opacity);

            //clone position values to mod them for openness
            int mod_y = (int)(((1f / 255f) * (float) openness) * height);
            int aax = x;
            int aay = y + (height / 2) - (mod_y / 2);
            int aaw = width;
            int aah = mod_y;
            if (mod_y > 1)
            {
                GL.Begin(BeginMode.Quads);

                //background layer 1
                GL.TexCoord2(0f, 0f);
                GL.Vertex3(aax + 2, aay + 2, 0.2f);
                GL.TexCoord2(0.5f, 0f);
                GL.Vertex3(aax + aaw - 2, aay + 2, 0.2f);
                GL.TexCoord2(0.5f, 0.5f);
                GL.Vertex3(aax + aaw - 2, aay + aah - 2, 0.2f);
                GL.TexCoord2(0f, 0.5f);
                GL.Vertex3(aax + 2, aay + aah - 2, 0.2f);

                //background layer 2 - NEEDS TO BE TILED
                GL.TexCoord2(0f, 0.5f);
                GL.Vertex3(aax + 2, aay + 2, 0.2f);
                GL.TexCoord2(0.5f, 0.5f);
                GL.Vertex3(aax + aaw - 2, aay + 2, 0.2f);
                GL.TexCoord2(0.5f, 1f);
                GL.Vertex3(aax + aaw - 2, aay + aah - 2, 0.2f);
                GL.TexCoord2(0f, 1f);
                GL.Vertex3(aax + 2, aay + aah - 2, 0.2f);

                //corners
                GL.Color4(1.0f, 1.0f, 1.0f, 1f);
                //topleft corner
                float uvtile = 0.125f; // 1f / 8f
                GL.TexCoord2(0.5f, 0f);
                GL.Vertex3(aax, aay, 0.2f);
                GL.TexCoord2(0.5f + uvtile, 0f);
                GL.Vertex3(aax + 16, aay, 0.2f);
                GL.TexCoord2(0.5f + uvtile, uvtile);
                GL.Vertex3(aax + 16, aay + 16, 0.2f);
                GL.TexCoord2(0.5f, uvtile);
                GL.Vertex3(aax, aay + 16, 0.2f);

                //topright corner
                GL.TexCoord2(1f - uvtile, 0f);
                GL.Vertex3(aax + aaw - 16, aay, 0.2f);
                GL.TexCoord2(1f, 0f);
                GL.Vertex3(aax + aaw, aay, 0.2f);
                GL.TexCoord2(1f, uvtile);
                GL.Vertex3(aax + aaw, aay + 16, 0.2f);
                GL.TexCoord2(1f - uvtile, uvtile);
                GL.Vertex3(aax + aaw - 16, aay + 16, 0.2f);

                //bottomleft corner
                GL.TexCoord2(0.5f, 0.5f - uvtile);
                GL.Vertex3(aax, aay + aah - 16, 0.2f);
                GL.TexCoord2(0.5f + uvtile, 0.5f - uvtile);
                GL.Vertex3(aax + 16, aay + aah - 16, 0.2f);
                GL.TexCoord2(0.5f + uvtile, 0.5f);
                GL.Vertex3(aax + 16, aay + aah, 0.2f);
                GL.TexCoord2(0.5f, 0.5f);
                GL.Vertex3(aax, aay + aah, 0.2f);

                //bottomright corner
                GL.TexCoord2(1f - uvtile, 0.5f - uvtile);
                GL.Vertex3(aax + aaw - 16, aay + aah - 16, 0.2f);
                GL.TexCoord2(1f, 0.5f - uvtile);
                GL.Vertex3(aax + aaw, aay + aah - 16, 0.2f);
                GL.TexCoord2(1f, 0.5f);
                GL.Vertex3(aax + aaw, aay + aah, 0.2f);
                GL.TexCoord2(1f - uvtile, 0.5f);
                GL.Vertex3(aax + aaw - 16, aay + aah, 0.2f);

                //top edge
                GL.TexCoord2(0.5f + uvtile, 0f);
                GL.Vertex3(aax + 16, aay, 0.2f);
                GL.TexCoord2(0.5f + uvtile * 3, 0f);
                GL.Vertex3(aax + aaw - 16, aay, 0.2f);
                GL.TexCoord2(0.5f + uvtile * 3, uvtile);
                GL.Vertex3(aax + aaw - 16, aay + 16, 0.2f);
                GL.TexCoord2(0.5f + uvtile, uvtile);
                GL.Vertex3(aax + 16, aay + 16, 0.2f);

                //left edge
                GL.TexCoord2(0.5f, uvtile);
                GL.Vertex3(aax, aay + 16, 0.2f);
                GL.TexCoord2(0.5f + uvtile, uvtile);
                GL.Vertex3(aax + 16, aay + 16, 0.2f);
                GL.TexCoord2(0.5f + uvtile, uvtile * 3);
                GL.Vertex3(aax + 16, aay + aah - 16, 0.2f);
                GL.TexCoord2(0.5f, uvtile * 3);
                GL.Vertex3(aax, aay + aah - 16, 0.2f);

                //bottom edge
                GL.TexCoord2(0.5f + uvtile, uvtile * 3);
                GL.Vertex3(aax + 16, aay + aah - 16, 0.2f);
                GL.TexCoord2(0.5f + uvtile * 3, uvtile * 3);
                GL.Vertex3(aax + aaw - 16, aay + aah - 16, 0.2f);
                GL.TexCoord2(0.5f + uvtile * 3, uvtile * 4);
                GL.Vertex3(aax + aaw - 16, aay + aah, 0.2f);
                GL.TexCoord2(0.5f + uvtile, uvtile * 4);
                GL.Vertex3(aax + 16, aay + aah, 0.2f);

                //right edge
                GL.TexCoord2(0.5f + uvtile * 3, uvtile);
                GL.Vertex3(aax + aaw - 16, aay + 16, 0.2f);
                GL.TexCoord2(1f, uvtile);
                GL.Vertex3(aax + aaw, aay + 16, 0.2f);
                GL.TexCoord2(1f, uvtile * 3);
                GL.Vertex3(aax + aaw, aay + aah - 16, 0.2f);
                GL.TexCoord2(0.5f + uvtile * 3, uvtile * 3);
                GL.Vertex3(aax + aaw - 16, aay + aah - 16, 0.2f);

                //end
                GL.End();
            }
        }
        //contents
        if (contents != null && contents.txid != 0 && contents_opacity > 0 && openness >= 255)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, contents.txid);

            GL.Color4(1.0f, 1.0f, 1.0f, (1f / 255f) * (float) contents_opacity);

            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0f, 0f);
            GL.Vertex3(x - ox + padding, y - oy + padding, 0.15f);
            GL.TexCoord2(1f, 0f);
            GL.Vertex3(x - ox + contents.width() + padding, y - oy + padding, 0.15f);
            GL.TexCoord2(1f, 1f);
            GL.Vertex3(x - ox + contents.width() + padding, y - oy + contents.height() + padding, 0.15f);
            GL.TexCoord2(0f, 1f);
            GL.Vertex3(x - ox + padding, y - oy + contents.height() + padding, 0.15f);
            GL.End();

            //cursor
            GL.Color4(1.0f, 1.0f, 1.0f, cursor_opacity);
            GL.BindTexture(TextureTarget.Texture2D, windowskin.txid);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0.5f, 0.5f);
            GL.Vertex3(x + cursor_rect.x + padding, y + cursor_rect.y + padding, 0.15f);
            GL.TexCoord2(0.75f, 0.5f);
            GL.Vertex3(x + cursor_rect.x + cursor_rect.width + padding, y + cursor_rect.y + padding, 0.15f);
            GL.TexCoord2(0.75f, 0.75f);
            GL.Vertex3(x + cursor_rect.x + cursor_rect.width + padding, y + cursor_rect.y + cursor_rect.height + padding, 0.15f);
            GL.TexCoord2(0.5f, 0.75f);
            GL.Vertex3(x + cursor_rect.x + padding, y + cursor_rect.y + cursor_rect.height + padding, 0.15f);

            GL.End();
        }
    }

    internal static string ruby_helper()
    {
        return @"
            class Window
                def disposed?
                    return is_disposed
                end

                def open?
                    return is_open
                end

                def close?
                    return is_closed
                end
            end
        ";
    }
}