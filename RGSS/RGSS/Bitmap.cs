using OpenTK.Graphics.OpenGL;
using RGSS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Bitmap
{
    public Font font;
    internal int txid = 0;
    private int width_;
    private int height_;
    internal System.Drawing.Bitmap bmp;
    private bool disposed = false;
    public string name = "";

    public Bitmap(string filename)
    {
        font = Font.default_font;
        LoadTexture(filename);
    }

    public Bitmap(int w, int h)
    {
        font = Font.default_font;
        CreateTexture(w, h);
    }

    public Bitmap(Bitmap b)
    {
        font = b.font;
        name = b.name;
        width_ = b.width();
        height_ = b.height();
        bmp = (System.Drawing.Bitmap) b.bmp.Clone();
        disposed = b.disposed;
        SyncBitmap();
    }

    private void CreateTexture(int w, int h)
    {
        w = Math.Abs(w);
        h = Math.Abs(h);
        if (w <= 0 || h <= 0)
        {
            w = 1;
            h = 1;
        }
        int gid = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, gid);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.ClampToEdge);
        //Console.WriteLine("Creating bitmap: " + w + "x" + h);
        bmp = new System.Drawing.Bitmap(w, h);
        //Console.WriteLine("Created.");
        width_ = bmp.Width;
        height_ = bmp.Height;
        BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

        bmp.UnlockBits(bmp_data);

        txid = gid;
    }

    private void LoadTexture(string filename)
    {
        if (String.IsNullOrEmpty(filename))
            throw new ArgumentException(filename);
        name = filename;
        filename = FindTexture(filename);
        int gid = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, gid);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.ClampToEdge);

        bmp = new System.Drawing.Bitmap(filename);
        width_ = bmp.Width;
        height_ = bmp.Height;
        BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

        bmp.UnlockBits(bmp_data);

        txid = gid;
    }

    private string FindTexture(string filename)
    {
        string file = filename + ".png";
        if (File.Exists(file))
        {
            return file;
        }
        else
        {
            file = filename + ".jpg";
            if (File.Exists(file))
            {
                return file;
            }
            else
            {
                file = Graphics.rtp_path + filename + ".png";
                if (File.Exists(file))
                {
                    return file;
                }
                else
                {
                    file = Graphics.rtp_path + filename + ".jpg";
                    if (File.Exists(file))
                    {
                        return file;
                    }
                    else {
                        Console.WriteLine("Failed to load texture: " + file);
                        return "";
                    }
                }
            }
        }
    }

    internal void SyncBitmap()
    {
        GL.Enable(EnableCap.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, txid);
        BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

        bmp.UnlockBits(bmp_data);
    }

    public void dispose()
    {
        if (disposed) return;
        disposed = true;
        if(bmp != null) bmp.Dispose();
        if (txid == 0) return;
        GL.DeleteTextures(1, ref txid);
        txid = 0;
    }

    public int width()
    {
        return width_;
    }

    public int height()
    {
        return height_;
    }

    public Rect rect()
    {
        return new Rect(0, 0, width_, height_);
    }

    public void blt(int x, int y, Bitmap src_bitmap, Rect src_rect)
    {
        stretch_blt(new Rect(x, y, src_rect.width, src_rect.height), src_bitmap, src_rect);
    }

    public void blt(int x, int y, Bitmap src_bitmap, Rect src_rect, int opacity)
    {
        stretch_blt(new Rect(x, y, src_rect.width, src_rect.height), src_bitmap, src_rect, opacity);
    }

    public void stretch_blt(Rect dest, Bitmap src_bitmap, Rect src_rect)
    {
        stretch_blt(dest, src_bitmap, src_rect, 255);
    }

    public Color get_pixel(int x, int y){
        System.Drawing.Color c = bmp.GetPixel(x, y);
        return new Color(c.R, c.G, c.B, c.A);
    }

    public void set_pixel(int x, int y, Color rc){
        System.Drawing.Color c = System.Drawing.Color.FromArgb(rc.alpha, rc.red, rc.green, rc.blue);
        bmp.SetPixel(x, y, c);
        SyncBitmap();
    }

    public void stretch_blt(Rect dest, Bitmap src_bitmap, Rect src_rect, int opacity)
    {
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
        g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver; //flip to sourceover to retain old image underneath. desired?
        System.Drawing.Rectangle source = new System.Drawing.Rectangle(new Point(src_rect.x, src_rect.y), new Size(src_rect.width, src_rect.height));
        System.Drawing.Rectangle destination = new System.Drawing.Rectangle(new Point(dest.x, dest.y), new Size(dest.width, dest.height));
        g.DrawImage(src_bitmap.bmp, destination, source, GraphicsUnit.Pixel);
        g.Dispose();
        SyncBitmap();
    }

    public void draw_text(int x, int y, int width, int height, int message)
    {
        draw_text(x, y, width, height, message.ToString());
    }
    public void draw_text(int x, int y, int width, int height, string message)
    {
        draw_text(x, y, width, height, message, 0);
    }

    public void draw_text(int x, int y, int width, int height, int message, int align)
    {
        draw_text(x, y, width, height, message.ToString(), align);
    }
    public void draw_text(int x, int y, int width, int height, string message, int align)
    {
        draw_text(new Rect(x, y, width, height), message, align);
    }

    public void draw_text(Rect r, int message)
    {
        draw_text(r, message.ToString());
    }
    public void draw_text(Rect r, string message)
    {
        draw_text(r, message, 0);
    }

    private void DrawStringShrink2Fit(System.Drawing.Graphics g, string value, System.Drawing.Font font, Brush brush, RectangleF rect, StringFormat format)
    {
        //sizes text vertically to fit
        SizeF size = g.MeasureString(value, font, (int)rect.Width);
        if ((size.Height <= rect.Height) || font.Size <= 1)
        {// fit?
            font = new System.Drawing.Font(font.FontFamily, font.Size + 3);
            g.DrawString(value, font, brush, rect, format); // then draw!
            font.Dispose();
        } 
        else
        {                                                           // otherwise shrink font
            float shrinkFactor = (float)Math.Sqrt(rect.Height / size.Height);
            using (System.Drawing.Font smallerFont = new System.Drawing.Font(font.FontFamily, (float)Math.Min(font.Size - 1, font.Size * shrinkFactor)))
                DrawStringShrink2Fit(g, value, smallerFont, brush, rect, format);      // and call function recursively
        }
    }

    public void draw_text(Rect r, int message, int align)
    {
        draw_text(r, message.ToString(), align);
    }
    public void draw_text(Rect r, string message, int align)
    {
        Brush brush = new SolidBrush(System.Drawing.Color.FromArgb((byte)font.color.alpha, (byte)font.color.red, (byte)font.color.green, (byte)font.color.blue));
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
        System.Drawing.Font f = font.get_drawing_font();
        System.Drawing.Rectangle destination = new System.Drawing.Rectangle(new Point(r.x, r.y), new Size(r.width, r.height));

        StringFormat stringFormat = new StringFormat();

        if (align == 0) stringFormat.Alignment = StringAlignment.Near;
        if (align == 1) stringFormat.Alignment = StringAlignment.Center;
        if (align == 2) stringFormat.Alignment = StringAlignment.Far;
        stringFormat.LineAlignment = StringAlignment.Center;

        //increases quality and accuracy
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

        if (font.outline)
        {
            Brush out_brush = new SolidBrush(System.Drawing.Color.FromArgb((byte)font.out_color.alpha, (byte)font.out_color.red, (byte)font.out_color.green, (byte)font.out_color.blue));
            destination.X -= font.outline_width;
            //g.DrawString(message, f, out_brush, destination, stringFormat);
            DrawStringShrink2Fit(g, message, f, out_brush, destination, stringFormat);
            destination.X += font.outline_width * 2;
            //g.DrawString(message, f, out_brush, destination, stringFormat);
            DrawStringShrink2Fit(g, message, f, out_brush, destination, stringFormat);
            destination.X -= font.outline_width;
            destination.Y -= font.outline_width;
            //g.DrawString(message, f, out_brush, destination, stringFormat);
            DrawStringShrink2Fit(g, message, f, out_brush, destination, stringFormat);
            destination.Y += font.outline_width * 2;
            //g.DrawString(message, f, out_brush, destination, stringFormat);
            DrawStringShrink2Fit(g, message, f, out_brush, destination, stringFormat);
            destination.Y -= font.outline_width;
            out_brush.Dispose();
        }

        //g.DrawString(message, f, brush, destination, stringFormat);
        DrawStringShrink2Fit(g, message, f, brush, destination, stringFormat);
        g.Dispose();
        brush.Dispose();
        SyncBitmap();
    }

    public Rect text_size(string message)
    {
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
        g.PageUnit = GraphicsUnit.Pixel;
        SizeF sf = g.MeasureString(message, font.get_drawing_font());
        g.Dispose();
        return new Rect(0, 0, (int)sf.Width, (int)sf.Height);
    }

    public void clear()
    {
        clear_rect(0,0,width_, height_);
    }

    public void clear_rect(Rect r) 
    {
        clear_rect(r.x, r.y, r.width, r.height);
    }

    public void clear_rect(int ax, int ay, int aw, int ah)
    {
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
        g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        using (var br = new SolidBrush(System.Drawing.Color.FromArgb(0, 255, 255, 255)))
        {
            g.FillRectangle(br, new Rectangle(ax, ay, aw, ah));
        }
        SyncBitmap();
        g.Dispose();
    }

    public void fill_rect(Rect r, Color c)
    {
        fill_rect(r.x, r.y, r.width, r.height, c);
    }

    public void fill_rect(int ax, int ay, int aw, int ah, Color c)
    {
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
        g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        using (var br = new SolidBrush(System.Drawing.Color.FromArgb(c.alpha, c.red, c.green, c.blue)))
        {
            g.FillRectangle(br, new Rectangle(ax, ay, aw, ah));
        }
        SyncBitmap();
        g.Dispose();
    }

    public void gradient_fill_rect(Rect r, Color c1, Color c2)
    {
        gradient_fill_rect(r, c1, c2, false);
    }

    public void gradient_fill_rect(Rect r, Color c1, Color c2, bool vertical)
    {
        gradient_fill_rect(r.x, r.y, r.width, r.height, c1, c2, vertical);
    }

    public void gradient_fill_rect(int ax, int ay, int aw, int ah, Color c1, Color c2)
    {
        gradient_fill_rect(ax, ay, aw, ah, c1, c2, false);
    }

    public void gradient_fill_rect(int ax, int ay, int aw, int ah, Color c1, Color c2, bool vertical)
    {
        if (aw == 0) aw = 100;
        if (ah == 0) ah = 100;
        if (aw < 0) aw = Math.Abs(aw);
        if (ah < 0) ah = Math.Abs(ah);
        //return;
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
        g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        System.Drawing.Drawing2D.LinearGradientBrush br;
        System.Drawing.Color syscolor1 = System.Drawing.Color.FromArgb(c1.alpha, c1.red, c1.green, c1.blue);
        System.Drawing.Color syscolor2 = System.Drawing.Color.FromArgb(c2.alpha, c2.red, c2.green, c2.blue);
        System.Drawing.Drawing2D.LinearGradientMode lgm;
        if (vertical)
        {
            lgm = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
        else
        {
            lgm = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
        }
        using (br = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(ax, ay, aw, ah), syscolor1, syscolor2, lgm))
        {
            g.FillRectangle(br, new Rectangle(ax, ay, aw, ah));
        }
        SyncBitmap();
        g.Dispose();
    }

    public Bitmap clone()
    {
        return new Bitmap(this);
    }

    public bool is_disposed()
    {
        return disposed;
    }

    public static string ruby_helper()
    {
        return @"
            class Bitmap
                def disposed?
                    return is_disposed
                end

                  def hue_change(hue)
                  end

                  def blur
                  end

                  def radial_blur(angle, division)
                  end
            end
        ";
    }
}