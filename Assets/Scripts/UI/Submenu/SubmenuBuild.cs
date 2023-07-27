using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SubmenuBuild : Submenu
{
    private Button buttonClose;

    private CategoryButton buttonCategoryColonists;
    private CategoryButton buttonCategoryProduction;
    private CategoryButton buttonCategoryOther;
    private CategoryButton buttonCategoryDecoration;
    private CategoryButton.Group buildGroup;

    private VisualElement scrollContent;

    private Button buttonClear;
    private Button buttonSearch;
    private TextField textFieldSearch;

    private List<ButtonBuildingElement> buttonBuildingElements;
    

    public SubmenuBuild(VisualElement root)
    {
        wrapper = root.Q<VisualElement>("SubmenuBuild");

        buttonClose = wrapper.Q<Button>("ButtonClose");

        buttonCategoryColonists = wrapper.Q<CategoryButton>("ButtonCategoryColonists");
        buttonCategoryProduction = wrapper.Q<CategoryButton>("ButtonCategoryProduction");
        buttonCategoryOther = wrapper.Q<CategoryButton>("ButtonCategoryOther");
        buttonCategoryDecoration = wrapper.Q<CategoryButton>("ButtonCategoryDecoration");

        buildGroup = new CategoryButton.Group();
        buildGroup.Add(buttonCategoryColonists);
        buildGroup.Add(buttonCategoryProduction);
        buildGroup.Add(buttonCategoryOther);
        buildGroup.Add(buttonCategoryDecoration);

        scrollContent = wrapper.Q<VisualElement>("ScrollContent");

        buttonClear = wrapper.Q<Button>("ButtonClear");
        buttonSearch = wrapper.Q<Button>("ButtonSearch");
        textFieldSearch = wrapper.Q<TextField>("TextFieldSearch");

        buttonBuildingElements = new List<ButtonBuildingElement>();
    }

    public override void RegisterCallbacks()
    {
        buttonClose.clicked += () =>
        {
            ProviderUmpaLumpa.sectorMenu.Return();
        };

        Action categoryAction = () => 
        {
            ApplyFilter();
        };
        buttonCategoryColonists.clicked += categoryAction;
        buttonCategoryProduction.clicked += categoryAction;
        buttonCategoryOther.clicked += categoryAction;
        buttonCategoryDecoration.clicked += categoryAction;

        buttonClear.clicked += () =>
        {
            textFieldSearch.value = "";
            ApplyFilter();
        };
        buttonSearch.clicked += () =>
        {
            ApplyFilter();
        };

        EventCallback<KeyDownEvent> tfSearchKeydownCallback = (KeyDownEvent ev) =>
        {
            if (ev.keyCode == KeyCode.Return) ApplyFilter();
        };
        textFieldSearch.RegisterCallback(tfSearchKeydownCallback, TrickleDown.TrickleDown);
    }

    public void UpdateBBEs()
    {
        buttonBuildingElements.Clear();
        foreach (BuildingData buildingData in SaverUmpaLumpa.save.unlockedBuildings)
        {
            buttonBuildingElements.Add(new ButtonBuildingElement(buildingData));
        }
        // TODO: Sort
        
        while (scrollContent.childCount > 0) scrollContent.RemoveAt(0);
        foreach (ButtonBuildingElement buttonBuildingElement in buttonBuildingElements)
        {
            scrollContent.Add(buttonBuildingElement);
        }
    }

    public void ApplyFilter()
    {
        foreach (ButtonBuildingElement bbe in buttonBuildingElements)
        {
            if 
            (
                (
                    buildGroup.currentCategoryName.ToLower().Normalize() == bbe.buildingData.category.ToLower().Normalize() 
                    || 
                    buildGroup.currentCategoryName == CategoryButton.CATEGORY_ALL
                )
                &&
                HelperUmpaLumpa.LevenshteinDistance.MatchWords(
                    bbe.buildingData.displayName.ToLower().Normalize(), 
                    textFieldSearch.value.ToLower().Normalize()) >= 0.5f
            )
            {
                bbe.style.display = DisplayStyle.Flex;
            }
            else
            {
                bbe.style.display = DisplayStyle.None;
            }
        }
    }
}
