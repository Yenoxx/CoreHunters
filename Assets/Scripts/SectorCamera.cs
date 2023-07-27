using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DigitalRuby.Tween;

public class SectorCamera : MonoBehaviour
{
    const int MOUSE_BUTTON = 0;
    const float SENSITIVITY_BASE = 0.1f;

    public float zoomScale;
    public float zoomDefault;
    public float zoomMin;
    public float zoomMax;
    public float zoomFocus;

    private Vector3 positionTarget;
    private float zoomTarget;
    private Vector3 dragOrigin;
    private float sensitivity;
    private int lastTouchCount;
    public bool drag { get; private set; }
    public bool dragSuspended { get; private set; }
    public bool dragCounted { get; private set; }
    public bool locked { get; set; }

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
        lastTouchCount = 0;
        locked = false;

        clicked = () => {};

        cameraComponent.orthographicSize = zoomDefault;

        positionTarget = transform.position;
        zoomTarget = zoomDefault;
    }

    void Update()
    {
        // Smooth zooming :-)
        float osDist = (zoomTarget - cameraComponent.orthographicSize) * Time.deltaTime * 10f;
        if (osDist > 0 && cameraComponent.orthographicSize + osDist >= zoomTarget ||
            osDist < 0 && cameraComponent.orthographicSize + osDist <= zoomTarget)
        {
            cameraComponent.orthographicSize = zoomTarget;
        }
        else
        {
            cameraComponent.orthographicSize += osDist;
        }

        // Smooth moving o_O
        Vector3 position = transform.position;
        Vector3 pDist = (positionTarget - position) * Time.deltaTime * 40f;
        if (pDist.x > 0 && position.x + pDist.x >= positionTarget.x ||
            pDist.x < 0 && position.x + pDist.x <= positionTarget.x)
        {
            position.x = positionTarget.x;
        }
        else
        {
            position.x += pDist.x;
        }
        if (pDist.y > 0 && position.y + pDist.y >= positionTarget.y ||
            pDist.y < 0 && position.y + pDist.y <= positionTarget.y)
        {
            position.y = positionTarget.y;
        }
        else
        {
            position.y += pDist.y;
        }
        transform.position = position;

        // If is controllable
        if (ProviderUmpaLumpa.eventSystem != null && !locked)
        {
            // Touch controls
            if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                if (Input.touchCount >= 1)
                {
                    Touch touchA = Input.GetTouch(0);

                    // If touch isn't over UI
                    if (!ProviderUmpaLumpa.eventSystem.IsPointerOverGameObject(touchA.fingerId))
                    {
                        // Touch motion
                        if (touchA.phase == TouchPhase.Began || lastTouchCount != Input.touchCount)
                        {
                            dragOrigin = GetAverageTouchWorldPosition();
                            drag = true;
                            dragCounted = false;
                            clicked.Invoke();
                        }

                        if (touchA.phase == TouchPhase.Moved)
                        {
                            if (dragSuspended)
                            {
                                dragOrigin = GetAverageTouchWorldPosition();
                                dragSuspended = false;
                            }
                            Vector3 difference = dragOrigin - GetAverageTouchWorldPosition();
                            if (difference.magnitude >= sensitivity) dragCounted = true;
                            Move(transform.position + new Vector3(difference.x, difference.y, 0));
                        }
                        
                        // Touch zooming
                        if (Input.touchCount >= 2)
                        {
                            Touch touchB = Input.GetTouch(1);

                            if (!ProviderUmpaLumpa.eventSystem.IsPointerOverGameObject(touchB.fingerId))
                            {
                                Vector2 prevPositionA = touchA.position - touchA.deltaPosition;
                                Vector2 prevPositionB = touchB.position - touchB.deltaPosition;

                                float currMagnitude = (touchA.position - touchB.position).magnitude;
                                float prevMagnitude = (prevPositionA - prevPositionB).magnitude;

                                float difference = currMagnitude - prevMagnitude;
                                Zoom(zoomTarget - difference / Mathf.Min(Screen.height, Screen.width) * zoomScale);
                            }
                        }

                        lastTouchCount = Input.touchCount;
                    }
                    else
                    {
                        dragSuspended = true;
                    }

                    if (touchA.phase == TouchPhase.Canceled)
                    {
                        drag = false;
                    }
                }
            }

            // Mouse controls
            else
            {
                // If mouse isn't over UI
                if (!ProviderUmpaLumpa.eventSystem.IsPointerOverGameObject())
                {
                    // Motion
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
                        Move(transform.position + new Vector3(difference.x, difference.y, 0));
                    }

                    // Zooming
                    float zoomAxis = Input.GetAxis("Mouse ScrollWheel");
                    if (zoomAxis != 0) 
                    {
                        Zoom(zoomTarget - zoomAxis * zoomScale);
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
    }

    public Vector3 GetMouseWorldPosition()
    {
        return cameraComponent.ScreenToWorldPoint(Input.mousePosition);
    }

    public Vector3 GetTouchWorldPosition(Touch touch)
    {
        return cameraComponent.ScreenToWorldPoint(touch.position);
    }

    public Vector3 GetAverageTouchWorldPosition()
    {
        Vector2 averagePosition = Vector2.zero;
        for (int i = 0; i < Input.touchCount; i++)
        {
            averagePosition += Input.GetTouch(i).position;
        }
        averagePosition /= (float)Input.touchCount;
        return cameraComponent.ScreenToWorldPoint(averagePosition);
    }

    private void Move(Vector2 position)
    {
        Vector3 position3 = new Vector3(position.x, position.y, transform.position.z);
        positionTarget = position3;
    }

    private void Zoom(float size)
    {
        float clampedSize = Mathf.Clamp(size, zoomMin, zoomMax);
        zoomTarget = clampedSize;
    }

    public void FocusOn(Vector2 position, Vector2 screenOffset)
    {
        float nextZoom = zoomFocus;
        Vector2 camDif = cameraComponent.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height)) - cameraComponent.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector2 offset = screenOffset * camDif * nextZoom / cameraComponent.orthographicSize;
        Move(position + offset);
        Zoom(nextZoom);
    }
    public void FocusOn(Vector2 position)
    {
        FocusOn(position, Vector2.zero);
    }

    // Snapshot creation
    public Snapshot CreateSnapshot()
    {
        return new Snapshot(this);
    }
    public void SaveSnapshot()
    {
        lastSnapshot = CreateSnapshot();
    }

    // Snapshot smooth loading
    public void LoadSnapshot(Snapshot snapshot)
    {
        Move(snapshot.position);
        Zoom(snapshot.size);
    }
    public void LoadSnapshotPosition(Snapshot snapshot)
    {
        Move(snapshot.position);
    }
    public void LoadSnapshotZoom(Snapshot snapshot)
    {
        Zoom(snapshot.size);
    }
    
    // Snapshot instant loading
    public void LoadSnapshotInstantly(Snapshot snapshot)
    {
        transform.position = snapshot.position;
        cameraComponent.orthographicSize = snapshot.size;
    }
    public void LoadSnapshotPositionInstantly(Snapshot snapshot)
    {
        transform.position = snapshot.position;
    }
    public void LoadSnapshotZoomInstantly(Snapshot snapshot)
    {
        cameraComponent.orthographicSize = snapshot.size;
    }

    // Last snapshot smooth loading
    public void LoadSnapshot()
    {
        LoadSnapshot(lastSnapshot);
    }
    public void LoadSnapshotPosition()
    {
        LoadSnapshotPosition(lastSnapshot);
    }
    public void LoadSnapshotZoom()
    {
        LoadSnapshotZoom(lastSnapshot);
    }

    // Last snapshot instant loading
    public void LoadSnapshotInstantly()
    {
        LoadSnapshotInstantly(lastSnapshot);
    }
    public void LoadSnapshotPositionInstantly()
    {
        LoadSnapshotPositionInstantly(lastSnapshot);
    }
    public void LoadSnapshotZoomInstantly()
    {
        LoadSnapshotZoomInstantly(lastSnapshot);
    }

    // Snapshot class
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
