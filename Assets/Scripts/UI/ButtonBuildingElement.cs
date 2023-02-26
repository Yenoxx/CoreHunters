using UnityEngine;
using UnityEngine.UIElements;


public class ButtonBuildingElement : Button
{
    public const string TEMPLATE_PATH = "UI/Templates/ButtonBuildingElement";

    public BuildingData buildingData { get; private set; }

    public ButtonBuildingElement(BuildingData buildingData) : base()
    {
        this.buildingData = buildingData;
        focusable = false;

        VisualTreeAsset template = Resources.Load<VisualTreeAsset>(TEMPLATE_PATH);
        VisualElement templateElement = template.CloneTree().Q<Button>("ButtonBuildingElement");

        foreach (string ussClassName in templateElement.GetClasses())
        {
            AddToClassList(ussClassName);
        }

        while (templateElement.childCount > 0)
        {
            VisualElement element = templateElement[0];
            templateElement.Remove(element);
            Add(element);
        }

        VisualElement icon = UQueryExtensions.Q<VisualElement>(this, "Icon");
        icon.style.backgroundImage = new StyleBackground(buildingData.sprite);

        Label labelName = UQueryExtensions.Q<Label>(this, "LabelName");
        labelName.text = buildingData.displayName;

        clicked += () =>
        {
            ProviderUmpaLumpa.sectorManager.InitiateBuild(buildingData);
        };
    }
}