using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Sprite : OpenGame.Runtime.Drawable
{
    public bool is_plane = false;

    private Viewport vp = null;
    public Viewport viewport
    {
        get { return vp; }
        set
        {
            if(vp == null) //if we were in global
            {
                //remove the sprite from the global drawables if applicable
                if(OG_Graphics.drawables.Contains(this)) OG_Graphics.drawables.Remove(this);
                vp = value;
            } else //we were in a viewport
            {
                if(vp.sprites.Contains(this)) vp.sprites.Remove(this);
                vp = value;
            } 
            if(vp != null)
            {
                vp.add_sprite(this);
            } else
            {
                OG_Graphics.drawables.Add(this);
            }
        }
    }
    private bool disposed = false;
    public Rect src_rect = new Rect(0, 0, 0, 0);
    public int x = 0;
    public int y = 0;
    public int ox = 0;
    public int oy = 0;
    public float zoom_x = 1f;
    public float zoom_y = 1f;
    public float angle = 0f;
    public float wave_amp = 0f;
    public float wave_length = 0f;
    public float wave_speed = 0f;
    public float wave_phase = 0f;
    public bool mirror = false;
    public int bush_depth = 0;
    public int bush_opacity = 128;
    public bool vissi = true;
    public bool visible
    {
        get { return vissi; }
        set { vissi = value; }
    }
    public int opacity = 255;
    public int blend_type = 0;
    private Bitmap bmp = null;
    public Color color = new Color(255, 255, 255, 255);
    public Tone tone = new Tone(0, 0, 0, 0);

    public Bitmap bitmap
    {
        get { return bmp; }
        set
        {
            bmp = value;
            if (bitmap != null) src_rect = new Rect(0, 0, bitmap.width(), bitmap.height());
            else src_rect = new Rect();
        }
    }

    public Sprite()
    {
        initialize(null);
    }

    public Sprite(Viewport v)
    {
        initialize(v);
    }

    public Sprite initialize()
    {
        return initialize(null);
    }

    public Sprite initialize(Viewport v)
    {
        created_at = DateTime.Now;
        viewport = v; //should magically do what's needed
        return this;
    }

    internal override void draw()
    {
        if (!visible || disposed || opacity == 0) return;
        if (bmp == null) return;
        if (bmp.is_disposed() || bmp.txid == 0) return;
        //Console.WriteLine("Drawing sprite at viewport " + viewport.z + " at z " + z);
        //if (is_plane) return; //temp fix
        GL.Enable(EnableCap.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, bitmap.txid);

        OpenTK.Graphics.Color4 c_val = new OpenTK.Graphics.Color4(1.0f, 1.0f, 1.0f, (1f / 255f) * (float)opacity);
        GL.Color4(c_val);

        //determine pixel size in texels
        float texel_x = 1f / (float)bitmap.width();
        float texel_y = 1f / (float)bitmap.height();
        float depth_z = is_plane ? 1f : 0.9f;

        GL.Begin(PrimitiveType.Quads);
        GL.TexCoord2(texel_x * src_rect.x, texel_y * src_rect.y);
        GL.Vertex3(x - ox, y - oy, depth_z);
        GL.TexCoord2(texel_x * (src_rect.width + src_rect.x), texel_y * src_rect.y);
        GL.Vertex3(x - ox + src_rect.width, y - oy, depth_z);
        GL.TexCoord2(texel_x * (src_rect.width + src_rect.x), texel_y * (src_rect.height + src_rect.y));
        GL.Vertex3(x - ox + src_rect.width, y - oy + src_rect.height, depth_z);
        GL.TexCoord2(texel_x * src_rect.x, texel_y * (src_rect.height + src_rect.y));
        GL.Vertex3(x - ox, y - oy + src_rect.height, depth_z);
        GL.End();
    }

    public void dispose()
    {
        if(viewport != null)
        {
            if (viewport.sprites.Contains(this)) viewport.sprites.Remove(this);
        } else
        {
            if (OG_Graphics.drawables.Contains(this)) OG_Graphics.drawables.Remove(this);
        }
        disposed = true;
    }

    public bool is_disposed()
    {
        return disposed;
    }

    public int width()
    {
        return src_rect.width;
    }

    public int height()
    {
        return src_rect.height;
    }

    public static string ruby_helper()
    {
        return @"
            class Sprite
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