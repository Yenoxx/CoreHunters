using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName ="New TileCollection", menuName ="Data/TileCollection")]
public class TileCollectionData : ScriptableObject
{
    public TileBase[] liquidTiles;
    public TileBase[] softTiles;
    public TileBase[] solidTiles;
}