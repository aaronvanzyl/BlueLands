using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChunk : MonoBehaviour
{
    public Tile[,] tiles;
    public Material material;
    int size;
    SpriteRenderer spriteRenderer;
    Texture2D mainTexture;
    Sprite mainSprite;
    Texture2D dataTexture;
    Sprite dataSprite;
    TileGraphics tileGraphics;
    bool needMainUpdate;
    bool needDataUpdate;
    bool inDataMode;
    //DataType dataType;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int size, TileGraphics tileGraphics)
    {
        this.tileGraphics = tileGraphics;
        this.size = size;
        tiles = new Tile[size, size];
        CreateTextures();
    }

    void CreateTextures()
    {
        mainTexture = new Texture2D(size * tileGraphics.tileResolution, size * tileGraphics.tileResolution);
        mainTexture.filterMode = FilterMode.Point;
        Rect rect = new Rect(0, 0, mainTexture.width, mainTexture.height);
        mainSprite = Sprite.Create(mainTexture, rect, Vector2.zero, mainTexture.width);
        spriteRenderer.sprite = mainSprite;

        dataTexture = new Texture2D(size, size);
        dataTexture.filterMode = FilterMode.Point;
        rect = new Rect(0, 0, dataTexture.width, dataTexture.height);
        dataSprite = Sprite.Create(dataTexture, rect, Vector2.zero, dataTexture.width);
    }

    public void SetTile(Tile tile, int x, int y)
    {
        tiles[x, y] = tile;
        UpdateTileTexture(x, y);
    }

    public void UpdateTileTexture(int x, int y)
    {
        //Debug.Log("Updating: " + x + "," + y);
        Texture t = tileGraphics.GetTexture(tiles[x, y]);
        //Graphics.CopyTexture(t, 0, 0, 0, 0, t.width, t.height, mainTexture, 0, 0, x * tileGraphics.tileResolution, y * tileGraphics.tileResolution);
        //texture.SetPixels(x * tileGraphics.tileResolution, y * tileGraphics.tileResolution, tileGraphics.tileResolution, tileGraphics.tileResolution, 
        //    texture.GetPixels(x * tileGraphics.tileResolution, y * tileGraphics.tileResolution, tileGraphics.tileResolution, tileGraphics.tileResolution));

        //needMainUpdate = true;
    }

    public void SetTextureMode(bool dataMode)
    {//, DataType type) {
        spriteRenderer.sprite = dataMode ? dataSprite : mainSprite;
        /*if (dataMode && (!inDataMode || type != dataType)) {
            UpdateAllDataTexture();
        }*/
        inDataMode = dataMode;
        //dataType = type;
    }

    /*void UpdateAllDataTexture() {
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                UpdateDataTexture(i, j);
            }
        }
    }*/

    public void SetDataTexture(int x, int y, Color color)
    {
        dataTexture.SetPixel(x, y, color);
        needDataUpdate = true;
    }

    /*public void UpdateDataTexture(int x, int y) {
        float h = 0, s = 0, v = 0;
        switch (dataType) {
            case DataType.Noise:
                v = Random.Range(0f, 1f);
                break;
            default:
                Debug.LogWarning("Unable to render data type: " + dataType.ToString());
                break;
        }
        dataTexture.SetPixel(x,y,Color.HSVToRGB(h,s,v));
        needDataUpdate = true;
    }*/



    private void LateUpdate()
    {
        if (needMainUpdate)
        {
            mainTexture.SetPixels(mainTexture.GetPixels());
            mainTexture.Apply();
            needMainUpdate = false;
        }
        if (needDataUpdate)
        {
            dataTexture.Apply();
            needDataUpdate = false;
        }
    }

    private void Update()
    {
        //Debug.Log(texture.loadedMipmapLevel);
    }
}
