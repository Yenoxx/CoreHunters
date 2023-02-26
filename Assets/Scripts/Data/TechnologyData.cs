using UnityEngine;


[CreateAssetMenu(fileName ="New TechnologyData", menuName ="Data/TechnologyData")]
public class TechnologyData : ScriptableObject
{
    public string displayName;
    [Multiline]
    public string description;
}
