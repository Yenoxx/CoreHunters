using UnityEngine;


[CreateAssetMenu(fileName ="New Planet", menuName ="Data/Planet")]
public class PlanetData : ScriptableObject
{
    public string displayName;
    [Multiline]
    public string description;
    public TileCollectionData tileCollection;
    public MapGenerationSettingsData mapGenerationSettings;
}
