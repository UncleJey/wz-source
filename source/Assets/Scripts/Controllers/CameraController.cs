using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    bool isMoving = false;
    private static readonly Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    Vector3 currentPos, newPos;
    public Vector2 scale = new Vector2 (3, 4);
    public Camera camera;
    Vector3 startpos, endpos;

    public System.Action<Vector3> clickAtPoint;

    private void Start()
    {
        Debug.Log(Screen.width / Screen.height);
    }

    void Update()
    {
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
                float delta = (endpos - startpos).magnitude;
                if (delta < Screen.dpi)
                {
                    Debug.Log(delta);
                    makeClickEvent();
                }
            }
            isMoving = false;
        }

        if (isMoving)
        {
            newPos = GetMousePoint();
            transform.localPosition += currentPos - newPos;
            currentPos = newPos;
        }
    }

    private void makeClickEvent()
    {
        Vector3 p = GetGroundPoint();
        Debug.Log("click at " + p);
        clickAtPoint.Execute(p);
    }

    private Vector3 GetGroundPoint()
    {
        float dist;
        Ray ray = camera.MouseRay();
        groundPlane.Raycast(ray, out dist);
        return ray.GetPoint(dist) + new Vector3(0.4f,0,0.2f);
    }

    private Vector3 GetMousePoint()
    {
        Vector3 p = camera.MouseViewport();
        p.x *= scale.x;
        p.y *= scale.y;
        return new Vector3(p.x + p.y, 0, p.y - p.x);
    }

}
