using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridChunk : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Texture2D texture;
    bool needUpdate;
    int numTiles;
    int resolution;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void CreateTexture(int numTiles, int resolution)
    {
        this.numTiles = numTiles;
        this.resolution = resolution;
        texture = new Texture2D(numTiles * resolution, numTiles * resolution);
        texture.filterMode = FilterMode.Point;
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Sprite s = Sprite.Create(texture, rect, Vector2.zero, texture.width);
        spriteRenderer.sprite = s;
    }

    public void SetTile(int x, int y, Color color)
    {
        Color[] colors = new Color[resolution * resolution];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }
        texture.SetPixels(x * resolution, y * resolution, resolution, resolution, colors);
        needUpdate = true;
    }

    public void SetTile(int x, int y, Texture2D t)
    {
        Graphics.CopyTexture(t, 0, 0, 0, 0, t.width, t.height,
            texture, 0, 0, x * resolution, y * resolution);
        needUpdate = true;
    }

    private void LateUpdate()
    {
        if (needUpdate) {
            texture.Apply();
            needUpdate = false;
        }
    }
}
