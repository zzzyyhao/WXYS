using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public Transform backgroundTransform; // 拖拽背景GameObject
    public SpriteRenderer backgroundSprite; // 拖拽背景的SpriteRenderer组件

    private Camera cam;
    private float initialOrthoSize;
    private float minOrthoSize;
    private float maxOrthoSize;
    private Vector3 dragOrigin;
    private bool isDragging = false;

    void Start()
    {
        cam = Camera.main;
        float bgHeight = backgroundSprite.bounds.size.y;
        initialOrthoSize = bgHeight / 2f;
        cam.orthographicSize = initialOrthoSize;
        minOrthoSize = initialOrthoSize / 1.5f;
        maxOrthoSize = initialOrthoSize;

        // 初始位置
        float bgLeft = backgroundSprite.bounds.min.x;
        float bgCenterY = backgroundSprite.bounds.center.y;
        cam.transform.position = new Vector3(bgLeft + cam.orthographicSize * cam.aspect, bgCenterY, cam.transform.position.z);
    }

    void Update()
    {
        HandleMove();
        HandleZoom();
    }

    void HandleMove()
    {
        // PC端鼠标拖拽
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }
        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 diff = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            MoveCamera(diff);
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // 移动端单指拖拽
        if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 diff = cam.ScreenToWorldPoint(touch.position - touch.deltaPosition) - cam.ScreenToWorldPoint(touch.position);
                MoveCamera(diff);
            }
        }
    }

    void HandleZoom()
    {
        // PC端滚轮
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            ZoomCamera(-scroll * initialOrthoSize * 0.2f);
        }

        // 移动端双指缩放
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);
            Vector2 prevDist = (t0.position - t0.deltaPosition) - (t1.position - t1.deltaPosition);
            Vector2 currDist = t0.position - t1.position;
            float delta = currDist.magnitude - prevDist.magnitude;
            ZoomCamera(-delta * 0.01f);
        }
    }

    void MoveCamera(Vector3 delta)
    {
        Vector3 newPos = cam.transform.position + delta;
        cam.transform.position = ClampCameraPosition(newPos);
    }

    void ZoomCamera(float delta)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + delta, minOrthoSize, maxOrthoSize);
        cam.transform.position = ClampCameraPosition(cam.transform.position);
    }

    Vector3 ClampCameraPosition(Vector3 pos)
    {
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        float minX = backgroundSprite.bounds.min.x + horzExtent;
        float maxX = backgroundSprite.bounds.max.x - horzExtent;
        float minY = backgroundSprite.bounds.min.y + vertExtent;
        float maxY = backgroundSprite.bounds.max.y - vertExtent;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        return pos;
    }
} 