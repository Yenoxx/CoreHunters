using UnityEngine;
using UnityEngine.UIElements;

public class SectorMenu : MonoBehaviour
{
    public VisualTreeAsset submenuBuildTree;
    public VisualTreeAsset submenuBuildingTree;
    public VisualTreeAsset submenuConstructionTree;

    private FlexSwitcher flexSwitcherScreen;

    private UIDocument document;
    private VisualElement safeArea;
    private VisualElement wrapperScreen;

    private VisualElement menuSector;

    private Button buttonBuild;
    private Button buttonProduction;
    private Button buttonResearch;
    private Button buttonDevTools;
    private Button buttonMap;

    private SubmenuBuild submenuBuild;
    private SubmenuBuilding submenuBuilding;
    private SubmenuConstruction submenuConstruction;


    void Awake()
    {
        ProviderUmpaLumpa.sectorMenu = this;

        document = GetComponent<UIDocument>();
        safeArea = document.rootVisualElement.Q<VisualElement>("SafeArea");
        wrapperScreen = document.rootVisualElement.Q<VisualElement>("WrapperScreen");

        // TODO: i don't know what to do with this
        // var safeAreaRectOffset = document.rootVisualElement.panel.GetSafeArea();
        // safeArea.style.paddingLeft = safeAreaRectOffset.Left;
        // safeArea.style.paddingRight = safeAreaRectOffset.Right;
        // safeArea.style.paddingTop = safeAreaRectOffset.Top;
        // safeArea.style.paddingBottom = safeAreaRectOffset.Bottom;

        menuSector = wrapperScreen.Q<VisualElement>("MenuSector");

        buttonBuild = menuSector.Q<Button>("ButtonBuild");
        buttonProduction = menuSector.Q<Button>("ButtonProduction");
        buttonResearch = menuSector.Q<Button>("ButtonResearch");
        buttonDevTools = menuSector.Q<Button>("ButtonDevTools");
        buttonMap = menuSector.Q<Button>("ButtonMap");
    }

    void Start()
    {
        flexSwitcherScreen = new FlexSwitcher(menuSector);

        submenuBuild = new SubmenuBuild(submenuBuildTree.CloneTree());
        wrapperScreen.Add(submenuBuild.wrapper);
        flexSwitcherScreen.Add(submenuBuild.wrapper);

        submenuBuilding = new SubmenuBuilding(submenuBuildingTree.CloneTree());
        wrapperScreen.Add(submenuBuilding.wrapper);
        flexSwitcherScreen.Add(submenuBuilding.wrapper);

        submenuConstruction = new SubmenuConstruction(submenuConstructionTree.CloneTree());
        wrapperScreen.Add(submenuConstruction.wrapper);
        flexSwitcherScreen.Add(submenuConstruction.wrapper);

        Return();

        RegisterCallbacks();
        submenuBuild.RegisterCallbacks();
        submenuBuilding.RegisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        ProviderUmpaLumpa.sectorCamera.clicked += () => 
        {
            if (flexSwitcherScreen.current == submenuBuilding.wrapper) Return();
        };

        flexSwitcherScreen.switchTo += (VisualElement element) =>
        {
            if (element == submenuBuilding.wrapper)
            {
                submenuBuilding.OnShow();
            }
        };

        flexSwitcherScreen.switchFrom += (VisualElement element) =>
        {
            if (element == submenuBuilding.wrapper)
            {
                submenuBuilding.OnHide();
            }
        };

        buttonBuild.clicked += () =>
        {
            flexSwitcherScreen.Switch(submenuBuild.wrapper);
            submenuBuild.UpdateBBEs();
        };

        buttonDevTools.clicked += () =>
        {
            // TODO: idk show dev tools then
        };
    }

    public void ShowSubmenuBuilding(Building building)
    {
        if (flexSwitcherScreen.current != submenuBuilding.wrapper)
        {
            submenuBuilding.building = building;
            flexSwitcherScreen.Switch(submenuBuilding.wrapper);
        }
    }

    public void ShowSubmenuConstruction()
    {
        flexSwitcherScreen.Switch(submenuConstruction.wrapper);
    }

    public void HideUI()
    {
        flexSwitcherScreen.HideAll();
    }

    public void Return()
    {
        flexSwitcherScreen.Return();
    }
}
