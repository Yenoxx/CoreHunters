using System;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New SpaceResource", menuName ="Data/SpaceResource"), Icon("Assets/Resources/Sprites/Icons/IconProduction.png")]
public class SpaceResourceData : ScriptableObject
{
    public enum Type
    {
        NORMAL,
        STATIC,
        COLONIST,
    }
    
    public string displayName;
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

    public bool RequirementsMet(SpaceResourceStorage requirements)
    {
        foreach (SpaceResourceData key in requirements.Keys) 
        {
            if (ContainsKey(key))
            {
                if (this[key] - requirements[key] < 0) return false;
            }
            else
            {
                if (requirements[key] > 0) return false;
            }
        }
        return true;
    }

    public bool StaticStorageLinked(SpaceResourceStorage staticStorage)
    {
        return storageLinks.Contains(staticStorage);
    }

    private void LinkStorage(SpaceResourceStorage storage)
    {
        storageLinks.Add(storage);
    }

    private void UnlinkStorage(SpaceResourceStorage storage)
    {
        storageLinks.Remove(storage);
    }
    
    public void AddStorage(SpaceResourceStorage storage)
    {
        if (!StaticStorageLinked(storage))
        {
            LinkStorage(storage);
            foreach (SpaceResourceData key in storage.Keys) 
            {
                if (!ContainsKey(key)) Add(key, 0);
                this[key] += storage[key]; 
            }
        }
        else
        {
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

    public void RemoveStorage(SpaceResourceStorage storage)
    {
        if (StaticStorageLinked(storage))
        {
            UnlinkStorage(storage);
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
}


[Serializable]
public class Production
{
    public int cycle;
    public SpaceResourceStorage requirements = new SpaceResourceStorage();
    public SpaceResourceStorage products = new SpaceResourceStorage();
}


[Serializable]
public class ProductionVariant : Production
{
    public bool continious = true;
    public string displayName;
    public TechnologyData technology;
}