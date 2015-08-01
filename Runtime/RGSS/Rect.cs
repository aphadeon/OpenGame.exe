using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Rect
{
    public int x;
    public int y;
    public int width;
    public int height;

    public Rect()
    {
        set(0, 0, 0, 0);
    }

    public Rect(int x, int y)
    {
        set(x, y, 0, 0);
    }

    public Rect(int x, int y, int width, int height)
    {
        set(x, y, width, height);
    }

    public void set(Rect r)
    {
        set(r.x, r.y, r.width, r.height);
    }

    public void set(int ax, int ay, int aw, int ah){
        x = ax;
        y = ay;
        width = aw;
        height = ah;
    }

    public void empty()
    {
        x = 0;
        y = 0;
        width = 0;
        height = 0;
    }

    public static string ruby_helper()
    {
        return @"
            class Rect
              def to_s
                ""(#{x}, #{y}, #{width}, #{height})""
              end
              def _dump limit
                [x, y, width, height].pack(""iiii"")
              end
              def self._load str
                new(*str.unpack(""iiii""))
              end
            end
        ";
    }
}


