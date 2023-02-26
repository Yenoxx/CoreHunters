using System;
using UnityEngine;


[Serializable]
public class Map {
    public MapTile[,] tiles;

    public Vector2Int size { get; }

    public Map(int width, int height)
    {
        size = new Vector2Int(width, height);
        tiles = new MapTile[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                tiles[x, y] = new MapTile(MapTile.Type.CHASM);
            }
        }
    }
    public Map(Vector2Int size) : this(size.x, size.y) {}

    public bool HasPosition(int x, int y)
    {
        return 
            x >= 0 && x < size.x &&
            y >= 0 && y < size.y;
    }
    public bool HasPosition(Vector2Int pos)
    {
        return HasPosition(pos.x, pos.y);
    }

    #nullable enable

    public MapTile? GetTile(int x, int y)
    {
        if (!HasPosition(x, y)) return null;
        return tiles[x, y];
    }
    public MapTile? GetTile(Vector2Int pos)
    {
        return GetTile(pos.x, pos.y);
    }

    public MapTile.Type GetTileType(int x, int y)
    {
        MapTile? tile = GetTile(x, y);
        if (tile == null) return MapTile.Type.CHASM;
        return tile.modifiedType;
    }
    public MapTile.Type GetTileType(Vector2Int pos)
    {
        return GetTileType(pos.x, pos.y);
    }

    public bool PositionTypeOf(int x, int y, MapTile.Type tileType)
    {
        MapTile? tile = GetTile(x, y);
        if (tile == null) return false;
        return tile.modifiedType == tileType;
    }
    public bool PositionTypeOf(Vector2Int pos, MapTile.Type tileType)
    {
        return PositionTypeOf(pos.x, pos.y, tileType);
    }

    #nullable disable

    public bool SetTile(int x, int y, MapTile tile)
    {
        if (!HasPosition(x, y)) return false;
        tiles[x, y] = tile;
        return true;
    }
    public bool SetTile(int x, int y, MapTile.Type tileType)
    {
        return SetTile(x, y, new MapTile(tileType));
    }
    public bool SetTile(Vector2Int pos, MapTile tile)
    {
        return SetTile(pos.x, pos.y, tile);
    }
    public bool SetTile(Vector2Int pos, MapTile.Type tileType)
    {
        return SetTile(pos.x, pos.y, tileType);
    }
}


[Serializable]
public class MapTile {
    public enum Type
    {
        CHASM,
        LIQUID,
        SOFT,
        SOLID,
    }

    public Type type;
    public bool hasFloor;
    public bool hasBuilding;

    public Type modifiedType
    {
        get { return hasFloor ? Type.SOLID : type; }
    }

    public bool available
    {
        get
        {
            return modifiedType == Type.SOLID && !hasBuilding;
        }
    }

    public MapTile(Type type)
    {
        this.type = type;
        hasFloor = false;
        hasBuilding = false;
    }
    public MapTile() : this(Type.SOLID) {}
}