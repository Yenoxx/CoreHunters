using System;
using UnityEngine;


[CreateAssetMenu(fileName ="New ResearchTree", menuName ="Data/ResearchTree")]
public class ResearchTreeData : ScriptableObject
{
    public string researchGroup;
    public ResearchEntry[] researchEntries;
}


[Serializable]
public class ResearchEntry
{
    public bool initial;
    public string researchName;
    public string displayName;
    [Multiline]
    public string description;
    public SpaceResourceStorage cost;
    public string[] requredResearch;
    public BuildingData[] unlockedBuildings;
    public TechnologyData[] unlockedTechnologies;
    public string[] unlockedResearchGroups;
    public SpaceResourceStorage givenResources;
}
