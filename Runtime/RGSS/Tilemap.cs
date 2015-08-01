using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// a struct for rects
public struct UVRECT {
    public float x1;
    public float x2;
    public float y1;
    public float y2;
}

public class CTilemap : OpenGame.Runtime.Drawable
{
    private int texture = 0;
    private int fbo = 0;
    private int width, height;
    public int animationFrame = 0;
    public int animationCounter = 0;
    private int boundTexture = 0;
    public int ox = 0;
    public int oy = 0;
    private int mapWidthInTiles = 0;
    private int mapHeightInTiles = 0;
    private Viewport vp = null;
    public Viewport viewport
    {
        get { return vp; }
        set
        {
            if (vp != null) vp.sprites.Remove(this);
            vp = value;
            vp.sprites.Add(this);
        }
    }
    public short[] mapData;
    public bool[] layerFlag;

    int tilesetA1, tilesetA1_w, tilesetA1_h;
    int tilesetA2, tilesetA2_w, tilesetA2_h;
    int tilesetA3, tilesetA3_w, tilesetA3_h;
    int tilesetA4, tilesetA4_w, tilesetA4_h;
    int tilesetA5, tilesetA5_w, tilesetA5_h;
    int tilesetB, tilesetB_w, tilesetB_h;
    int tilesetC, tilesetC_w, tilesetC_h;
    int tilesetD, tilesetD_w, tilesetD_h;
    int tilesetE, tilesetE_w, tilesetE_h;

    int[,] A1Pos = new int[16, 2]{
      {0,0},
      {0,32*3},
      {32*6,0},
      {32*6,32*3},
      {32*8,0},
      {32*14,0},
      {32*8,32*3},
      {32*14,32*3},
      {0,32*6},
      {32*6,32*6},
      {0,32*9},
      {32*6,32*9},
      {32*8,32*6},
      {32*14,32*6},
      {32*8,32*9},
      {32*14,32*9},
    };

    int[,] A1 = new int[48, 4]{
        {18,17,14,13}, {2,17,14,13},  {18,3,14,13},  {2,3,14,13},
        {18,17,14,7},  {2,17,14,7},   {18,3,14,7},   {2,3,14,7},
        {18,17,6,13},  {2,17,6,13},   {18,3,6,13},   {2,3,6,13},
        {18,17,6,7},   {2,17,6,7},    {18,3,6,7},    {2,3,6,7},
        {16,17,12,13}, {16,3,12,13},  {16,17,12,7},  {16,3,12,7},
        {10,9,14,13},  {10,9,14,7},   {10,9,6,13},   {10,9,6,7},
        {18,19,14,15}, {18,19,6,15},  {2,19,14,15},  {2,19,6,15},
        {18,17,22,21}, {2,17,22,21},  {18,3,22,21},  {2,3,22,21},
        {16,19,12,15}, {10,9,22,21},  {8,9,12,13},   {8,9,12,7},
        {10,11,14,15}, {10,11,6,15},  {18,19,22,23}, {2,19,22,23},
        {16,17,20,21}, {16,3,20,21},  {8,11,12,15},  {8,9,20,21},
        {16,19,20,23}, {10,11,22,23}, {8,11,20,23},  {0,1,4,5}
    };

    int[,] A1E = new int[4, 4]{
        {2,1,6,5},{0,1,4,5},{2,3,6,7},{0,3,4,7}
    };

    int[,] A3 = new int[16, 4] {
        {5,6,9,10},    {4,5,8,9},    {1,2,5,6},   {0,1,4,5},
        {6,7,10,11},   {4,7,8,11},   {2,3,6,7},   {0,3,4,7},
        {9,10,13,14},  {8,9,12,13},  {1,2,13,14}, {0,1,12,13},
        {10,11,14,15}, {8,11,12,13}, {2,3,14,15}, {0,3,12,15}
    };

    public CTilemap()
    {
        initialize(Graphics.default_viewport);
    }
    public CTilemap(Viewport v)
    {
        initialize(v);
    }

