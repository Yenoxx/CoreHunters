using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LoaderUmpaLumpa
{
    public static Sprite LoadSprite(string path) 
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        if (sprites.Length <= 0) return default;
        return sprites[0];
    }
}
