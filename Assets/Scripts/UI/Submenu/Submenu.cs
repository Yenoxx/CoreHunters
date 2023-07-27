using UnityEngine.UIElements;

public abstract class Submenu
{
    public VisualElement wrapper {get; protected set;}

    public virtual void RegisterCallbacks()
    {

    }

    public virtual void OnShow()
    {

    }

    public virtual void OnHide()
    {

    }
}