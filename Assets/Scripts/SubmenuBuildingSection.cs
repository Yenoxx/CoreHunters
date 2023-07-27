using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SubmenuBuildingSection
{
    public VisualElement wrapper { get; private set; }
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

    private Button buttonMove;
    private Button buttonCopy;
    private Button buttonDestroy;

    private Label labelName;

    private CategoryButton buttonCategoryProduction;
    private CategoryButton buttonCategoryInfo;

    private VisualElement viewProduction;
    private VisualElement productonContent;
    private ViewResources viewResourcesProduction;

    private VisualElement viewInfo;
    private Label labelInfoText;

    private CategoryButton.Group categoryGroup;
    private FlexSwitcher flexSwitcher;

    public SubmenuBuildingSection(VisualElement root)
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

    public void RegisterCallbacks()
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
    }
    
    public void OnShow()
    {
        if (building != null)
        {
            building.blinking = true;
            labelName.text = building.buildingData.displayName;

            if (viewResourcesProduction == null)
            {
                viewResourcesProduction = new ViewResources(building.currentProductionVariant.products);
                productonContent.Add(viewResourcesProduction);
            }
            if (buildingChanged)
            {
                viewResourcesProduction.storage = building.currentProductionVariant.products;
            }
            labelInfoText.text = building.buildingData.description;
        }
    }

    public void OnHide()
    {
        if (building != null)
        {
            building.blinking = false;
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
