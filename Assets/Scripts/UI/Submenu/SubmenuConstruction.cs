using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SubmenuConstruction : Submenu
{
    public SubmenuConstruction(VisualElement root)
    {
        wrapper = root.Q<VisualElement>("SubmenuConstruction");
    }
}