    private void initialize(Viewport v)
    {
        created_at = DateTime.Now;
        viewport = v;
        z = 0;
        width = v.rect.width;
        height = v.rect.height;
        v.sprites.Add(this);
        rebuild();
    }

    public void dispose()
    {
        viewport.sprites.Remove(this);
    }

    private void rebuild()
    {
        GL.Enable(EnableCap.Texture2D);
        if (texture != 0)
        {
            GL.DeleteTextures(1, ref texture);
            texture = 0;
        }
        if (fbo != 0)
        {
            GL.DeleteFramebuffers(1, ref fbo);
            fbo = 0;
        }
        //generate texture
        GL.GenTextures(1, out texture);
        GL.BindTexture(TextureTarget.Texture2D, texture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
        //setup the fbo
        GL.GenFramebuffers(1, out fbo);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
        GL.BindTexture(TextureTarget.Texture2D, texture);
        GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, texture, 0);
        GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
    }
    private bool animationDirection = false;
    private int lastAnimationFrame = -1;
    private int lastOx = -1;
    private int lastOy = -1;
    public void update()
    {
        animationCounter += 1;
        if (animationCounter > 30)
        {
            animationCounter = 0;
            switch (animationFrame)
            {
                case 0:
                    animationFrame = 1;
                    break;
                case 1:
                    animationFrame = animationDirection ? 0 : 2;
                    animationDirection = !animationDirection;
                    break;
                case 2:
                    animationFrame = 1;
                    break;
            }
        }
        if (animationFrame == lastAnimationFrame && ox == lastOx && oy == lastOy) return;
        lastAnimationFrame = animationFrame;
        lastOx = ox;
        lastOy = oy;

        //Bind our special offscreen render buffer
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

        //clear the target texture
        GL.ClearColor(0f, 0f, 0f, 0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        //use the entire texture
        GL.Viewport(0, 0, width, height);

        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(0, width, height, 0, -1, 1);

        //offset render position based on origin
        float offsetx = ox;
        float offsety = 0 - oy - 16 + height;
        GL.Translate(offsetx, offsety, 0);

        //determine focal tile
        int startX = ox / 32;
        int startY = (0 - oy) / 32;

        //determine viewport dimensions in tiles
        int widthInTiles = width / 32;
        int heightInTiles = height / 32;

        //determine tiles visible on screen to be rendered (we'll ignore the rest)
        //the last operation is just adding padding
        int startTileX = startX;
        int endTileX = startX + widthInTiles + 1;
        int startTileY = 0 - startY;
        int endTileY = startTileY + heightInTiles + 1;

        //iterate the tilemap and render it
        for (int tilex = startTileX; tilex < endTileX; tilex++)
        { //just tiles in range
            for (int tiley = startTileY; tiley < endTileY; tiley++)
            { //just tiles in range
                //validate tile
                if ((tilex >= 0 && tilex < mapWidthInTiles) && (tiley >= 0 && tiley < mapWidthInTiles))
                {
                    //check if animated
                    bool animated = isAnimatedTile(get_map_data(tilex, tiley, 0));

                    short tiledata = get_map_data(tilex, tiley, 0);
                    //layer 1 tilesetA1
                    if (tiledata >= 2048 && tiledata <= 2815)
                    {
                        renderA1Tile(tilex, tiley, -0.5f, tiledata, animated);
                    }
                    //layer 1 tilesetA2
                    if (tiledata >= 2816 && tiledata <= 4351)
                    {
                        renderA2Tile(tilex, tiley, -0.5f, tiledata);
                    }
                    //layer 1 tilesetA3
                    if (tiledata >= 4352 && tiledata <= 5887)
                    {
                        renderA3Tile(tilex, tiley, -0.5f, tiledata);
                    }
                    //layer 1 tilesetA4
                    if (tiledata >= 5888 && tiledata <= 8191)
                    {
                        renderA4Tile(tilex, tiley, -0.5f, tiledata);
                    }
                    //layer 1 tilesetA5
                    if (tiledata >= 1536 && tiledata <= 1663)
                    {
                        renderA5Tile(tilex, tiley, -0.5f, tiledata);
                    }

                    tiledata = get_map_data(tilex, tiley, 1);
                    //layer 2 tilesetA1
                    if (tiledata >= 2048 && tiledata <= 2815)
                    {
                        renderA1Tile(tilex, tiley, -0.3f, tiledata, animated);
                    }
                    //layer 2 tilesetA2
                    if (tiledata >= 2816 && tiledata <= 4351)
                    {
                        renderA2Tile(tilex, tiley, -0.3f, tiledata);
                    }

                    //draw upper layers - there are 2 "upper" layers. we'll distinguish them by the z axis

                    UVRECT tileuv;
                    tiledata = get_map_data(tilex, tiley, 2);
                    int n = tiledata % 256;
                    tileuv.x1 = 32 * ((n % 8) + (8 * (n / 128)));
                    tileuv.y1 = 32 * ((n % 128) / 8);
                    tileuv.x2 = tileuv.x1 + 32;
                    tileuv.y2 = tileuv.y1 + 32;
                    //have to implement flags for the following line, per the comment below it
                    if (get_flag(tilex, tiley))
                    { //if @flags[@map_data[x,y,2]] & 0x10 == 0
                        renderTile(tilex * 32, tiley * 32, -0.2f, 5 + tiledata / 256, tileuv, true);
                    }
                    else
                    {
                        renderTile(tilex * 32, tiley * 32, -0.1f, 5 + tiledata / 256, tileuv, true);
                    }
                }
            }
        }
    }

    public override void draw(){
        //Console.WriteLine("Drawing tilemap at viewport " + viewport.z + " at z " + z);
        //draw like sprite
        GL.Enable(EnableCap.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, texture);

        GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);

        GL.Begin(BeginMode.Quads);


        int aw = width;
        int ah = height;
        GL.TexCoord2(0f, 0f);
        GL.Vertex3(0, 0, 0.5f);
        GL.TexCoord2(1f, 0f);
        GL.Vertex3(0 + aw, 0, 0.5f);
        GL.TexCoord2(1f, 1f);
        GL.Vertex3(0 + aw, 0 + ah, 0.5f);
        GL.TexCoord2(0f, 1f);
        GL.Vertex3(0, 0 + ah, 0.5f);

        GL.End();

    }

