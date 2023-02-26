using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DigitalRuby.Tween;

public class SectorCamera : MonoBehaviour
{
    const int MOUSE_BUTTON = 0;
    const float SENSITIVITY_BASE = 0.1f;

    public GameObject eventSystemObject;
    public float zoomScale;
    public float zoomDefault;
    public float zoomMin;
    public float zoomMax;
    public float zoomFocus;

    private Vector3 dragOrigin;
    private float sensitivity;
    public bool drag { get; private set; }
    public bool dragSuspended { get; private set; }
    public bool dragCounted { get; private set; }
    public bool locked { get; set; }

    private Action<ITween<Vector3>> motionUF;
    private Action<ITween<float>> zoomUF;
    private FloatTween zoomTween;

    public Action clicked;

    private Camera cameraComponent_;
    public Camera cameraComponent { private set => cameraComponent_ = value; get => cameraComponent_; }

    private Snapshot lastSnapshot;

    void Awake()
    {
        ProviderUmpaLumpa.sectorCamera = this;

        cameraComponent = GetComponent<Camera>();
    }

    void Start()
    {
        dragOrigin = new Vector3();
        drag = false;
        dragSuspended = false;
        dragCounted = false;
        sensitivity = SENSITIVITY_BASE;
        locked = false;

        motionUF = (t) => 
        { 
            transform.position = t.CurrentValue;
        };
        zoomUF = (t) => 
        { 
            cameraComponent.orthographicSize = t.CurrentValue;
        };

        clicked = () => {};

        cameraComponent.orthographicSize = zoomDefault;
    }

    void Update()
    {
        if (ProviderUmpaLumpa.eventSystem != null && !locked)
        {
            if (!ProviderUmpaLumpa.eventSystem.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(MOUSE_BUTTON))
                {
                    dragOrigin = GetMouseWorldPosition();
                    drag = true;
                    dragCounted = false;
                    clicked.Invoke();
                }
                
                if (Input.GetMouseButton(MOUSE_BUTTON) && drag)
                {
                    if (dragSuspended)
                    {
                        dragOrigin = GetMouseWorldPosition();
                        dragSuspended = false;
                    }
                    Vector3 difference = dragOrigin - GetMouseWorldPosition();
                    if (difference.magnitude >= sensitivity) dragCounted = true;
                    Move(cameraComponent.transform.position + new Vector3(difference.x, difference.y, 0), 0);
                }

                float zoomAxis = Input.GetAxis("Mouse ScrollWheel");
                if (zoomAxis != 0) 
                {
                    Zoom(cameraComponent.orthographicSize - zoomAxis * zoomScale);
                    clicked.Invoke();
                }
            }
            else
            {
                dragSuspended = true;
            }

            if (Input.GetMouseButtonUp(MOUSE_BUTTON))
            {
                drag = false;
            }
        }
    }

    public Vector3 GetMouseWorldPosition()
    {
        return cameraComponent.ScreenToWorldPoint(Input.mousePosition);
    }

    private void Move(Vector2 position, float time)
    {
        Vector3 position3 = new Vector3(position.x, position.y, transform.position.z);
        gameObject.Tween(
            gameObject + "_motion", 
            transform.position, 
            position3, 
            time, TweenScaleFunctions.QuadraticEaseInOut, motionUF);
    }
    private void Move(Vector2 position)
    {
        Move(position, 0.1f);
    }

    private void Zoom(float size, float time)
    {
        float clampedSize = Mathf.Clamp(size, zoomMin, zoomMax);
        if (zoomTween != null && zoomTween.State == TweenState.Running)
        {
            clampedSize = Mathf.Clamp(size + zoomTween.EndValue - zoomTween.CurrentValue, zoomMin, zoomMax);
        }
        zoomTween = gameObject.Tween(
            gameObject + "_zooming", 
            cameraComponent.orthographicSize, 
            clampedSize, 
            time, TweenScaleFunctions.QuadraticEaseInOut, zoomUF);
    }
    private void Zoom(float size)
    {
        Zoom(size, 0.1f);
    }

    public void FocusOn(Vector2 position, Vector2 screenOffset)
    {
        Vector2 offset = screenOffset * new Vector2(Screen.width, Screen.height) * Constants.UNITS_TO_PIXELS;
        Move(position + offset, 0.25f);
        Zoom(zoomFocus, 0.25f);
    }
    public void FocusOn(Vector2 position)
    {
        FocusOn(position, Vector2.zero);
    }

    public void CreateSnapshot()
    {
        lastSnapshot = new Snapshot(this);
    }

    public void LoadSnapshot(Snapshot snapshot)
    {
        Move(snapshot.position, 0.25f);
        Zoom(snapshot.size, 0.25f);
    }
    public void LoadSnapshot()
    {
        LoadSnapshot(lastSnapshot);
    }

    public void LoadSnapshotInstantly(Snapshot snapshot)
    {
        transform.position = snapshot.position;
        cameraComponent.orthographicSize = snapshot.size;
    }
    public void LoadSnapshotInstantly()
    {
        LoadSnapshotInstantly(lastSnapshot);
    }

    public class Snapshot
    {
        public Vector3 position { get; private set; }
        public float size { get; private set; }

        public Snapshot(SectorCamera sectorCamera)
        {
            position = sectorCamera.transform.position;
            size = sectorCamera.cameraComponent.orthographicSize;
        }
    }
}
