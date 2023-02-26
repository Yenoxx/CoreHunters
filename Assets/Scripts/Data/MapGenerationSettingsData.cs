using UnityEngine;


[CreateAssetMenu(fileName ="New MapGenerationSettings", menuName ="Data/MapGenerationSettings")]
public class MapGenerationSettingsData : ScriptableObject
{
    public enum GenerationType
    {
        PLAINS,
        ISLAND,
        SURROUND,
        CRACKED,
    }

    public GenerationType generationType;
    [Range(0, 100)]
    public float softnessPercent;
    [Range(-1, 1)]
    public int surroundEW;
    [Range(-1, 1)]
    public int surroundNS;
}