    public void set_dimensions(int aw, int ah){
        mapWidthInTiles = aw;
        mapHeightInTiles = ah;
        mapData = new short[((aw * ah)+1) * 3];
        layerFlag = new bool[(aw * ah)+1];
    }

    public short get_map_data(int x, int y, int layer)
    {
        //Console.WriteLine(String.Format("Accessing tile: {0} x {1}, {2}; Index {3}/{4}", x, y, layer, x * (3 * mapHeightInTiles) + y * 3 + layer, mapWidthInTiles * mapHeightInTiles * 3));
        return mapData[x * (3 * mapHeightInTiles) + y * 3 + layer];
    }

    public bool get_flag(int ax, int ay)
    {
        return layerFlag[(ax * mapHeightInTiles) + ay];
    }

    private bool isAnimatedTile(short tileId){
        return (tileId >= 2048 && tileId <= 2815);
    }

    void bindTexture(int newTexture){
        if(newTexture != boundTexture){
            GL.BindTexture(TextureTarget.Texture2D, newTexture);
            boundTexture = newTexture;
        }
    }

    int getBitmapForAutotile(int autotile){
        if(autotile >= 0 && autotile <= 15) return 0;
        if(autotile >= 16 && autotile <= 47) return 1;
        if(autotile >= 48 && autotile <= 79) return 2;
        if(autotile >= 80 && autotile <= 127) return 3;
        return 0;
    }

