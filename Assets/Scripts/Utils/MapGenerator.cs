public static class MapGenerator
{
    public static void Generate(Map map, MapGenerationSettingsData mapGenerationSettings)
    {
        switch (mapGenerationSettings.generationType)
        {
            case MapGenerationSettingsData.GenerationType.PLAINS:
            {
                GeneratePlains(map, mapGenerationSettings);
                break;
            }
        }
    }

    private static void GeneratePlains(Map map, MapGenerationSettingsData mapGenerationSettings)
    {
        // TODO: add soft tiles
        // TODO: add objects like rocks or trees
        for (int x = 0; x < map.size.x; x++)
        {
            for (int y = 0; y < map.size.y; y++)
            {
                map.SetTile(x, y, MapTile.Type.SOLID);
            }
        }
    }
}