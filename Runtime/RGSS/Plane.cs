using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Plane
{
    public Sprite sprite;
    private Bitmap bmp;
    private int ox_r = 0;
    private int oy_r = 0;

    public Bitmap bitmap
    {
        get { return bmp; }
        set
        {
            //get the viewport size
            int w, h;
            if (sprite.viewport != null)
            {
                w = sprite.viewport.rect.width;
                h = sprite.viewport.rect.height;
            } else
            {
                w = OG_Graphics.width;
                h = OG_Graphics.height;
            }
            //determine the number of times to tile the image (3x3 viewport rects)
            int dx = (int) Math.Max(Math.Ceiling((double) w / value.width()), 1) * 3;
            int dy = (int) Math.Max(Math.Ceiling((double) h / value.height()), 1) * 3;
 
            //create the container bitmap which will hold several tiles of the source bitmap
            int bw = dx * value.width();
            int bh = dy * value.height();
 
            bmp = value;
            if(sprite.bitmap != null) sprite.bitmap.dispose();
            sprite.bitmap = new Bitmap(bw, bh);

            //tile the source bitmap over the container bitmap
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(sprite.bitmap.bmp);
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            for (int x = 0; x < dx; x++){
                for(int y = 0; y < dy; y++){
                    System.Drawing.Rectangle source = new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), new System.Drawing.Size(bmp.bmp.Width, bmp.bmp.Height));
                    System.Drawing.Rectangle destination = new System.Drawing.Rectangle(new System.Drawing.Point(x * bmp.width(), y * bmp.height()), new System.Drawing.Size(bmp.width(), bmp.height()));
                    g.DrawImage(bmp.bmp, destination, source, System.Drawing.GraphicsUnit.Pixel);
                }
            }
            g.Dispose();
            OG_Graphics.deferred_action_add(sprite.bitmap.syncBitmap);

            //reset ox and oy
            //instead of resetting to 0, we reset to 1/3rd in
            ox_r = bw / 3;
            oy_r = bh / 3;
            sprite.ox = ox_r;
            sprite.oy = oy_r;
        }
    }



    public int ox
    {
        get { return sprite.ox; }
        set {
            sprite.ox = value;
            while (sprite.ox > ox_r * 2) sprite.ox -= ox_r;
            while (sprite.ox <= 0) sprite.ox += ox_r;
        }
    }

    public int oy
    {
        get { return sprite.oy; }
        set
        {
            sprite.oy = value;
            while (sprite.oy > oy_r * 2) sprite.oy -= oy_r;
            while (sprite.oy <= 0) sprite.oy += ox_r;
        }
    }

    public Plane(Viewport v = null)
    {
        initialize(v);
    }

    public Plane initialize(Viewport v = null)
    {
        sprite = new Sprite(v);
        return this;
    }

    public void dispose()
    {
        if (sprite == null) return;
        if (sprite.is_disposed()) return;
        if (sprite.bitmap != null && !sprite.bitmap.is_disposed()) sprite.bitmap.dispose();
        sprite.dispose();
    }

    public bool is_disposed()
    {
        if (sprite == null) return true;
        if (sprite.is_disposed()) return true;
        return false;
    }

    public static string ruby_helper()
    {
        return @"
            class Plane
              def disposed?
                return is_disposed
              end

              def method_missing(sym, *argv, &argb)
                if sprite.respond_to?(sym)
                  return sprite.send(sym, *argv, &argb)
                end
                super(sym, *argv, &argb)
              end
            end
        ";
    }
}