    UVRECT convertUV(UVRECT input, int tilesetId, int tilesetWidth, int tilesetHeight){
        UVRECT output;
        output.x1 = (1.0f / (float) tilesetWidth) * input.x1;
        output.x2 = (1.0f / (float) tilesetWidth) * input.x2;
        output.y1 = (1.0f / (float) tilesetHeight) * input.y2;
        output.y2 = (1.0f / (float) tilesetHeight) * input.y1;
        return output;
    }

    void renderTile(int x, int y, float z, int tilesetid, UVRECT uv_rect, bool whole){
        //tilesetid: 0 - A1, 1 - A2...8 - E

        //step 1: bind appropriate texture
        int tilesetWidth = 0;
        int tilesetHeight = 0;
        switch(tilesetid){
        case 0: //A1
            if (tilesetA1 == 0) return;
            bindTexture(tilesetA1);
            tilesetWidth = tilesetA1_w;
            tilesetHeight = tilesetA1_h;
            break;
        case 1: //A2
            if (tilesetA2 == 0) return;
            bindTexture(tilesetA2);
            tilesetWidth = tilesetA2_w;
            tilesetHeight = tilesetA2_h;
            break;
        case 2: //A3
            if (tilesetA3 == 0) return;
            bindTexture(tilesetA3);
            tilesetWidth = tilesetA3_w;
            tilesetHeight = tilesetA3_h;
            break;
        case 3: //A4
            if (tilesetA4 == 0) return;
            bindTexture(tilesetA4);
            tilesetWidth = tilesetA4_w;
            tilesetHeight = tilesetA4_h;
            break;
        case 4: //A5
            if (tilesetA5 == 0) return;
            bindTexture(tilesetA5);
            tilesetWidth = tilesetA5_w;
            tilesetHeight = tilesetA5_h;
            break;
        case 5: //B
            if (tilesetB == 0) return;
            bindTexture(tilesetB);
            tilesetWidth = tilesetB_w;
            tilesetHeight = tilesetB_h;
            break;
        case 6: //C
            if (tilesetC == 0) return;
            bindTexture(tilesetC);
            tilesetWidth = tilesetC_w;
            tilesetHeight = tilesetC_h;
            break;
        case 7: //D
            if (tilesetD == 0) return;
            bindTexture(tilesetD);
            tilesetWidth = tilesetD_w;
            tilesetHeight = tilesetD_h;
            break;
        case 8: //E
            if (tilesetE == 0) return;
            bindTexture(tilesetE);
            tilesetWidth = tilesetE_w;
            tilesetHeight = tilesetE_h;
            break;
        }
        int drawx = x;
        int drawy = 0 - y;
        if(whole) drawy -= 16;
        //grab dimensions of the original src_rect before we convert it to uv
        int draww = (int) (uv_rect.x2 - uv_rect.x1);
        int drawh = (int) (uv_rect.y2 - uv_rect.y1);

        //we'll set color on a per-draw basis for now in case we want to get crafty later
        GL.Color4(1f, 1f, 1f, 1f);

        //process the uv rect from pixels to uv
        UVRECT tileuv = convertUV(uv_rect, tilesetid, tilesetWidth, tilesetHeight);
        GL.Enable(EnableCap.Texture2D);
        GL.Begin(BeginMode.Polygon);
            GL.TexCoord2 (tileuv.x1, tileuv.y2);
            GL.Vertex3(drawx, drawy + drawh, z);       // bottom left
            GL.TexCoord2 (tileuv.x1, tileuv.y1);
            GL.Vertex3(drawx, drawy, z);       // top left
            GL.TexCoord2 (tileuv.x2, tileuv.y1);
            GL.Vertex3(drawx + draww, drawy, z);       // top right
            GL.TexCoord2 (tileuv.x2, tileuv.y2);
            GL.Vertex3(drawx + draww, drawy + drawh, z);       // bottom right
        GL.End();

    }

    void renderTile(int x, int y, float z, int tilesetid, UVRECT uv_rect){
        renderTile(x,y,z,tilesetid,uv_rect,false);
    }


