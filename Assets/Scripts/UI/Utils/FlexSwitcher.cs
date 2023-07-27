using System.Collections.Generic;
using UnityEngine.UIElements;

public class FlexSwitcher
{
    public delegate void SwitchEvent(VisualElement element);


    public static VisualElement noElement 
    {
        get => noElement_;
    }
    public static VisualElement noElement_ = new VisualElement();

    public static VisualElement allElements
    {
        get => allElements_;
    }
    public static VisualElement allElements_ = new VisualElement();


    public VisualElement current { get; private set; }

    public SwitchEvent switchTo;
    public SwitchEvent switchFrom;

    private List<VisualElement> visualElements;
    private VisualElement home;

    public FlexSwitcher(VisualElement home)
    {
        visualElements = new List<VisualElement>();
        visualElements.Add(home);
        this.home = home;

        switchTo = (VisualElement element) => { };
        switchFrom = (VisualElement element) => { };

        Return();
    }
    public FlexSwitcher() : this(noElement) { }

    public void Add(VisualElement element)
    {
        visualElements.Add(element);
    }

    public void Switch(VisualElement element)
    {
        VisualElement last = current;
        HideAll();
        element.style.display = DisplayStyle.Flex;
        current = element;
        if (last != current)
        {
            switchFrom.Invoke(last);
            switchTo.Invoke(current);
        }
    }

    public void Return()
    {
        Switch(home);
    }

    public void HideAll()
    {
        current = noElement;
        foreach (VisualElement element in visualElements)
        {
            element.style.display = DisplayStyle.None;
        }
    }

    public void ShowAll()
    {
        current = allElements;
        foreach (VisualElement element in visualElements)
        {
            element.style.display = DisplayStyle.Flex;
        }
    }
}