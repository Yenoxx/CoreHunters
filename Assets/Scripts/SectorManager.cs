using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using DigitalRuby.Tween;

public class SectorManager : MonoBehaviour
{
    public enum GameState
    {
        NORMAL,
        BUILD,
        PAUSE,
    }

    // Specify in editor
    public Tilemap tilemapLiquid;
    public Tilemap tilemapTerrain;
    public Tilemap tilemapFloor;
    public Tilemap tilemapMarkup;
    public Tilemap tilemapMarkupBuild;
    public TileBase markupGreen;
    public TileBase markupRed;
    public TileBase markupWhite;
    public GameObject buildingPrefab;

    // TODO: ramove later (very later)
    public Vector2Int mapSize;
    public FactionData faction;
    public PlanetData planet;

    private GameObject currentBuilding;
    private BuildingData buildingData;
    private bool flipMode;

    private Save save;
    public Save Save { private set => save = value; get => save; }
    private Map map;
    public Map Map { private set => map = value; get => map; }
    private GameState state;
    public GameState State
    {
        set
        {
            OnModeChanged(state, value);
            state = value;
        }
        get
        {
            return state;
        }
    }

    void Awake()
    {
        ProviderUmpaLumpa.sectorManager = this;

        // TODO: move to an appropriate place
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 200;

        GameObject eventSystemGameObject = GameObject.Find("EventSystem");
        ProviderUmpaLumpa.eventSystem = eventSystemGameObject.GetComponent<EventSystem>();
    }

    void Start()
    {
        flipMode = false;

        HideMarkup();
        Generate();
    }

    void OnModeChanged(GameState lastMode, GameState mode)
    {
        if (lastMode != mode) 
        {
            // Entering mode
            switch (mode)
            {
                case GameState.NORMAL:
                {
                    break;
                }
                case GameState.BUILD:
                {
                    ShowMarkup();
                    CreateBuilding();
                    break;
                }
            }

            // Exiting mode
            switch (lastMode)
            {
                case GameState.NORMAL:
                {
                    break;
                }
                case GameState.BUILD:
                {
                    HideMarkup();
                    if (currentBuilding != null)
                    {
                        ClearMarkupBuild();
                        Destroy(currentBuilding);
                    }
                    break;
                }
            }
        }
    }

    void CreateBuilding()
    {
        Transform cameraTransform = ProviderUmpaLumpa.sectorCamera.GetComponent<Transform>();
        Vector3 position = new Vector3(cameraTransform.position.x, cameraTransform.position.y, 0);
        currentBuilding = Instantiate<GameObject>(buildingPrefab, position, Quaternion.identity);
        Building building = currentBuilding.GetComponent<Building>();
        building.buildingData = buildingData;
        if (flipMode) building.Flip();
    }

    public void Build()
    {
        if (State != GameState.BUILD) return;

        Building building = currentBuilding.GetComponent<Building>();
        if (BuildingPositionAvailable(building))
        {
            building.mode = Building.BehaviorMode.READY;
            AddMarkup(building);
            CreateBuilding();
        }
    }

    public void Flip()
    {
        if (State != GameState.BUILD) return;
        flipMode = !flipMode;
        
        Building building = currentBuilding.GetComponent<Building>();
        building.Flip();
    }

    public void InitiateBuild(BuildingData buildingData)
    {
        this.buildingData = buildingData;
        State = GameState.BUILD;
        ProviderUmpaLumpa.sectorMenu.flexSwitcherScreen.Return();
    }

    public void ToggleBuild()
    {
        if (State == GameState.BUILD) State = GameState.NORMAL;
        else State = GameState.BUILD;
    }
    
    public void ShowMarkup()
    {
        tilemapMarkup.GetComponent<Renderer>().enabled = true;
        tilemapMarkupBuild.GetComponent<Renderer>().enabled = true;
    }
    public void HideMarkup()
    {
        tilemapMarkup.GetComponent<Renderer>().enabled = false;
        tilemapMarkupBuild.GetComponent<Renderer>().enabled = false;
    }

    public TileBase ChooseTile(TileBase[] tiles)
    {
        return tiles[Random.Range(0, tiles.Length)];
    }

