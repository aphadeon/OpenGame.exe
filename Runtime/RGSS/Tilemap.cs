using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Tilemap
{
    //rpg maker stuff
    public Viewport viewport; //all 3
    public Table map_data; //all 3
    public Table flash_data; //all 3
    public bool visible = true; //all 3
    public float ox = 0f; //all 3
    public float oy = 0f; //all 3
    public bool disposed = false; //all 3

    public Table passages; //only vx
    public Table flags; //only ace

    public Bitmap[] bitmaps = new Bitmap[9]; //vx and ace
    public Bitmap tileset; //only xp
    public Bitmap[] autotiles = new Bitmap[7]; //only xp
    public Table priorities; //only xp

    //our stuff
    private List<TilemapLayer> layers = new List<TilemapLayer>();

    public Tilemap(Viewport v = null){
        initialize(v);
    }

    public Tilemap initialize(Viewport v = null)
    {
        viewport = v == null ? Graphics.default_viewport : v;
        create_layers();
        return this;
    }

    private void create_layers()
    {
        if (OpenGame.Runtime.Runtime.RGSSVersion == 1)
        {
            //XP renders all tiles below the player unless they have priority.
            //Priority tiles are rendered as sprites whose depth changes in realtime.
            layers.Add(new TilemapLayer(this, 0)); //under character
        }
        else
        {
            layers.Add(new TilemapLayer(this, 0)); //under character
            layers.Add(new TilemapLayer(this, 200)); //over character
        }
    }

    public void update()
    {
        if (!visible || disposed) return;
    }

    public void dispose()
    {
        foreach (TilemapLayer layer in layers)
        {
            layer.dispose();
        }
        disposed = true;
    }

    public bool is_disposed()
    {
        return disposed;
    }
    public static string ruby_helper()
    {
        return @"
            class Tilemap
                def disposed?
                    return is_disposed
                end
            end
        ";
    }

    private class TilemapLayer : OpenGame.Runtime.Drawable {

        public DateTime created_at;
        public Tilemap parent;
        public int z = 0;

        public TilemapLayer(Tilemap parent, int z)
        {
            this.parent = parent;
            this.z = z;
            created_at = DateTime.Now;
            parent.viewport.sprites.Add(this);
        }

        public virtual void draw()
        {
            if (!parent.visible || parent.disposed) return;
        }

        public void dispose()
        {
            parent.viewport.sprites.Remove(this);
        }
    }

}
