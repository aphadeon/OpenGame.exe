using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Tone
{
    public int red = 0;
    public int green = 0;
    public int blue = 0;
    public int gray = 255;

    public Tone()
    {
        red = 0; green = 0; blue = 0; gray = 255;
    }

    public Tone(int r, int g, int b)
    {
        red = r; green = g; blue = b; gray = 255;
    }

    public Tone(int r, int g, int b, int a)
    {
        red = r; green = g; blue = b; gray = a;
    }

    public void set(Tone c)
    {
        if (c == null) return;
        red = c.red;
        green = c.green;
        blue = c.blue;
        gray = c.gray;
    }

    public void set(int r, int g, int b)
    {
        red = r; green = g; blue = b; gray = 255;
    }

    public void set(int r, int g, int b, int a)
    {
        red = r; green = g; blue = b; gray = a;
    }

    public Color color()
    {
        return new Color(red, green, blue, gray);
    }

    internal static string ruby_helper()
    {
        return @"
            class Tone
	            def _dump(d = 0)
		            [@red, @green, @blue, @gray].pack('d4')
	            end
	            def self._load(s)
		            Tone.new(*s.unpack('d4'))
	            end
            end
        ";
    }
}