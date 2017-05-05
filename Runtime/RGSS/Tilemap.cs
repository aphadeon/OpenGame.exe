/*
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

    public Tilemap()
    {
        setup(null);
    }

    public Tilemap(Viewport v){
        setup(v);
    }

    public Tilemap initialize(Viewport v = null)
    {
        setup(v);
        return this;
    }

    internal void setup(Viewport v)
    {
        viewport = v;
        create_layers();
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

        public Tilemap parent;

        public TilemapLayer(Tilemap parent, int z)
        {
            this.parent = parent;
            this.z = z;
            created_at = DateTime.Now;
            if (parent.viewport != null)
            {
                parent.viewport.add_sprite(this);
            } else
            {
                OG_Graphics.drawables.Add(this);
            }
        }

        internal override void draw()
        {
            if (!parent.visible || parent.disposed) return;
        }

        public void dispose()
        {
            if(parent.viewport != null)
            {
                if (parent.viewport.sprites.Contains(this)) parent.viewport.sprites.Remove(this);
            } else
            {
                if (OG_Graphics.drawables.Contains(this)) OG_Graphics.drawables.Remove(this);
            }
        }
    }

}
*/