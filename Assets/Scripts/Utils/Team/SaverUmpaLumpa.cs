using System.Collections.Generic;
using UnityEngine;


public static class SaverUmpaLumpa
{
    public static Save save { get; private set; }

    public static void CreateSave(FactionData faction, PlanetData planet, Vector2Int mapSize)
    {
        save = new Save(faction, planet, mapSize);
    }

    public static void GenerateMap()
    {
        MapGenerator.Generate(save.map, save.planet.mapGenerationSettings);
    }
}
