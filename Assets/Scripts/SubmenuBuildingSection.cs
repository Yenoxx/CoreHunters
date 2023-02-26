using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SubmenuBuildingSection
{
    public VisualElement wrapper { get; private set; }
    public Building building { get; set; }

    private Button buttonMove;
    private Button buttonCopy;
    private Button buttonDestroy;

    private Label labelName;

    private CategoryButton buttonCategoryProduction;
    private CategoryButton buttonCategoryInfo;

    private CategoryButton.Group categoryGroup;

    public SubmenuBuildingSection(VisualElement root)
    {
        wrapper = root.Q<VisualElement>("SubmenuBuilding");

        buttonMove = wrapper.Q<Button>("ButtonMove");
        buttonCopy = wrapper.Q<Button>("ButtonCopy");
        buttonDestroy = wrapper.Q<Button>("ButtonDestroy");

        labelName = wrapper.Q<Label>("LabelName");

        buttonCategoryProduction = wrapper.Q<CategoryButton>("ButtonCategoryProduction");
        buttonCategoryInfo = wrapper.Q<CategoryButton>("ButtonCategoryInfo");

        categoryGroup = new CategoryButton.Group();
        categoryGroup.Add(buttonCategoryProduction);
        categoryGroup.Add(buttonCategoryInfo);
        buttonCategoryProduction.Click();
    }

    public void RegisterCallbacks()
    {

    }
    
    public void OnShow()
    {
        if (building != null)
        {
            building.blinking = true;
            labelName.text = building.buildingData.displayName;
        }
    }

    public void OnHide()
    {
        if (building != null)
        {
            building.blinking = false;
        }
    }
}
