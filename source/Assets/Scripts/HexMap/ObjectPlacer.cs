using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPlacer
{
    static HexMapTileType[,] map;
    static Vector2Int [,] arrays;

    public static void Parce(HexMapTileType[,] pMap)
    {
        map = new HexMapTileType[pMap.GetLength(0), pMap.GetLength(1)];
        System.Array.Copy(pMap, map, 0);


    }


}
