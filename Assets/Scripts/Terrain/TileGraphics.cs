using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGraphics : MonoBehaviour
{
    public int tileResolution;
    public Sprite[] sprites;
    Texture2D[] textures;

    private void Awake()
    {
        textures = new Texture2D[sprites.Length];
        for (int i = 0; i < sprites.Length; i++) {
            //Debug.Log("sprite to texture " + i);
            textures[i] = new Texture2D(tileResolution,tileResolution);
            Graphics.CopyTexture(sprites[i].texture,0,0,(int)sprites[i].rect.x,(int)sprites[i].rect.y,tileResolution,tileResolution,
                textures[i],0,0,0,0);
        }
    }

    public Texture2D GetTexture(Tile tile) {
        //Graphics.
        //Debug.Log(sprites[(int)tile.Type].rect);

        //return textures[(int)tile.biome];
        return textures[0];
    }


}
