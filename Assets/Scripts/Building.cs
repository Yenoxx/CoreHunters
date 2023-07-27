using System;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;

[ExecuteAlways]
public class Building : MonoBehaviour
{
    // Behavior mode of a building
    // NONE:  initial state
    // BUILD: is being placed for the first time
    // MOVE:  is being moved to another place
    // READY: is built
    public enum BehaviorMode
    {
        NONE,
        BUILD,
        MOVE,
        READY,
    }

    // Automatic generated shadow angle (always the same)
    public const float SHADOW_ANGLE = -10;
    // Guess what is it
    public const float DOUBLE_CLICK_INTERVAL = 0.3f;

    // Context button names shown when in BUILD or MOVE mode
    private static string[] contextBuildMove = {"ok", "flip", "cancel"};

    // Public fields specified in editor
    public Sprite sprite;
    public Sprite spriteFlip;
    public Vector2 spriteOffset;
    public Vector2Int cellSize;
    [Range(0.1f, 2f)]
    public float shadowScale;
    public Vector2 shadowOffset;
    public bool flip;
    public Material sampleShadowMaterial;
    public Material sampleSpriteMaterial;

    // References to components, child objects and their fields
    private PolygonCollider2D collider_;
    private GameObject spriteGameObject;
    private Vector3 spriteGOLocalPosition;
    private SpriteRenderer spriteRenderer;
    private Material spriteMaterial;
    private GameObject shadowGameObject;
    private MeshFilter shadowMeshFilter;
    private MeshRenderer shadowMeshRenderer;
    private Mesh shadowMesh;
    private Material shadowMaterial;

    // Gameplay mechanics
    public ProductionVariant currentProductionVariant;

    // Press state and mechanics
    public bool pressed { get; set; }
    public bool justPressed { get; set; }
    public bool justReleased { get; set; }
    private Clock doubleClickClock;
    private Vector3 pressOffset;

    // Tween related
    private Action<ITween<Vector3>> motionUF;
    private Action<ITween<Vector3>> dragMotionUF;
    private Action<ITween<Vector3>> jumpUF;
    private Action<ITween<float>> blinkUF;
    private FloatTween blinkTween1;
    private FloatTween blinkTween2;
    private bool _blinking;
    public bool blinking
    {
        get => _blinking;
        set
        {
            _blinking = value;
            if (_blinking == false)
            {
                StopBlinking();
            }
        }
    }

    // Colors
    private static Color _colorNormal = Color.white;
    public static Color colorNormal { get => _colorNormal; }
    private static Color _colorDimmed = new Color(1, 1, 1, 0.5f);
    public static Color colorDimmed { get => _colorDimmed; }

    // Position in grid space
    private Vector3Int _cellPosition;
    public Vector3Int cellPosition
    {
        private set
        {
            if (_cellPosition != value)
            {
                Vector3Int lastPosition = _cellPosition;
                _cellPosition = value;
                OnCellPositionChanged(lastPosition, cellPosition);
            }
        }
        get => _cellPosition;
    }

    // BehaviorMode property (calls OnModeChanged)
    private BehaviorMode _mode;
    public BehaviorMode mode
    {
        set
        {
            if (_mode != value)
            {
                OnModeChanged(_mode, value);
                _mode = value;
            }
        }
        get
        {
            return _mode;
        }
    }

    // BuildingData property (applies BuildingData settings)
    private BuildingData _buildingData = default;
    public BuildingData buildingData
    {
        set
        {
            _buildingData = value;
            sprite = _buildingData.sprite;
            spriteFlip = _buildingData.spriteFlip;
            spriteOffset = _buildingData.spriteOffset;
            cellSize = _buildingData.cellSize;
            shadowScale = _buildingData.shadowScale;
            shadowOffset = _buildingData.shadowOffset;
        }
        get => _buildingData;
    }

    // Misc
    private Vector2 sortingNormal;
    public Vector2 centerOffset { get; private set; }
    public Vector2 contextButtonOffset { get; private set; }

