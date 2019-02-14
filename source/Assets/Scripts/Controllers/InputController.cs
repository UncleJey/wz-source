using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private Game game;
    [SerializeField]
    private HexMap map;

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
        game.selector.Select(v);
    }
}
