using System.Collections.Generic;
using UnityEngine.UIElements;

public class FlexSwitcher
{
    public VisualElement current { get; private set; }

    private List<VisualElement> visualElements;
    private VisualElement home;

    public FlexSwitcher(VisualElement home)
    {
        visualElements = new List<VisualElement>();
        visualElements.Add(home);
        this.home = home;
        Return();
    }
    public FlexSwitcher() : this(new VisualElement()) { }

    public void Add(VisualElement element)
    {
        visualElements.Add(element);
    }

    public void Switch(VisualElement element)
    {
        foreach (VisualElement otherElement in visualElements)
        {
            otherElement.style.display = DisplayStyle.None;
        }
        element.style.display = DisplayStyle.Flex;
        current = element;
    }

    public void Return()
    {
        Switch(home);
    }

    public void ShowAll()
    {
        foreach (VisualElement element in visualElements)
        {
            element.style.display = DisplayStyle.Flex;
        }
    }
}