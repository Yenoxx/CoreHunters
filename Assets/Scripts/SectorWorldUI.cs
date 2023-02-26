using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class SectorWorldUI : MonoBehaviour
{
    private UIDocument document;
    private VisualElement wrapperScreen;

    private VisualElement contextButtons;

    private Button buttonOK;
    private Button buttonFlip;
    private Button buttonCancel;
    private Button buttonInfo;
    private Button buttonProduction;
    private Button buttonMove;
    private Button buttonDestroy;

    private Dictionary<string, Button> buttons;

    private bool buildingContextVisible;
    private GameObject selectedBuilding;

    void Awake()
    {
        ProviderUmpaLumpa.sectorWorldUI = this;

        document = GetComponent<UIDocument>();
        document.enabled = true;
        wrapperScreen = document.rootVisualElement.Q<VisualElement>("WrapperScreen");

        contextButtons = wrapperScreen.Q<VisualElement>("ContextButtons");

        buttonOK = contextButtons.Q<Button>("ButtonOK");
        buttonFlip = contextButtons.Q<Button>("ButtonFlip");
        buttonCancel = contextButtons.Q<Button>("ButtonCancel");
        buttonInfo = contextButtons.Q<Button>("ButtonInfo");
        buttonProduction = contextButtons.Q<Button>("ButtonProduction");
        buttonMove = contextButtons.Q<Button>("ButtonMove");
        buttonDestroy = contextButtons.Q<Button>("ButtonDestroy");

        buttons = new Dictionary<string, Button>();
    }

    void Start()
    {
        HideContextButtons();

        RegisterButton("ok", buttonOK);
        RegisterButton("flip", buttonFlip);
        RegisterButton("cancel", buttonCancel);
        RegisterButton("info", buttonInfo);
        RegisterButton("production", buttonProduction);
        RegisterButton("move", buttonMove);
        RegisterButton("destroy", buttonDestroy);
    }

    void LateUpdate()
    {
        if (buildingContextVisible)
        {
            if (selectedBuilding != null)
            {
                Building building = selectedBuilding.GetComponent<Building>();
                Vector3 buildingPosition = building.transform.position + new Vector3(building.contextButtonOffset.x, building.contextButtonOffset.y, 0);
                Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
                    contextButtons.panel, buildingPosition, ProviderUmpaLumpa.sectorCamera.cameraComponent);
                newPosition.x -= contextButtons.layout.width / 2 + 24;
                contextButtons.transform.position = newPosition;
            }
            else
            {
                HideContextButtons();
            }
        }
    }

    public void ShowContextButtons(GameObject selectedBuilding, params string[] buttonNames)
    {
        contextButtons.style.display = DisplayStyle.Flex;
        buildingContextVisible = true;
        this.selectedBuilding = selectedBuilding;

        foreach (VisualElement element in contextButtons.Children())
        {
            element.style.display = DisplayStyle.None;
        }
        foreach (string buttonName in buttonNames)
        {
            if (buttons.ContainsKey(buttonName)) 
            {
                buttons[buttonName].style.display = DisplayStyle.Flex;
            }
        }
    }

    public void HideContextButtons()
    {
        contextButtons.style.display = DisplayStyle.None;
        buildingContextVisible = false;
    }

    private void RegisterButton(string name, Button button)
    {
        buttons.Add(name, button);
        button.clicked += () =>
        {
            SendBuildingContextActionString(name);
        };
    }

    private void SendBuildingContextActionString(string actionString)
    {
        Building building = selectedBuilding.GetComponent<Building>();
        building.ReceiveContextActionString(actionString);
    }
}