    void Awake()
    {
        collider_ = GetComponent<PolygonCollider2D>();
        spriteGameObject = transform.Find("Sprite").gameObject;
        spriteRenderer = spriteGameObject.GetComponent<SpriteRenderer>();
        spriteMaterial = new Material(sampleSpriteMaterial);
        spriteRenderer.material = spriteMaterial;
        shadowGameObject = transform.Find("Shadow").gameObject;
        shadowMeshFilter = shadowGameObject.GetComponent<MeshFilter>();
        shadowMesh = new Mesh();
        shadowMeshFilter.mesh = shadowMesh;
        shadowMeshRenderer = shadowGameObject.GetComponent<MeshRenderer>();
        shadowMaterial = new Material(sampleShadowMaterial);
        shadowMeshRenderer.material = shadowMaterial;
    }

    void Start()
    {
        _cellPosition = new Vector3Int();

        mode = BehaviorMode.BUILD;

        pressed = false;
        justPressed = false;
        justReleased = false;
        doubleClickClock = new Clock(DOUBLE_CLICK_INTERVAL);
        pressOffset = new Vector3();

        blinking = false;

        sortingNormal = new Vector2();

        if (Application.IsPlaying(gameObject))
        {
            motionUF = (t) => 
            { 
                transform.position = t.CurrentValue;
            };
            dragMotionUF = (t) => 
            { 
                SetPosition(t.CurrentValue);
            };
            jumpUF = (t) => 
            {
                spriteGameObject.transform.localPosition = t.CurrentValue;
            };
            blinkUF = (t) =>
            {
                spriteMaterial.SetFloat("_MixWhite", t.CurrentValue);
            };

            UpdateSpriteGeometry();
            UpdateGeometry();
            cellPosition = GetCellPosition();
            if (!MoveToAvailablePosition())
            {
                ProviderUmpaLumpa.sectorManager.ToggleBuild();
            }
            ProviderUmpaLumpa.sectorManager.UpdateMarkupBuild(this);
        }
    }

    void Update()
    {
        if (Application.IsPlaying(gameObject))
        {
            if (mode == BehaviorMode.BUILD || mode == BehaviorMode.MOVE)
            {
                if (justPressed)
                {
                    ProviderUmpaLumpa.sectorCamera.locked = true;
                    Vector3 mousePosition = ProviderUmpaLumpa.sectorCamera.GetMouseWorldPosition();
                    Vector3 position = new Vector3(mousePosition.x, mousePosition.y, 0);
                    pressOffset = transform.position - position;
                }

                if (justReleased)
                {
                    ProviderUmpaLumpa.sectorCamera.locked = false;
                }

                if (pressed)
                {
                    Vector3 mousePosition = ProviderUmpaLumpa.sectorCamera.GetMouseWorldPosition();
                    Vector3 position = new Vector3(mousePosition.x, mousePosition.y, 0) + pressOffset;
                    gameObject.Tween(
                        gameObject, 
                        transform.position, 
                        position, 
                        0.05f, TweenScaleFunctions.QuadraticEaseOut, dragMotionUF);
                }
                else
                {
                    gameObject.Tween(
                        gameObject, 
                        transform.position, 
                        GetCellWorldPosition(cellPosition), 
                        0.1f, TweenScaleFunctions.QuadraticEaseOut, motionUF);
                }
            }
            else if (mode == BehaviorMode.READY)
            {
                if (justReleased)
                {
                    if (!ProviderUmpaLumpa.sectorCamera.dragCounted)
                    {
                        if (doubleClickClock.active)
                        {
                            ProviderUmpaLumpa.sectorMenu.ShowSubmenuBuilding(this);
                        }
                        else
                        {
                            doubleClickClock.Start();
                            DubstepRave();
                        }
                    }
                }

                if (blinking) Blink();

                doubleClickClock.Update();
            }

            justPressed = false;
            justReleased = false;
        }
        else
        {
            UpdateSpriteGeometry();
            UpdateGeometry();
        }
    }

    void OnDisable()
    {
        TweenFactory.RemoveTweenKey(gameObject, TweenStopBehavior.Complete);
    }

    void OnMouseDown()
    {
        if (!ProviderUmpaLumpa.eventSystem.IsPointerOverGameObject())
        {
            justPressed = true;
            pressed = true;
        }
    }

    void OnMouseUp()
    {
        if (!ProviderUmpaLumpa.eventSystem.IsPointerOverGameObject())
        {
            justReleased = true;
            pressed = false;
        }
    }

