using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private Game game;
    [SerializeField]
    private HexMap map;
    private iSelector _selected;

    private iSelector selected
    {
        get
        {
            return _selected;
        }
        set
        {
            if (_selected != null)
            {
                _selected.DeSelect();
            }
            _selected = value;
            if (_selected != null)
            {
                _selected.Select();
            }
        }
    }

    private void Awake()
    {
        CameraController.clickAtPoint += clickAtPoint;
    }

    private void OnDestroy()
    {
        CameraController.clickAtPoint -= clickAtPoint;
    }

    private void clickAtPoint(Vector3 pPoint)
    {
        Vector2Int v = HexMap.WorldToHex(pPoint);
#if DEBUG
        Debug.Log("select " + v);
#endif
        Select(v);
    }

    /// <summary>
    /// Выбрать ячейку на карте
    /// </summary>
    /// <param name="pPoint"></param>
    public void Select(Vector2Int pPoint)
    {
        UnSelect();
        MapTile t = map.GetTileAtPoint(pPoint);
        if (t)
        {
            t.Selected = true;

            iSelector s1 = t.GetComponent<iSelector>();
            iSelector s2 = t.GetComponentInChildren<iSelector>();
            if (s1 != null && s1 != _selected)
            {
                selected = s1;
            }
            else if (s2 != null)
            {
                selected = s2;
            }
            else
            {
                selected = null;
            }
        }
        //HighLightPath(pPoint, 5);
    }

    void UnSelect()
    {
        if (MapTile.SelectedTile)
            MapTile.SelectedTile.Selected = false;
        map.pathPool.Clear();
    }

    /// <summary>
    /// Подсветить путь
    /// </summary>
    /// <param name="pPoint">точка начала</param>
    /// <param name="pRange">дистанция</param>
    public void HighLightPath(Vector2Int pPoint, int pRange)
    {
        map.pathPool.Clear();
        PathFinder pf = new PathFinder(map);

        Vector2Int[] _neighbors = pf.GetPathSmart(pPoint, pRange, false, false);
        foreach (Vector2Int n in _neighbors)
        {
            if (!n.Equals(pPoint))
            {
                Transform p = map.pathPool.InstantiateElement();
                p.position = HexMap.HexToWorld(n) + new Vector3(0f, 0.2f, 0f);
            }
        }
    }
}
