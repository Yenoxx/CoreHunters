using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Save
{
    public FactionData faction;
    public PlanetData planet;

    public SpaceResourceStorage storage;
    public HashSet<string> unlockedResearchGroups;
    public HashSet<string> unlockedResearch;
    public HashSet<BuildingData> unlockedBuildings;
    public HashSet<TechnologyData> unlockedTechnologies;

    public Map map;

    public Save(FactionData faction, PlanetData planet, Vector2Int mapSize)
    {
        this.faction = faction;
        this.planet = planet;

        storage = new SpaceResourceStorage();

        unlockedResearchGroups = new HashSet<string>();
        unlockedResearch = new HashSet<string>();
        unlockedBuildings = new HashSet<BuildingData>();
        unlockedTechnologies = new HashSet<TechnologyData>();

        map = new Map(mapSize);

        foreach (ResearchTreeData researchTree in faction.researchTrees)
        {
            foreach (ResearchEntry researchEntry in researchTree.researchEntries)
            {
                if (researchEntry.initial) UnlockResearchEntry(researchTree, researchEntry);
            }
        }
    }

    public bool ResearchAvailable(ResearchEntry researchEntry)
    {
        foreach (string item in researchEntry.requredResearch)
        {
            if (!unlockedResearch.Contains(item)) return false;
        }
        if (!storage.CostIsAffordable(researchEntry.cost)) return false;
        return true;
    }

    public void PayForResearch(ResearchEntry researchEntry)
    {
        storage.AddStorage(researchEntry.cost);
    }

    public void UnlockResearchEntry(ResearchTreeData researchTreeData, ResearchEntry researchEntry)
    {
        unlockedResearch.Add(researchTreeData.researchGroup + "_" + researchEntry.researchName);
        foreach (BuildingData item in researchEntry.unlockedBuildings)
        {
            unlockedBuildings.Add(item);
        }
        foreach (TechnologyData item in researchEntry.unlockedTechnologies)
        {
            unlockedTechnologies.Add(item);
        }
        foreach (string item in researchEntry.unlockedResearchGroups)
        {
            unlockedResearchGroups.Add(item);
        }
    }
}