    // Automatically called when mode is changed
    void OnModeChanged(BehaviorMode lastMode, BehaviorMode mode)
    {
        switch (mode)
        {
            case BehaviorMode.BUILD:
            {
                spriteRenderer.color = colorDimmed;
                if (Application.IsPlaying(gameObject)) 
                {
                    ProviderUmpaLumpa.sectorWorldUI.ShowContextButtons(gameObject, contextBuildMove);
                }
                break;
            }
            case BehaviorMode.MOVE:
            {
                spriteRenderer.color = colorDimmed;
                if (Application.IsPlaying(gameObject)) 
                {
                    ProviderUmpaLumpa.sectorWorldUI.ShowContextButtons(gameObject, contextBuildMove);
                }
                break;
            }
            case BehaviorMode.READY:
            {
                spriteRenderer.color = colorNormal;
                if (buildingData.productionVariants.Length > 0)
                {
                    currentProductionVariant = buildingData.productionVariants[0];
                }
                if (Application.IsPlaying(gameObject)) 
                {
                    ProviderUmpaLumpa.sectorWorldUI.HideContextButtons();
                }
                break;
            }
        }
    }

    // Automatically called when cell position is changed
    void OnCellPositionChanged(Vector3Int lastPosition, Vector3Int position)
    {
        ProviderUmpaLumpa.sectorManager.UpdateMarkupBuild(this);
        UpdateSortingOrder();
    }

    // Context button action receiver
    public void ReceiveContextActionString(string actionString)
    {
        switch (actionString)
        {
            case "ok":
            {
                if (mode == BehaviorMode.BUILD)
                {
                    ProviderUmpaLumpa.sectorManager.Build();
                }
                break;
            }
            case "flip":
            {
                if (mode == BehaviorMode.BUILD || mode == BehaviorMode.MOVE)
                {
                    ProviderUmpaLumpa.sectorManager.Flip();
                }
                break;
            }
            case "cancel":
            {
                if (mode == BehaviorMode.BUILD)
                {
                    ProviderUmpaLumpa.sectorManager.State = SectorManager.GameState.NORMAL;
                }
                break;
            }
            case "info":
            {

                break;
            }
        }
    }

    public void DubstepRave()
    {
        Vector3 midPoint = spriteGOLocalPosition + new Vector3(0, 0.1f, 0);
        gameObject.Tween(
            gameObject + "_jump", 
            spriteGOLocalPosition,
            midPoint, 
            0.1f, TweenScaleFunctions.QuadraticEaseOut, jumpUF)
        .ContinueWith(new Vector3Tween().Setup(
            midPoint, 
            spriteGOLocalPosition, 
            0.1f, TweenScaleFunctions.QuadraticEaseIn, jumpUF));
    }

    public void Blink()
    {
        if (blinkTween1 == null || blinkTween2 == null 
            || blinkTween1.State != TweenState.Running && blinkTween2.State != TweenState.Running)
        {
            blinkTween1 = gameObject.Tween(
                gameObject + "_blink", 
                0f,
                0.2f,
                1f, TweenScaleFunctions.QuadraticEaseInOut, blinkUF);
            blinkTween2 = (FloatTween) blinkTween1.ContinueWith(new FloatTween().Setup(
                0.2f, 
                0f, 
                1f, TweenScaleFunctions.QuadraticEaseInOut, blinkUF));
        }
    }

    public void StopBlinking()
    {
        if (blinkTween1 != null && blinkTween2 != null
            && (blinkTween1.State == TweenState.Running || blinkTween2.State == TweenState.Running))
        {
            blinkTween1.Stop(TweenStopBehavior.Complete);
            blinkTween2.Stop(TweenStopBehavior.Complete);
        }
    }

    private void UpdateSortingOrder()
    {
        //spriteRenderer.sortingOrder = -Mathf.FloorToInt(
        //   (cellPosition.x + cellPosition.y) * 10 +
        //   (cellPosition.x - cellPosition.y) * (sortingNormal.y > 0 ? 1 : -1));
        spriteRenderer.sortingOrder = -Mathf.FloorToInt(
          (cellPosition.x + cellPosition.y) * 2 +
          (cellSize.x + cellSize.y));
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;

        Vector3Int probableCellPosition = GetCellPosition();
        if (ProviderUmpaLumpa.sectorManager.BuildingPositionAvailable(this, probableCellPosition.x, probableCellPosition.y))
        {
            cellPosition = probableCellPosition;
        }
    }

