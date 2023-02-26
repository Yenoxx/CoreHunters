using UnityEngine;
using UnityEngine.UIElements;

public class SectorMenu : MonoBehaviour
{
    public VisualTreeAsset submenuBuildTree;
    public VisualTreeAsset submenuBuildingTree;

    public FlexSwitcher flexSwitcherScreen { get; private set; }

    private UIDocument document;
    private VisualElement wrapperScreen;

    private VisualElement menuSector;

    private Button buttonBuild;
    private Button buttonProduction;
    private Button buttonResearch;
    private Button buttonDevTools;
    private Button buttonMap;

    private SubmenuBuildSection submenuBuildSection;
    private SubmenuBuildingSection submenuBuildingSection;

    void Awake()
    {
        ProviderUmpaLumpa.sectorMenu = this;

        document = GetComponent<UIDocument>();
        wrapperScreen = document.rootVisualElement.Q<VisualElement>("WrapperScreen");

        menuSector = wrapperScreen.Q<VisualElement>("MenuSector");

        buttonBuild = menuSector.Q<Button>("ButtonBuild");
        buttonProduction = menuSector.Q<Button>("ButtonProduction");
        buttonResearch = menuSector.Q<Button>("ButtonResearch");
        buttonDevTools = menuSector.Q<Button>("ButtonDevTools");
        buttonMap = menuSector.Q<Button>("ButtonMap");
    }

    void Start()
    {
        ProviderUmpaLumpa.sectorCamera.clicked += () => 
        {
            HideSubmenuBuilding();
        };

        flexSwitcherScreen = new FlexSwitcher(menuSector);

        submenuBuildSection = new SubmenuBuildSection(submenuBuildTree.CloneTree());
        wrapperScreen.Add(submenuBuildSection.wrapper);
        flexSwitcherScreen.Add(submenuBuildSection.wrapper);

        submenuBuildingSection = new SubmenuBuildingSection(submenuBuildingTree.CloneTree());
        wrapperScreen.Add(submenuBuildingSection.wrapper);
        flexSwitcherScreen.Add(submenuBuildingSection.wrapper);

        flexSwitcherScreen.Return();

        buttonBuild.clicked += () =>
        {
            flexSwitcherScreen.Switch(submenuBuildSection.wrapper);
            submenuBuildSection.UpdateBBEs();
        };

        buttonDevTools.clicked += () =>
        {
            // TODO: idk show dev tools then
        };

        submenuBuildSection.RegisterCallbacks();
        submenuBuildingSection.RegisterCallbacks();
    }

    public void ShowSubmenuBuilding(Building building)
    {
        if (flexSwitcherScreen.current != submenuBuildingSection.wrapper)
        {
            submenuBuildingSection.building = building;
            submenuBuildingSection.OnShow();

            Vector2 position = new Vector2(building.transform.position.x, building.transform.position.y);
            ProviderUmpaLumpa.sectorCamera.CreateSnapshot();
            ProviderUmpaLumpa.sectorCamera.FocusOn(position + building.centerOffset, new Vector2(0, -0.3f));

            flexSwitcherScreen.Switch(submenuBuildingSection.wrapper);
        }
    }

    public void HideSubmenuBuilding()
    {
        if (flexSwitcherScreen.current == submenuBuildingSection.wrapper)
        {
            submenuBuildingSection.OnHide();

            ProviderUmpaLumpa.sectorCamera.LoadSnapshot();
            
            flexSwitcherScreen.Return();
        }
    }
}
