using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Color
{
    public int red = 0;
    public int green = 0;
    public int blue = 0;
    public int alpha = 255;

    public Color()
    {
        red = 0; green = 0; blue = 0; alpha = 255;
    }

    public Color(int r, int g, int b)
    {
        red = r; green = g; blue = b; alpha = 255;
    }

    public Color(int r, int g, int b, int a)
    {
        red = r; green = g; blue = b; alpha = a;
    }

    public void set(Color c)
    {
        if (c == null) return;
        red = c.red;
        green = c.green;
        blue = c.blue;
        alpha = c.alpha;
    }

    public void set(int r, int g, int b)
    {
        red = r; green = g; blue = b; alpha = 255;
    }

    public void set(int r, int g, int b, int a)
    {
        red = r; green = g; blue = b; alpha = a;
    }

    public static string ruby_helper()
    {
        return @"
            class Color
	            def _dump(d = 0)
		            [@red, @green, @blue, @alpha].pack('d4')
	            end
	            def self._load(s)
		            Color.new(*s.unpack('d4'))
	            end
            end
        ";
    }
}