    public bool MoveToAvailablePosition()
    {
        if (ProviderUmpaLumpa.sectorManager.Map == null) 
        {
            SetPosition(GetCellWorldPosition(cellPosition));
            return false;
        }
        if (ProviderUmpaLumpa.sectorManager.BuildingPositionAvailable(this)) {
            SetPosition(GetCellWorldPosition(cellPosition));
            return true;
        }

        bool inBounds = true;
        bool foundTile = false;
        int halfSide = 1;

        while (inBounds)
        {
            foundTile = false;

            for (int x = cellPosition.x - halfSide; x <= cellPosition.x + halfSide; x++) 
            {
                for (int y = cellPosition.y - halfSide; y <= cellPosition.y + halfSide; y += halfSide * 2)
                {
                    if (ProviderUmpaLumpa.sectorManager.Map.GetTile(x, y) != null) foundTile = true;
                    if (ProviderUmpaLumpa.sectorManager.BuildingPositionAvailable(this, x, y)) {
                        SetPosition(GetCellWorldPosition(new Vector3Int(x, y, 0)));
                        return true;
                    }
                }
            }
            for (int x = cellPosition.x - halfSide; x <= cellPosition.x + halfSide; x += halfSide * 2) 
            {
                for (int y = cellPosition.y - halfSide + 1; y <= cellPosition.y + halfSide - 1; y++)
                {
                    if (ProviderUmpaLumpa.sectorManager.Map.GetTile(x, y) != null) foundTile = true;
                    if (ProviderUmpaLumpa.sectorManager.BuildingPositionAvailable(this, x, y)) {
                        SetPosition(GetCellWorldPosition(new Vector3Int(x, y, 0)));
                        return true;
                    }
                }
            }

            halfSide++;
            if (!foundTile) inBounds = false;
        }

        return false;
    }

    public void Flip()
    {
        flip = !flip;
        if (Application.IsPlaying(gameObject))
        {
            UpdateSpriteGeometry();
            UpdateGeometry();
            UpdateSortingOrder();
        }
        ProviderUmpaLumpa.sectorManager.UpdateMarkupBuild(this);
    }

    public IEnumerable<int> RangeX(int xOverride)
    {
        if (flip)
        {
            for (int x = xOverride; x <= xOverride + cellSize.y - 1; x++)
            {
                yield return x;
            }
        }
        else
        {
            for (int x = xOverride; x <= xOverride + cellSize.x - 1; x++)
            {
                yield return x;
            }
        }
    }
    public IEnumerable<int> RangeX()
    {
        return RangeX(cellPosition.x);
    }

    public IEnumerable<int> RangeY(int yOverride)
    {
        if (flip)
        {
            for (int y = yOverride; y <= yOverride + cellSize.x - 1; y++)
            {
                yield return y;
            }
        }
        else
        {
            for (int y = yOverride; y <= yOverride + cellSize.y - 1; y++)
            {
                yield return y;
            }
        }
    }
    public IEnumerable<int> RangeY()
    {
        return RangeY(cellPosition.y);
    }

