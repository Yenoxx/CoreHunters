using System.Collections.Generic;
using UnityEngine.UIElements;

public class CategoryButton : IconButton
{
    public const string CATEGORY_ALL = "#all";

    public bool pressed { get; private set; }
    public Group group { get; set; }

    public string categoryName { get; set; }

    public CategoryButton() : base()
    {
        pressed = false;

        clicked += () =>
        {
            Click();
        };
    }

    public void Click()
    {
        if (pressed)
        {
            if (group != null) group.ResetCurrentCategoryName();
            Release();
        }
        else
        {
            if (group != null) 
            {
                group.ReleaseAll();
                group.SetCurrentCategoryName(categoryName);
            }
            Press();
        }
    }

    public void Press()
    {
        AddToClassList("button-category-active");
        RemoveFromClassList("button-category");
        pressed = true;
    }

    public void Release()
    {
        AddToClassList("button-category");
        RemoveFromClassList("button-category-active");
        pressed = false;
    }

    public new class UxmlFactory : UxmlFactory<CategoryButton, UxmlTraits> { }

    public new class UxmlTraits : IconButton.UxmlTraits
    {
        UxmlStringAttributeDescription m_CategoryName =
            new UxmlStringAttributeDescription { name = "category-name", defaultValue = "" };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            CategoryButton ate = ve as CategoryButton;

            ate.AddToClassList("button-category");
            ate.RemoveFromClassList("button-icon");
            ate.categoryName = m_CategoryName.GetValueFromBag(bag, cc);
        }
    }

    public class Group
    {
        private List<CategoryButton> buttons;

        public string currentCategoryName { get; private set; }

        public Group()
        {
            buttons = new List<CategoryButton>();
            ResetCurrentCategoryName();
        }

        public void Add(CategoryButton button)
        {
            buttons.Add(button);
            button.group = this;
        }

        public void ReleaseAll()
        {
            foreach (CategoryButton button in buttons)
            {
                button.Release();
            }
        }

        public void SetCurrentCategoryName(string categoryName)
        {
            currentCategoryName = categoryName;
        }

        public void ResetCurrentCategoryName()
        {
            currentCategoryName = CATEGORY_ALL;
        }
    }
}