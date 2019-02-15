using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static System.Action<Vector3> clickAtPoint;

    bool isMoving = false;
    private static readonly Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    Vector3 currentPos, newPos;
    public float scale = 0.3f;
    public new Camera camera;
    Vector3 startpos, endpos;
    [SerializeField]
    Vector3 deltaVector = new Vector3(0.35f, 0, 0.2f);

#region zoom
    public float zoom
    {
        get
        {
            if (camera.orthographic)
                return camera.orthographicSize;
            else
                return camera.fieldOfView;
        }
        private set
        {
            if (camera.orthographic)
                camera.orthographicSize = value;
            else
                camera.fieldOfView = value;
        }
    }

    public float ZoomSpeed = 20f;

 #endregion zoom

    void Update()
    {
        // if GUIManager.Instance.AnyWindowsOpened())
        if (Input.GetMouseButtonDown(0))
        {
            currentPos = GetMousePoint();
            isMoving = true;
            startpos = camera.MousePixels();
        }

        if (!Input.GetMouseButton(0))
        {
            if (isMoving)
            {
                endpos = camera.MousePixels();
                float delta = (endpos - startpos).sqrMagnitude;
                if (delta < Screen.dpi)
                    MakeClickEvent();
            }
            isMoving = false;
        }

        if (isMoving)
        {
            newPos = GetMousePoint();
            transform.localPosition += currentPos - newPos;
            currentPos = newPos;
        }

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0f)
            zoom -= axis * ZoomSpeed;
    }

    private void MakeClickEvent()
    {
        Vector3 p = GetGroundPoint();
#if DEBUG
        Debug.Log("click at " + p);
#endif
        clickAtPoint.Execute(p);
    }

    private Vector3 GetGroundPoint()
    {
        float dist;
        Ray ray = camera.MouseRay();
        groundPlane.Raycast(ray, out dist);
        return ray.GetPoint(dist) + deltaVector;// new Vector3(0.4f,0,0.2f);
    }

    private Vector3 GetMousePoint()
    {
        Vector3 p = camera.MouseViewport() * zoom * scale;
        return new Vector3(p.x + p.y, 0, p.y - p.x);
    }

}
