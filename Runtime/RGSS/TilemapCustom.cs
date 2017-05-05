using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TilemapCustom
{
    //xp vars
    public Viewport viewport;
    public Table map_data;
    public Table flash_data;
    public bool visible = true;
    public float ox = 0f;
    public float oy = 0f;
    public bool disposed = false;
    public Bitmap tileset;
    public Bitmap[] autotiles = new Bitmap[7];
    public Table priorities;

    //custom vars
    List<Sprite> map_tiles = null;

    public bool map_tiles_empty()
    {
        if (map_tiles == null) map_tiles = new List<Sprite>();
        return (map_tiles.Count == 0);
    }

    public void add_map_tile(Sprite s)
    {
        if (map_tiles == null) map_tiles = new List<Sprite>();
        map_tiles.Add(s);
    }

    public void dispose_map_tiles()
    {
        if (map_tiles == null) map_tiles = new List<Sprite>();
        foreach (Sprite s in map_tiles)
        {
            s.bitmap.dispose();
            s.dispose();
        }
        map_tiles.Clear();
    }


    public void generate_tiles(Table map_data, Bitmap tileset, Viewport viewport)
    {
        if(viewport == null)
        {
            Console.WriteLine("Tilemap viewport is invalid!");
            return;
        }
        Console.Write("Generating tilemap...");
        //if not ready or needed, gtfo
        if (map_data == null || tileset == null)
        {
            if (map_data == null) Console.WriteLine("Attempted to generate tilemap with null map_data");
            if (tileset == null) Console.WriteLine("Attempted to generate tilemap with null tileset");
            return;
        }
        if(map_data.xsize == 0 || map_data.ysize == 0 || map_data.zsize == 0)
        {
            Console.WriteLine("Attempted to generate tilemap of size 0");
        }
        if (!map_tiles_empty())
        {
            Console.WriteLine("Attempted to generate a tilemap that is already generated");
            return;
        }

        //get map dimensions in 64x tiles
        int tile_width = Math.Max(1, (int)((float)map_data.xsize / 64f));
        int tile_height = Math.Max(1, (int)((float)map_data.ysize / 64f));
        //set blank color
        Color c_blank = new Color(0, 0, 0, 0);
        //iterate through the 64x64 map tiles we intend to create
        for(int tile_x = 0; tile_x < tile_width; tile_x++){
            for(int tile_y = 0; tile_y < tile_height; tile_y++){
                //determine the map starting tile coordinates
                int start_x = tile_x * 64;
                int start_y = tile_y * 64;
                //calculate the map tile dimensions - normally 64x64 except close to bottom or right edges
                int bm_width = 64;
                int bm_height = 64;
                if (tile_x == tile_width - 1) bm_width = (map_data.xsize - start_x);
                if (tile_y == tile_height - 1) bm_height = (map_data.ysize - start_y);
                //create a bitmap for this map tile
                Bitmap bitmap = new Bitmap(bm_width * 32, bm_height * 32);
                //determine map tile ending coordinates
                int end_x = start_x + bm_width;
                int end_y = start_y + bm_height;
                //initialize tile image coordinates
                int tile_image_x = 0;
                int tile_image_y = 0;

                //open the bitmap for direct access
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap.bmp);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                //iterate the map data and fill in the tile bitmap as needed
                for (int x = start_x; x < end_x; x++){
                    for(int y = start_y; y < end_y; y++){
                        //get the tile id
                        int tile_id = map_data.get(x, y, 0) - 384;
                        if(tile_id < 0) //if tile is empty
                        {
                            //fill in a blank, update the image coordinates, and continue
                            bitmap.fill_rect(tile_image_x, tile_image_y, 32, 32, c_blank);
                            tile_image_y += 32;
                            continue;
                        }

                        //direct copy bitmap info (blt is too slow)
                        System.Drawing.Rectangle source = new System.Drawing.Rectangle(new System.Drawing.Point((tile_id % 8) * 32, (tile_id / 8) * 32), new System.Drawing.Size(32, 32));
                        System.Drawing.Rectangle destination = new System.Drawing.Rectangle(new System.Drawing.Point(tile_image_x, tile_image_y), new System.Drawing.Size(32, 32));
                        g.DrawImage(tileset.bmp, destination, source, System.Drawing.GraphicsUnit.Pixel);

                        //update tile coordinates and continue
                        tile_image_y += 32;
                    }
                    //recycle column
                    tile_image_x += 32;
                    tile_image_y = 0;
                }

                //close direct access to bitmap
                g.Dispose();
                OG_Graphics.deferred_action_add(bitmap.syncBitmap);

                //create the tile sprite and add it to the map
                Sprite sprite = new Sprite(viewport);
                sprite.bitmap = bitmap;
                sprite.x = tile_x * 2048;
                sprite.y = tile_y * 2048;
                add_map_tile(sprite);
            }
        }
        Console.Write("   done.\n");
    }

}

/*

*/