    void renderWaterfallTile(float x, float y, float z, short tileId){
        int autotile = (tileId - 2048) / 48;
        int index = (tileId - 2048) % 48;
        int xa = A1Pos[autotile,0];
        int ya = A1Pos[autotile,1];
        UVRECT tileuv;
        ya += 32 * animationFrame;
        for(int i = 0; i <= 3; i++){
            tileuv.x1 = xa + (32 / 2) * (A1E[index,i]%4);
            tileuv.y1 = ya + (32 / 2) * (A1E[index,i]/4);
            tileuv.x2 = tileuv.x1 + 16;
            tileuv.y2 = tileuv.y1 + 16;
            switch(i){
            case 0:
                renderTile((int)x * 32, (int)y * 32, z, getBitmapForAutotile(autotile), tileuv);
                break;
            case 1:
                renderTile((int)x * 32 + (32 / 2), (int)y * 32, z, getBitmapForAutotile(autotile), tileuv);
                break;
            case 2:
                renderTile((int)x * 32, (int)y * 32 + (32/2), z, getBitmapForAutotile(autotile), tileuv);
                break;
            case 3:
                renderTile((int)x * 32 + (32/2), ((int)y * 32 + (32/2)), z, getBitmapForAutotile(autotile), tileuv);
                break;
            }
        }
    }

    void renderA1Tile(float x, float y, float z, short tileId, bool animated){
        int autotile = (tileId - 2048) / 48;
        int[] waterfallTiles = new int[6]{5,7,9,11,13,15};
        for(int i = 0; i < 6; i++){
            if(waterfallTiles[i] == autotile){
                renderWaterfallTile(x, y, z, tileId);
                return;
            }
        }
        int index = (tileId - 2048) % 48;
        int xa, ya;
        xa = 0; ya = 0;
        switch(getBitmapForAutotile(autotile)){
        case 0:
            xa = A1Pos[autotile,0];
            ya = A1Pos[autotile,1];
            break;
        case 1:
            xa = (32 * 2) * ((autotile - 16) % 8);
            ya = (32 * 3) * ((autotile - 16) / 8);
            break;
        case 2:
            xa = (32 * 2) * ((autotile - 48) % 8);
            ya = (32 * 2) * ((autotile - 48) / 8);
            break;
        case 3:
            xa = (32 * 2) * ((autotile - 80) % 8);
            ya = (32 * 3) * ((((autotile - 80) / 8)+1)/2) + (32 * 2) * (((autotile - 80) / 8) / 2);
            break;
        }
        UVRECT tileuv; //starts as pixel coordinates and will be converted via a utility function
        if(animated && (autotile != 2) && (autotile != 3)) xa += (32 * 2) * animationFrame;
        for(int i = 0; i <= 3; i++){
            tileuv.x1 = xa + (32 / 2) * (A1[index,i]%4);
            tileuv.y1 = ya + (32 / 2) * (A1[index,i]/4);
            tileuv.x2 = tileuv.x1 + 16;
            tileuv.y2 = tileuv.y1 + 16;
            switch(i){
            case 0:
                renderTile((int)x * 32, (int)y * 32, z, getBitmapForAutotile(autotile), tileuv);
                break;
            case 1:
                renderTile((int)x * 32 + (32 / 2), (int)y * 32, z, getBitmapForAutotile(autotile), tileuv);
                break;
            case 2:
                renderTile((int)x * 32, (int)y * 32 + (32 / 2), z, getBitmapForAutotile(autotile), tileuv);
                break;
            case 3:
                renderTile((int)x * 32 + (32 / 2), ((int)y * 32 + (32 / 2)), z, getBitmapForAutotile(autotile), tileuv);
                break;
            }
        }
    }

    void renderA2Tile(float x, float y, float z, short tileId){
        renderA1Tile(x,y,z,tileId, false);
    }


