using System.Collections.Generic;
using UnityEngine.UIElements;

public class IconButton : Button
{
    public const string ICON_PATH_PREFIX = "Sprites/Icons/Icon";

    public string iconString { get; set; }

    public StyleBackground GetBackground()
    {
        return new StyleBackground(LoaderUmpaLumpa.LoadSprite(ICON_PATH_PREFIX + iconString));
    }

    public new class UxmlFactory : UxmlFactory<IconButton, UxmlTraits> { }

    public new class UxmlTraits : Button.UxmlTraits
    {
        UxmlStringAttributeDescription m_IconString =
            new UxmlStringAttributeDescription { name = "icon-string", defaultValue = "" };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            IconButton ate = ve as IconButton;

            ate.iconString = m_IconString.GetValueFromBag(bag, cc);
            ate.focusable = false;
            ate.AddToClassList("button-icon");

            VisualElement icon = ate.Q("Icon");
            if (icon == null)
            {
                VisualElement iconElement = new VisualElement();
                iconElement.name = "Icon";
                iconElement.pickingMode = PickingMode.Ignore;
                iconElement.style.flexGrow = new StyleFloat(1f);
                iconElement.style.backgroundImage = ate.GetBackground();
                ate.Add(iconElement);
            }
            else
            {
                icon.style.backgroundImage = ate.GetBackground();
            }
        }
    }
}