    public TileBase ChooseTile(TileBase[] tiles, int difference)
    {
        TileBase tile = tiles[Random.Range(0, tiles.Length)];
        if (tile is AnimatedTile)
        {
            AnimatedTile animatedTile = tile as AnimatedTile;
            AnimatedTile otherTile = new AnimatedTile();
            otherTile.m_AnimatedSprites = animatedTile.m_AnimatedSprites;
            otherTile.m_AnimationStartFrame = (animatedTile.m_AnimationStartFrame + difference) % animatedTile.m_AnimatedSprites.Length;
            otherTile.m_AnimationStartTime = animatedTile.m_AnimationStartTime;
            otherTile.m_MaxSpeed = animatedTile.m_MaxSpeed;
            otherTile.m_MinSpeed = animatedTile.m_MinSpeed;
            otherTile.m_TileColliderType = animatedTile.m_TileColliderType;
            return otherTile;
        }
        else
        {
            return tile;
        }
    }

    public void Generate() 
    {
        SaverUmpaLumpa.CreateSave(faction, planet, mapSize);
        Map = SaverUmpaLumpa.save.map;
        SaverUmpaLumpa.GenerateMap();

        tilemapLiquid.ClearAllTiles();
        tilemapTerrain.ClearAllTiles();
        tilemapFloor.ClearAllTiles();
        tilemapMarkup.ClearAllTiles();
        tilemapMarkupBuild.ClearAllTiles();

        for (int y = 0; y < mapSize.y; y++) 
        {
            for (int x = 0; x < mapSize.x; x++) 
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                switch (Map.GetTileType(x, y))
                {
                    case MapTile.Type.CHASM:
                    {
                        // Actually nothing!
                        break;
                    }
                    case MapTile.Type.LIQUID:
                    {
                        tilemapLiquid.SetTile(pos, ChooseTile(planet.tileCollection.liquidTiles, x));
                        tilemapMarkup.SetTile(pos, markupRed);
                        break;
                    }
                    case MapTile.Type.SOFT:
                    {
                        tilemapTerrain.SetTile(pos, ChooseTile(planet.tileCollection.softTiles));
                        tilemapMarkup.SetTile(pos, markupRed);
                        break;
                    }
                    case MapTile.Type.SOLID:
                    {
                        tilemapTerrain.SetTile(pos, ChooseTile(planet.tileCollection.solidTiles));
                        tilemapMarkup.SetTile(pos, markupWhite);
                        break;
                    }
                }
            }
        }
    }

    public bool BuildingPositionAvailable(Building building)
    {
        if (Map == null) return false;

        foreach (int x in building.RangeX())
        {
            foreach (int y in building.RangeY())
            {
                MapTile tile = Map.GetTile(x, y);
                if (tile == null) return false;
                if (!tile.available) return false;
            }
        }

        return true;
    }
    public bool BuildingPositionAvailable(Building building, int xOverride, int yOverride)
    {
        if (Map == null) return false;

        foreach (int x in building.RangeX(xOverride))
        {
            foreach (int y in building.RangeY(yOverride))
            {
                MapTile tile = Map.GetTile(x, y);
                if (tile == null) return false;
                if (!tile.available) return false;
            }
        }

        return true;
    }

    public void ClearMarkupBuild()
    {
        tilemapMarkupBuild.ClearAllTiles();
    }

    public void UpdateMarkupBuild(Building building)
    {
        TileBase currentMarkupTile;
        if (BuildingPositionAvailable(building)) currentMarkupTile = markupGreen;
        else currentMarkupTile = markupRed;

        ClearMarkupBuild();
        foreach (int x in building.RangeX())
        {
            foreach (int y in building.RangeY())
            {
                tilemapMarkupBuild.SetTile(new Vector3Int(x, y, 0), currentMarkupTile);
            }
        }
    }

    public void AddMarkup(Building building)
    {
        foreach (int x in building.RangeX())
        {
            foreach (int y in building.RangeY())
            {
                tilemapMarkup.SetTile(new Vector3Int(x, y, 0), markupRed);
                MapTile tile = Map.GetTile(x, y);
                tile.hasBuilding = true;
            }
        }
    }

    public Vector3Int GetCellPosition(Vector3 near)
    {
        return new Vector3Int(
            Mathf.FloorToInt(( near.x / Constants.CELL_HALF_SIZE_X + near.y / Constants.CELL_HALF_SIZE_Y) * 0.5f),
            Mathf.FloorToInt((-near.x / Constants.CELL_HALF_SIZE_X + near.y / Constants.CELL_HALF_SIZE_Y) * 0.5f),
            0
        );
    }

    public Vector3 GetCellWorldPosition(Vector3 near)
    {
        return tilemapMarkup.GetCellCenterWorld(GetCellPosition(near));
    }

    public Vector3 GetCellWorldPosition(Vector3Int cellPosition)
    {
        return tilemapMarkup.GetCellCenterWorld(cellPosition);
    }
}
