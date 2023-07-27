using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;


public class ViewResources : VisualElement
{
    public const string TEMPLATE_PATH = "UI/Templates/ViewResources";
    public const string TEMPLATE_VIEW_RESOURCE_PATH = "UI/Templates/ViewResource";


    public SpaceResourceStorage storage
    {
        set
        {
            storage_ = value;
            UpdateAll();
        }
        get => storage_;
    }
    private SpaceResourceStorage storage_;

    private Dictionary<SpaceResourceData, VisualElement> resourceViews;
    private Dictionary<SpaceResourceData, Label> resourceQuantities;


    public ViewResources(SpaceResourceStorage storage) : base()
    {
        this.storage_ = storage;

        resourceViews = new Dictionary<SpaceResourceData, VisualElement>();
        resourceQuantities = new Dictionary<SpaceResourceData, Label>();

        VisualTreeAsset template = Resources.Load<VisualTreeAsset>(TEMPLATE_PATH);
        VisualElement templateElement = template.CloneTree().Q<VisualElement>("ViewResources");

        foreach (string ussClassName in templateElement.GetClasses())
        {
            AddToClassList(ussClassName);
        }

        style.flexGrow = 1;
        style.flexShrink = 1;

        style.flexDirection = FlexDirection.Column;

        style.paddingTop = 8;
        style.paddingLeft = 8;
        style.paddingRight = 8;
        style.paddingBottom = 0;

        style.borderTopLeftRadius = 2;
        style.borderTopRightRadius = 2;
        style.borderBottomLeftRadius = 2;
        style.borderBottomRightRadius = 2;

        UpdateAll();
    }

    public void UpdateAll()
    {
        Clear();
        resourceViews.Clear();
        resourceQuantities.Clear();

        VisualTreeAsset template = Resources.Load<VisualTreeAsset>(TEMPLATE_VIEW_RESOURCE_PATH);

        if (storage.Keys.Count > 0)
        {
            foreach (SpaceResourceData res in storage.Keys)
            {
                VisualElement viewResource = template.CloneTree().Q<VisualElement>("ViewResource");
                resourceViews.Add(res, viewResource);

                if (res.icon != null)
                {
                    VisualElement icon = viewResource.Q<VisualElement>("Icon");
                    icon.style.backgroundImage = new StyleBackground(res.icon);
                }
                
                Label labelName = viewResource.Q<Label>("LabelName");
                labelName.text = res.shortName;
                
                Label labelQuantity = viewResource.Q<Label>("LabelQuantity");
                resourceQuantities.Add(res, labelQuantity);
                labelQuantity.text = storage[res].ToString(); // TODO: format!!!

                Add(viewResource);
            }
        }
        else
        {
            VisualElement viewResource = template.CloneTree().Q<VisualElement>("ViewResource");
            
            Label labelName = viewResource.Q<Label>("LabelName");
            labelName.text = "[no production]";
            
            Label labelQuantity = viewResource.Q<Label>("LabelQuantity");
            labelQuantity.text = ""; // TODO: format!!!

            Add(viewResource);
        }
    }

    public void UpdateValues()
    {
        foreach (SpaceResourceData res in storage.Keys)
        {
            if (resourceQuantities.ContainsKey(res))
            {
                resourceQuantities[res].name = storage[res].ToString(); // TODO: format!!!
            }
        }
    }
}