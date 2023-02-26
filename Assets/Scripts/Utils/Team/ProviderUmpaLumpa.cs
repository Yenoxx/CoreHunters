using UnityEngine;
using UnityEngine.EventSystems;

public static class ProviderUmpaLumpa
{
    public static EventSystem eventSystem { get; set; }
    public static SectorCamera sectorCamera { get; set; }
    public static SectorManager sectorManager { get; set; }
    public static SectorMenu sectorMenu { get; set; }
    public static SectorWorldUI sectorWorldUI { get; set; }
}
