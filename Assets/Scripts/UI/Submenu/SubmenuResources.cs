using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SubmenuResources : Submenu
{
    private IconButton buttonClose;

    private CategoryButton buttonCategoryColonists;

    private VisualElement scrollContent;
    private ViewResources viewResources;

    public SubmenuResources(VisualElement root)
    {
        wrapper = root.Q<VisualElement>("SubmenuBuild");

        
    }

    public override void RegisterCallbacks()
    {
        
    }

    public void ApplyFilter()
    {
        
    }
}
