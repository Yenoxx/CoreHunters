using System;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New SpaceResource", menuName ="Data/SpaceResource")]
public class SpaceResourceData : ScriptableObject
{
    public enum Type
    {
        NORMAL,
        STATIC,
        COLONIST,
    }
    
    public string displayName;
    public string shortName;
    [Multiline]
    public string description;
    public Type type = Type.NORMAL;
    public Sprite icon;
    public bool producing;
    public Production production;
}


[Serializable]
public class SpaceResourceStorage : SerializableDictionary<SpaceResourceData, int>
{
    private HashSet<SpaceResourceStorage> storageLinks;

    public SpaceResourceStorage() : base()
    {
        storageLinks = new HashSet<SpaceResourceStorage>();
    }

    public new void Clear()
    {
        base.Clear();
        storageLinks.Clear();
    }

    public bool CostIsAffordable(SpaceResourceStorage cost)
    {
        if (StorageLinked(cost))
        {
            foreach (SpaceResourceData key in cost.Keys) 
            {
                if (key.type != SpaceResourceData.Type.STATIC)
                {
                    if (ContainsKey(key))
                    {
                        if (this[key] + cost[key] < 0) return false;
                    }
                    else
                    {
                        if (cost[key] < 0) return false;
                    }
                }
            }
            return true;
        }
        else
        {
            foreach (SpaceResourceData key in cost.Keys) 
            {
                if (ContainsKey(key))
                {
                    if (this[key] + cost[key] < 0) return false;
                }
                else
                {
                    if (cost[key] < 0) return false;
                }
            }
            return true;
        }
    }

    public bool StorageLinked(SpaceResourceStorage storage)
    {
        return storageLinks.Contains(storage);
    }

    private void LinkStorage(SpaceResourceStorage storage)
    {
        if (!StorageLinked(storage))
        {
            storageLinks.Add(storage);
            foreach (SpaceResourceData key in storage.Keys) 
            {
                if (key.type == SpaceResourceData.Type.STATIC)
                {
                    if (!ContainsKey(key)) Add(key, 0);
                    this[key] += storage[key]; 
                }
            }
        }
    }

    private void UnlinkStorage(SpaceResourceStorage storage)
    {
        if (StorageLinked(storage))
        {
            storageLinks.Remove(storage);
            foreach (SpaceResourceData key in storage.Keys) 
            {
                if (key.type == SpaceResourceData.Type.STATIC)
                {
                    if (!ContainsKey(key)) Add(key, 0);
                    this[key] -= storage[key];
                }
            }
        }
    }
    
    public void AddStorage(SpaceResourceStorage storage)
    {
        LinkStorage(storage);
        foreach (SpaceResourceData key in storage.Keys) 
        {
            if (key.type != SpaceResourceData.Type.STATIC)
            {
                if (!ContainsKey(key)) Add(key, 0);
                this[key] += storage[key]; 
            }
        }
    }
}


[Serializable]
public class Production
{
    public int cycle;
    public SpaceResourceStorage cost = new SpaceResourceStorage();
    public SpaceResourceStorage products = new SpaceResourceStorage();
    [NonSerialized]
    public SpaceResourceStorage total = new SpaceResourceStorage();

    public void UpdateTotal()
    {
        total.Clear();
        total.AddStorage(cost);
        total.AddStorage(products);
    }
}


[Serializable]
public class ProductionVariant : Production
{
    public bool continious = true;
    public string displayName;
    public TechnologyData technology;
}