    void UpdateGeometry()
    {
        // Collider mesh
        Vector2 bottom;
        Vector2 right;
        Vector2 top;
        Vector2 left;
        
        float shadowAngle;

        if (flip)
        {
            bottom = new Vector2(
                0, 
                Constants.CELL_SIZE_Y * -0.5f);
            right = new Vector2(
                Constants.CELL_SIZE_X * (cellSize.y * 0.5f), 
                Constants.CELL_SIZE_Y * (-0.5f + 0.5f * cellSize.y));
            top = new Vector2(
                Constants.CELL_SIZE_X * ((cellSize.y - cellSize.x) * 0.5f), 
                Constants.CELL_SIZE_Y * (-0.5f + (cellSize.x + cellSize.y) * 0.5f));
            left = new Vector2(
                Constants.CELL_SIZE_X * (cellSize.x * -0.5f), 
                Constants.CELL_SIZE_Y * (-0.5f + 0.5f * cellSize.x));
            shadowAngle = -SHADOW_ANGLE;
        }
        else 
        {
            bottom = new Vector2(
                0, 
                Constants.CELL_SIZE_Y * -0.5f);
            right = new Vector2(
                Constants.CELL_SIZE_X * (cellSize.x * 0.5f), 
                Constants.CELL_SIZE_Y * (-0.5f + cellSize.x * 0.5f));
            top = new Vector2(
                Constants.CELL_SIZE_X * ((cellSize.x - cellSize.y) * 0.5f), 
                Constants.CELL_SIZE_Y * (-0.5f + (cellSize.x + cellSize.y) * 0.5f));
            left = new Vector2(
                Constants.CELL_SIZE_X * (cellSize.y * -0.5f), 
                Constants.CELL_SIZE_Y * (-0.5f + cellSize.y * 0.5f));
            shadowAngle = SHADOW_ANGLE;
        }

        Vector2 center = new Vector2((left.x + right.x) / 2f, (bottom.y + top.y) / 2f);

        Vector3 bottomShadow = HelperUmpaLumpa.VectorUtils.Vector2To3XY((bottom - center) * shadowScale + center);
        Vector3 rightShadow = HelperUmpaLumpa.VectorUtils.Vector2To3XY((right - center) * shadowScale + center);
        Vector3 topShadow = HelperUmpaLumpa.VectorUtils.Vector2To3XY((top - center) * shadowScale + center);
        Vector3 leftShadow = HelperUmpaLumpa.VectorUtils.Vector2To3XY((left - center) * shadowScale + center);

        // Set collider path and utility fields
        collider_.SetPath(0, new Vector2[] {bottom, right, top, left});
        sortingNormal = (left - right).normalized;
        centerOffset = center;
        contextButtonOffset = new Vector2((left.x + right.x) / 2f, 0);
        
        // Set shadow offset and mesh
        shadowGameObject.transform.localPosition = new Vector3(shadowOffset.x * (flip ? -1f : 1f), shadowOffset.y, 0);
        if (shadowMesh)
        {
            shadowMesh.Clear();
            shadowMesh.SetVertices(new Vector3[] {bottomShadow, rightShadow, topShadow, leftShadow});
            shadowMesh.SetTriangles(new int[] {0, 2, 1, 0, 3, 2}, 0);
            shadowMesh.Optimize();
        }

        // Configure shadow material
        if (shadowMaterial)
        {
            float tileHalfSide = 0.35f * Mathf.Sqrt(Mathf.Pow(Constants.CELL_HALF_SIZE_X, 2) + Mathf.Pow(Constants.CELL_HALF_SIZE_Y, 2));
            float ellipseA = cellSize.y * tileHalfSide * shadowScale;
            float ellipseB = cellSize.x * tileHalfSide * shadowScale * (cellSize.x == cellSize.y ? 0.5f : 1f);
            Vector4 ellipseOffset = new Vector4(-center.x, -center.y);
            float dist = Mathf.Pow(ellipseA, 2) * Mathf.Pow(ellipseB, 2) * 2f;
            shadowMaterial.SetFloat("_EllipseA", ellipseA);
            shadowMaterial.SetFloat("_EllipseB", ellipseB);
            shadowMaterial.SetFloat("_EllipseAngle", shadowAngle * (cellSize.x == cellSize.y ? 0 : 1));
            shadowMaterial.SetVector("_EllipseOffset", ellipseOffset);
            shadowMaterial.SetFloat("_Dist", dist);
        }
    }

    void UpdateSpriteGeometry()
    {   
        Sprite currentSprite = flip ? spriteFlip : sprite;
        Vector3 flippedOffset;
        if (flip)
        {
            flippedOffset = new Vector3(
                spriteOffset.x + Constants.CELL_SIZE_X * 0.5f * Mathf.Abs(cellSize.x - cellSize.y), 
                spriteOffset.y, 
                0);
        }
        else
        {
            flippedOffset = new Vector3(
                spriteOffset.x, 
                spriteOffset.y, 
                0);
        }
        spriteGOLocalPosition = flippedOffset;
        spriteGameObject.transform.localPosition = spriteGOLocalPosition;
        spriteRenderer.sprite = currentSprite;

        PolygonCollider2D spriteCollider = spriteGameObject.GetComponent<PolygonCollider2D>();
        spriteCollider.TryUpdateShapeToAttachedSprite();
    }

    public Vector3Int GetCellPosition()
    {
        return ProviderUmpaLumpa.sectorManager.GetCellPosition(transform.position);
    }

    public Vector3 GetCellWorldPosition()
    {
        return ProviderUmpaLumpa.sectorManager.GetCellWorldPosition(transform.position);
    }

    public Vector3 GetCellWorldPosition(Vector3Int cellPosition)
    {
        return ProviderUmpaLumpa.sectorManager.GetCellWorldPosition(cellPosition);
    }
}
