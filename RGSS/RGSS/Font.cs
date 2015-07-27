using RGSS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Font
{
    public static Font default_font;
    private static System.Drawing.Text.PrivateFontCollection private_fonts;

    public static string default_name;
    public static float default_size = 24;
    public static bool default_bold = false;
    public static bool default_italic = false;
    public static Color default_color = new Color(255, 255, 255, 255);
    public static Color default_out_color = new Color(0, 0, 0, 255);
    internal System.Drawing.Font internal_font;

    public static void load_fonts()
    {
        private_fonts = new System.Drawing.Text.PrivateFontCollection();

        List<string> list = new List<string>();
        if (Directory.Exists(@"Fonts\")) list.AddRange(Directory.GetFiles(@"Fonts\"));
        list.AddRange(Directory.GetFiles(Graphics.rtp_path + @"Fonts\"));
        string[] found = list.ToArray();
        int id = 0;
        foreach (string file in found)
        {
            if (Path.GetExtension(file) == ".ttf")
            {
                private_fonts.AddFontFile(file);
                //Console.WriteLine("Installed font: " + private_fonts.Families[id].Name);
                id++;
            }
        }
        default_name = "VL Gothic";
        if (Graphics.RgssVersion == 2)
        {
            default_name = "Verdana";
            default_size = 20;
        }
        default_font = new Font(default_name);
        //Console.WriteLine("Set default font to: " + default_font.name);
    }

    public string name;
    public float size;
    public bool bold = default_bold;
    public bool italic = default_italic;
    public Color color = default_color;
    public Color out_color = default_out_color;
    public bool shadow = false;
    public bool outline = true;
    public int outline_width = 1;

    public Font()
    {
        initialize(default_name, default_size);
    }

    public Font(string name){
        initialize(name, default_size);
    }

    public Font(string name, float size){
        initialize(name, size);
    }

    private void initialize(string aname, float asize)
    {
        name = aname;
        size = asize;
    }

    public bool exists(string aname){
        //test against local fonts
        foreach (System.Drawing.FontFamily ff in private_fonts.Families)
        {
            if (ff.GetName(0) == aname)
            {
                return true;
            }
        }
        //test against system installed fonts
        using (System.Drawing.Font fontTester = new System.Drawing.Font(
                aname, 12, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel))
        {
            if (fontTester.Name == aname)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    internal System.Drawing.Font get_drawing_font()
    {
        if (internal_font == null || check_font_dirty())
        {
            System.Drawing.FontFamily ff;
            try
            {
                ff = new System.Drawing.FontFamily(name, private_fonts);
            }
            catch (Exception e)
            {
                ff = new System.Drawing.FontFamily(name);
            }
            System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
            if (bold && ff.IsStyleAvailable(System.Drawing.FontStyle.Bold)) style = System.Drawing.FontStyle.Bold;
            if (italic && ff.IsStyleAvailable(System.Drawing.FontStyle.Italic)) style = System.Drawing.FontStyle.Italic;
            if (bold && italic && ff.IsStyleAvailable(System.Drawing.FontStyle.Bold) && ff.IsStyleAvailable(System.Drawing.FontStyle.Italic)) style = System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic;

            if (internal_font != null) internal_font.Dispose();
            internal_font = new System.Drawing.Font(ff, size - 10, style, System.Drawing.GraphicsUnit.Pixel);
        }
        return internal_font;
    }

    private bool check_font_dirty()
    {
        if (internal_font == null) return true;
        if (internal_font.FontFamily.GetName(0) != name) return true;
        if (internal_font.Size != size - 10) return true;
        return false;
    }

    public static string ruby_helper()
    {
        return @"
            class Font
                def exist?(arg_name)
                    return exists(arg_name)
                end
            end
        ";
    }
}