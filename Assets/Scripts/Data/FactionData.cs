using UnityEngine;


[CreateAssetMenu(fileName ="New Faction", menuName ="Data/Faction")]
public class FactionData : ScriptableObject
{
    public string displayName;
    [Multiline]
    public string description;
    public SpaceResourceStorage initialResources;
    public BuildingData initialBuilding;
    public ResearchTreeData[] researchTrees;
}