using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Plane
{
    public Sprite sprite;
    private Bitmap bmp;

    public Bitmap bitmap
    {
        get { return bmp; }
        set
        {
            int w = sprite.viewport.rect.width;
            int h = sprite.viewport.rect.height;
   
            int nw = value.width() <= 100 ? 2 : 3;
            int nh = value.height() <= 100 ? 2 : 3;
   
            int dx = (int) Math.Max(Math.Ceiling((double) w / value.width()), 1) * nw;
            int dy = (int) Math.Max(Math.Ceiling((double) h / value.height()), 1) * nh;
 
            int bw = dx * value.width();
            int bh = dy * value.height();
 
            bmp = value;
            if(sprite.bitmap != null) sprite.bitmap.dispose();
            sprite.bitmap = new Bitmap(bw, bh);
            for(int x = 0; x < dx; x++){
                for(int y = 0; y < dy; y++){
                    sprite.bitmap.blt(x * value.width(), y * value.height(), bmp, bitmap.rect());
                }
            }
        }
    }



    public int ox
    {
        get { return sprite.ox; }
        set { sprite.ox = (value % (bitmap == null ? 1 : bitmap.width())); }
    }

    public int oy
    {
        get { return sprite.oy; }
        set { sprite.oy = (value % (bitmap == null ? 1 : bitmap.height())); }
    }

    public Plane(Viewport v)
    {
        initialize(v);
    }

    public Plane initialize(Viewport v)
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

