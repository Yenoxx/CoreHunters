using UnityEngine;

public class BuildingSprite : MonoBehaviour
{
    public Building building;

    void OnMouseDown()
    {
        if (building != null)
        {
            if (!ProviderUmpaLumpa.eventSystem.IsPointerOverGameObject())
            {
                building.justPressed = true;
                building.pressed = true;
            }
        }
    }

    void OnMouseUp()
    {
        if (building != null)
        {
            if (!ProviderUmpaLumpa.eventSystem.IsPointerOverGameObject())
            {
                building.justReleased = true;
                building.pressed = false;
            }
        }
    }
}
