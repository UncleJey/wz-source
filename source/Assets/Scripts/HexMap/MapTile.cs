using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public HexMapTile data;
    Vector2Int _position;
    public Vector2Int position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
            transform.localPosition = HexMap.HexToWorld(value);
        }
    }

    public void Init(HexMapTile pData, Vector2Int pPosition)
    {
        data = pData;
        position = pPosition;
        name =  pPosition.ToString() + "_" + pData.name;
        if (data.bottom)
        {
            data.bottom.transform.SetParent(transform);
            data.bottom.transform.localScale = Vector3.one;
            data.bottom.transform.localEulerAngles = Vector3.zero;
            data.bottom.transform.localPosition = Vector3.zero;
        }
        if (data.top)
        {
            data.top.transform.SetParent(transform);
            data.top.transform.localScale = Vector3.one;
            data.top.transform.localEulerAngles = Vector3.zero;
            data.top.transform.localPosition = Vector3.zero;
        }

    }
}
