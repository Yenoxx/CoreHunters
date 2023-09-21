using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SubmenuBuilding : Submenu
{
    public Building building 
    { 
        get => building_; 
        set
        {
            if (building != value) 
            {
                buildingChanged = true;
            }
            building_ = value;
        }
    }
    private Building building_;
    private bool buildingChanged;
    private SectorCamera.Snapshot cameraSnapshot;

    private Button buttonMove;
    private Button buttonCopy;
    private Button buttonDestroy;

    private Label labelName;

    private CategoryButton buttonCategoryProduction;
    private CategoryButton buttonCategoryInfo;

    private VisualElement viewProduction;
    private DropdownField dropdownTechnology;
    private VisualElement productonContent;
    private ViewResources viewResourcesProduction;

    private VisualElement viewInfo;
    private Label labelInfoText;

    private CategoryButton.Group categoryGroup;
    private FlexSwitcher flexSwitcher;

    public SubmenuBuilding(VisualElement root)
    {
        buildingChanged = true;

        wrapper = root.Q<VisualElement>("SubmenuBuilding");

        buttonMove = wrapper.Q<Button>("ButtonMove");
        buttonCopy = wrapper.Q<Button>("ButtonCopy");
        buttonDestroy = wrapper.Q<Button>("ButtonDestroy");

        labelName = wrapper.Q<Label>("LabelName");

        buttonCategoryProduction = wrapper.Q<CategoryButton>("ButtonCategoryProduction");
        buttonCategoryInfo = wrapper.Q<CategoryButton>("ButtonCategoryInfo");

        viewProduction = wrapper.Q<VisualElement>("ViewProduction");
        dropdownTechnology = wrapper.Q<DropdownField>("DropdownTechnology");
        productonContent = wrapper.Q<VisualElement>("ProductionContent");

        viewInfo = wrapper.Q<VisualElement>("ViewInfo");
        labelInfoText = wrapper.Q<Label>("LabelInfoText");

        categoryGroup = new CategoryButton.Group();
        categoryGroup.Add(buttonCategoryProduction);
        categoryGroup.Add(buttonCategoryInfo);
        buttonCategoryProduction.Click();

        flexSwitcher = new FlexSwitcher(viewProduction);
        flexSwitcher.Add(viewProduction);
        flexSwitcher.Add(viewInfo);
        flexSwitcher.Switch(viewProduction);
        UpdateContent();
    }

    public override void RegisterCallbacks()
    {
        buttonCopy.clicked += () =>
        {
            if (building != null)
            {
                ProviderUmpaLumpa.sectorManager.InitiateBuild(building.buildingData);
            }
        };

        Action categoryButtonClicked = () =>
        {
            UpdateContent();
        };
        buttonCategoryProduction.clicked += categoryButtonClicked;
        buttonCategoryInfo.clicked += categoryButtonClicked;

        EventCallback<ChangeEvent<string>> dropdownTechnologyCallback = (ChangeEvent<string> e) =>
        {
            if (building != null)
            {
                building.SetCurrentProductionVariant(e.newValue);
                if (viewResourcesProduction != null)
                {
                    building.currentProductionVariant.UpdateTotal();
                    viewResourcesProduction.storage = building.currentProductionVariant.total;
                }
            }
        };
        dropdownTechnology.RegisterValueChangedCallback(dropdownTechnologyCallback);
    }
    
    public override void OnShow()
    {
        if (building != null)
        {
            Vector2 position = new Vector2(building.transform.position.x, building.transform.position.y);
            cameraSnapshot = ProviderUmpaLumpa.sectorCamera.CreateSnapshot();
            ProviderUmpaLumpa.sectorCamera.FocusOn(position + building.centerOffset, new Vector2(0, -0.2f));

            building.blinking = true;
            labelName.text = building.buildingData.displayName;

            if (viewResourcesProduction == null)
            {
                building.currentProductionVariant.UpdateTotal();
                viewResourcesProduction = new ViewResources(building.currentProductionVariant.total);
                productonContent.Add(viewResourcesProduction);
            }
            if (buildingChanged)
            {
                building.currentProductionVariant.UpdateTotal();
                viewResourcesProduction.storage = building.currentProductionVariant.total;

                dropdownTechnology.choices.Clear();
                foreach (String choice in building.GetProductionVariantNames())
                {
                    dropdownTechnology.choices.Add(choice);
                }
                dropdownTechnology.value = building.currentProductionVariant.displayName;
                
            }
            labelInfoText.text = building.buildingData.description;
        }
    }

    public override void OnHide()
    {
        if (building != null)
        {
            building.blinking = false;
            ProviderUmpaLumpa.sectorCamera.LoadSnapshotZoom(cameraSnapshot);
        }
    }

    public void UpdateContent()
    {
        if (categoryGroup.currentCategoryName == CategoryButton.CATEGORY_ALL)
        {
            flexSwitcher.ShowAll();
        }
        else if (categoryGroup.currentCategoryName == buttonCategoryProduction.categoryName)
        {
            flexSwitcher.Switch(viewProduction);
        }
        else if (categoryGroup.currentCategoryName == buttonCategoryInfo.categoryName)
        {
            flexSwitcher.Switch(viewInfo);
        }
    }
}