    void renderA3Tile(float x, float y, float z, short tileId){

        int autotile = (tileId - 2048) / 48;
        int index = (tileId - 2048) % 48;

        int xa, ya;
        xa = 0; ya = 0;
        switch(getBitmapForAutotile(autotile)){
        case 0:
            xa = (32 * 2) * ((autotile) % 8);
            ya = (32 * 3) * ((autotile) / 8);
            break;
        case 1:
            xa = (32 * 2) * ((autotile - 16) % 8);
            ya = (32 * 3) * ((autotile - 16) / 8);
            break;
        case 2:
            xa = (32 * 2) * ((autotile - 48) % 8);
            ya = (32 * 2) * ((autotile - 48) / 8);
            break;
        case 3:
            xa = (32 * 2) * ((autotile - 80) % 8);
            ya = (32 * 3) * ((((autotile - 80) / 8)+1)/2) + (32 * 2) * (((autotile - 80) / 8) / 2);
            break;
        }
        UVRECT tileuv; //starts as pixel coordinates and will be converted via a utility function
        for(int i = 0; i <= 3; i++){
            if (index > 15){
                tileuv.x1 = xa + (32 / 2) * (A1[index,i]%4);
                tileuv.y1 = ya + (32 / 2) * (A1[index,i]/4);
            } else {
                tileuv.x1 = xa + (32 / 2) * (A3[index,i]%4);
                tileuv.y1 = ya + (32 / 2) * (A3[index,i]/4);
            }

            tileuv.x2 = tileuv.x1 + 16;
            tileuv.y2 = tileuv.y1 + 16;
            switch(i){
            case 0:
                renderTile((int)x * 32, (int)y * 32, z, getBitmapForAutotile(autotile), tileuv);
                break;
            case 1:
                renderTile((int)x * 32 + (32 / 2), (int)y * 32, z, getBitmapForAutotile(autotile), tileuv);
                break;
            case 2:
                renderTile((int)x * 32, (int)y * 32 + (32 / 2), z, getBitmapForAutotile(autotile), tileuv);
                break;
            case 3:
                renderTile((int)x * 32 + (32 / 2), ((int)y * 32 + (32 / 2)), z, getBitmapForAutotile(autotile), tileuv);
                break;
            }
        }
    }

    void renderA4Tile(float x, float y, float z, short tileId){
        int autotile = (tileId - 2048) / 48;
        if(autotile >= 80 && autotile <= 87) {renderA1Tile(x,y,z,tileId, false); return; }
        if(autotile >= 96 && autotile <= 103) {renderA1Tile(x,y,z,tileId, false); return; }
        if (autotile >= 112 && autotile <= 119) { renderA1Tile(x, y, z, tileId, false); return; }
        renderA3Tile(x,y,z,tileId);
    }

    void renderA5Tile(float x, float y, float z, short tileId){
        tileId -= 1536;
        UVRECT tileuv; //starts as pixel coordinates and will be converted via a utility function
        tileuv.x1 = 32 * (tileId % 8);
        tileuv.y1 = 32 * ((tileId % 128)/8);
        tileuv.x2 = tileuv.x1 + 32;
        tileuv.y2 = tileuv.y1 + 32;
        renderTile((int)x * 32, (int)y * 32, z, 4, tileuv, true);
    }

