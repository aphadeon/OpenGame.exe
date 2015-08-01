using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Table
{
    public int xsize = 0;
    public int ysize = 0;
    public int zsize = 0;
    public int[] data = new int[0];

    public Table(int width, int height = 1, int depth = 1)
    {
        initialize(width, height, depth);
    }

    public Table initialize(int width, int height = 1, int depth = 1)
    {
        setup(width, height, depth);
        return this;
    }

    public void setup(int w, int h, int d)
    {
        xsize = w;
        ysize = h;
        zsize = d;
        data = new int[xsize * ysize * zsize];
    }

    public void resize(int new_xsize, int new_ysize, int new_zsize)
    {
        //TODO
    }

    public int get(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0) return 0;
        if(x > xsize || y > ysize || z > zsize) return 0;
        return data[x + y * xsize + z * xsize * ysize];
    }

    public void set(int x, int y, int z, dynamic value)
    {
        data[x + y * xsize + z * xsize * ysize] = value;
    }

    public static string ruby_helper()
    {
        return @"
            class Table
                def [](x, y=0, z=0)
                    return get(x, y, z)
                end

                def []= (x, y, z, v)
                    set(x, y, z, v);
                end

                # Willing to lose some performance on load/save for better performance while in-use.
                # Not to mention C# access to the data contents.

                def self._load(s)
                    Table.new(1).instance_eval {
                        @size, w, h, d, xx, *xdata = s.unpack('LLLLLS*')
                        setup(w,h,d)
                        counter = 0
                        xdata.each { |x| 
                            data[counter] = x
                            counter += 1
                        }
                        self
                    }
                end

                def _dump(d = 0)
                    xdata = []
                    counter = 0
                    data.each { |x|
                        xdata[counter] = x
                        counter += 1
                    }
                    [@size, xsize, ysize, zsize, xsize * ysize * zsize, *xdata].pack('LLLLLS*')
                end
            end
        ";
    }
}
