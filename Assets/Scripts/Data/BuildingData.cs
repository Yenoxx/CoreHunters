using System;
using UnityEngine;


[CreateAssetMenu(fileName ="New Building", menuName ="Data/Building"), Icon("Assets/Resources/Sprites/Icons/IconBuild.png")]
public class BuildingData : ScriptableObject
{
    public string displayName;
    [Multiline]
    public string description;
    public string category;
    
    public Sprite sprite;
    public Sprite spriteFlip;
    public Vector2 spriteOffset;
    public Vector2Int cellSize;
    [Range(0.1f, 2f)]
    public float shadowScale;
    public Vector2 shadowOffset;

    public SpaceResourceStorage cost;
    public ProductionVariant[] productionVariants;
}