    public void load_tileset(int index, string filename, bool autotile = false){
        bool clearflag = false;
        if (String.IsNullOrEmpty(filename)) clearflag = true;
        if(!clearflag){
            filename = FindTexture(filename, autotile);
            int gid = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, gid);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.ClampToEdge);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filename);
            int width_ = bmp.Width;
            int height_ = bmp.Height;
            System.Drawing.Imaging.BitmapData bmp_data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);
            bmp.Dispose();

            switch(index){
                case 0:
                    if (tilesetA1 != 0) { GL.DeleteTexture(tilesetA1); tilesetA1 = 0; }
                    tilesetA1 = gid; tilesetA1_w = width_; tilesetA1_h = height_;
                    break;
                case 1:
                    if (tilesetA2 != 0) { GL.DeleteTexture(tilesetA2); tilesetA2 = 0; }
                    tilesetA2 = gid; tilesetA2_w = width_; tilesetA2_h = height_;
                    break;
                case 2:
                    if (tilesetA3 != 0) { GL.DeleteTexture(tilesetA3); tilesetA3 = 0; }
                    tilesetA3 = gid; tilesetA3_w = width_; tilesetA3_h = height_;
                    break;
                case 3:
                    if (tilesetA4 != 0) { GL.DeleteTexture(tilesetA4); tilesetA4 = 0; }
                    tilesetA4 = gid; tilesetA4_w = width_; tilesetA4_h = height_;
                    break;
                case 4:
                    if (tilesetA5 != 0) { GL.DeleteTexture(tilesetA5); tilesetA5 = 0; }
                    tilesetA5 = gid; tilesetA5_w = width_; tilesetA5_h = height_;
                    break;
                case 5:
                    if (tilesetB != 0) { GL.DeleteTexture(tilesetB); tilesetB = 0; }
                    tilesetB = gid; tilesetB_w = width_; tilesetB_h = height_;
                    break;
                case 6:
                    if (tilesetC != 0) { GL.DeleteTexture(tilesetC); tilesetC = 0; }
                    tilesetC = gid; tilesetC_w = width_; tilesetC_h = height_;
                    break;
                case 7:
                    if (tilesetD != 0) { GL.DeleteTexture(tilesetD); tilesetD = 0; }
                    tilesetD = gid; tilesetD_w = width_; tilesetD_h = height_;
                    break;
                case 8:
                    if (tilesetE != 0) { GL.DeleteTexture(tilesetE); tilesetE = 0; }
                    tilesetE = gid; tilesetE_w = width_; tilesetE_h = height_;
                    break;
            }
        } else { //empty slot. remove the texture!
            switch(index){
                case 0: if (tilesetA1 != 0) { GL.DeleteTexture(tilesetA1); tilesetA1 = 0; } break;
                case 1: if (tilesetA2 != 0) { GL.DeleteTexture(tilesetA2); tilesetA2 = 0; } break;
                case 2: if (tilesetA3 != 0) { GL.DeleteTexture(tilesetA3); tilesetA3 = 0; } break;
                case 3: if (tilesetA4 != 0) { GL.DeleteTexture(tilesetA4); tilesetA4 = 0; } break;
                case 4: if (tilesetA5 != 0) { GL.DeleteTexture(tilesetA5); tilesetA5 = 0; } break;
                case 5: if (tilesetB != 0) { GL.DeleteTexture(tilesetB); tilesetB = 0; } break;
                case 6: if (tilesetC != 0) { GL.DeleteTexture(tilesetC); tilesetC = 0; } break;
                case 7: if (tilesetD != 0) { GL.DeleteTexture(tilesetD); tilesetD = 0; } break;
                case 8: if (tilesetE != 0) { GL.DeleteTexture(tilesetE); tilesetE = 0; } break;
            }
        }
    }

    private string FindTexture(string filename, bool autotile)
    {
        //Console.WriteLine("Loading tileset image: " + filename);
        string file = @"Graphics\Tilesets\" + filename;
        if (OpenGame.Runtime.Runtime.RGSSVersion == 2)
        {
            file = @"Graphics\System\" + filename;
        }
        if (OpenGame.Runtime.Runtime.RGSSVersion == 1 && autotile)
        {
            file = @"Graphics\Autotiles\" + filename;
        }
        file = OpenGame.Runtime.Runtime.FindImageResource(file);
        if (file == null)
        {
            Console.WriteLine("Failed to load texture: " + filename);
            return "";
        }
        return file;
    }

    public void set_tile(int x, int y, int layer, int tileid, int flag)
    {
        //Console.WriteLine(String.Format("Setting tile: {0} x {1}, layer {2}. Array Index/Size: {3}, {4}", x, y, layer, mapWidthInTiles * mapHeightInTiles, (x * mapWidthInTiles) + y));

        mapData[x * (3 * mapHeightInTiles) + y * 3 + layer] = (short) tileid;
        layerFlag[(x * mapHeightInTiles) + y] = (flag > 0);
    }

    public static string ruby_helper()
    {
        return @"

class Tilemap
  
  attr_reader   :map_data
  attr_accessor :bitmaps
  attr_accessor :flash_data
  attr_accessor :flags
  attr_accessor :viewport
  attr_accessor :visible
  attr_accessor :ox
  attr_accessor :oy
  attr_accessor :z
  if($RGSS_VERSION == 1)
    attr_accessor :tileset
    attr_accessor :autotiles
    attr_accessor :priorities
  end
  if($RGSS_VERSION == 2)
    attr_accessor :passages
  end
  def initialize(viewport = nil)
    @z = 0
    @ct = CTilemap.new(viewport) if (viewport)
    @ct = CTilemap.new if (!viewport)
    @bitmaps = []
    @disposed = false
    @viewport = viewport
    @visible = true
    @ox = 0
    @oy = 0
    @animated_layer = []
    @anim_count = 0
    @disposed = false
    if($RGSS_VERSION == 1)
        @tileset = nil
        @autotiles = []
        @priorities = []
    end
    if($RGSS_VERSION == 2)
        @passages = []
    end
  end

  def load_tileset
    if($RGSS_VERSION == 1)
      @ct.load_tileset(0, $game_map.tileset_name)
      @ct.load_tileset(1, $game_map.autotile_names[0], true) if($game_map.autotile_names[0])
      @ct.load_tileset(2, $game_map.autotile_names[1], true) if($game_map.autotile_names[1])
      @ct.load_tileset(3, $game_map.autotile_names[2], true) if($game_map.autotile_names[2])
      @ct.load_tileset(4, $game_map.autotile_names[3], true) if($game_map.autotile_names[3])
      @ct.load_tileset(5, $game_map.autotile_names[4], true) if($game_map.autotile_names[4])
      @ct.load_tileset(6, $game_map.autotile_names[5], true) if($game_map.autotile_names[5])
      @ct.load_tileset(7, $game_map.autotile_names[6], true) if($game_map.autotile_names[6])
    end
    if($RGSS_VERSION == 2)
        @ct.load_tileset(0, 'TileA1')
        @ct.load_tileset(1, 'TileA2')
        @ct.load_tileset(2, 'TileA3')
        @ct.load_tileset(3, 'TileA4')
        @ct.load_tileset(4, 'TileA5')
        @ct.load_tileset(5, 'TileB')
        @ct.load_tileset(6, 'TileC')
        @ct.load_tileset(7, 'TileD')
        @ct.load_tileset(8, 'TileE')
    end
    if($RGSS_VERSION == 3)
        $game_map.tileset.tileset_names.each_with_index do |name, i|
          @ct.load_tileset(i, name)
        end
    end
  end
    
  def dispose
    @ct.dispose
    @disposed = true
  end

  def disposed?
    return @disposed
  end
  
  def update
    @ct.ox = @ox
    @ct.oy = @oy
    @ct.z =  @z
    @ct.update
  end
  
  def refresh
    return if @map_data.nil? || @flags.nil?
    @ct.set_dimensions(@map_data.xsize, @map_data.ysize)
    for x in 0...@map_data.xsize
      for y in 0...@map_data.ysize
        for layer in 0..2
          tileid = @map_data[x,y,layer];
          flag = 0
          flag = 1 if @flags[@map_data[x,y,2]] & 0x10 == 0
          @ct.set_tile(x,y,layer, tileid, flag)
        end
      end
    end
  end
  
  def map_data=(data)
    return if @map_data == data
    @map_data = data
    load_tileset if $RGSS_VERSION == 1
    refresh
  end
  
  def flags=(data)
    @flags = data
    load_tileset
    refresh
  end

  if($RGSS_VERSION == 2)
    def passages=(data)
        @passages = data
        load_tileset
        refresh
    end
  end
end
        ";
    }
}