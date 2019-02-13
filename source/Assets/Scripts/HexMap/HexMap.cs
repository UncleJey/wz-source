using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Элемент для поиска соседей
/// </summary>
struct scanPoint
{
    public Vector2Int point;
    public byte deep;

    public scanPoint(Vector2Int pPoint, int pDeep)
    {
        point = pPoint;
        deep = (byte) pDeep;
    }
}

public enum HexMapTileType : byte
{
     ground = 0 // земля
    ,mount = 1 // гора
    ,forest = 2 // лес
    ,water = 3 // вода
    ,coal = 4   // уголь
    ,oil = 5 // нефть
    ,iron = 6 // железо
    ,none = 7 // пустота
}

[System.Serializable]
public struct HexMapTile
{
    public string name;
    public GameObject bottom;
    public GameObject top;
    public HexMapTileType type;

    public HexMapTile copy ()
    {
        HexMapTile t = new HexMapTile();
        t.name = name;
        if (bottom)
            t.bottom = GameObject.Instantiate(bottom);
        if (top)
            t.top = GameObject.Instantiate(top);

        t.type = type;

        return t;
    }

}

public class HexMap : MonoBehaviour
{
    public Button test;
    public Text text;

    /// <summary>
    /// Типы тайлов
    /// </summary>
    [SerializeField]
    private HexMapTile[] tiles;

    /// <summary>
    /// Шаг сетки
    /// </summary>
    public static Vector2 scale = new Vector2(1.02f, 0.87f);

    public static int mapSize = 20;

#region Generator
    HexMapTile getTileOfType(HexMapTileType pType)
    {
        for (int i=tiles.Length-1; i>=0; i--)
        {
            if (tiles[i].type.Equals(pType))
                return tiles[i];
        }
        return tiles[0];
    }

    HexMapTile generateCell(HexMapTileType pType, int pX, int pY)
    {
        HexMapTile tile = getTileOfType(pType).copy();
        GameObject g = new GameObject();
        g.transform.SetParent(transform);
        g.transform.localScale = Vector3.one;
        MapTile t = g.AddComponent<MapTile>();
        t.Init(tile, new Vector2Int(pX, pY));
        return tile;
    }

    void GenerateMap(HexMapTileType[,] pMap)
    {
        for (int i=pMap.GetLength(0)-1; i>=0; i--)
        {
            for (int j = pMap.GetLength(1) - 1; j >= 0; j--)
            {
                generateCell(pMap[i, j], i, j);
            }
        }
    }

    /// <summary>
    /// Сканирование области на предмет совпадений
    /// </summary>
    public Vector2Int[] ScannArea(HexMapTileType[,] pMap, Vector2Int pPoint, HexMapTileType[] types, int pDeep = 254)
    {
        Stack<scanPoint> points = new Stack<scanPoint>();
        List<Vector2Int> result = new List<Vector2Int>();
        Vector2Int[] neighbors;
        points.Clear();
        points.Push(new scanPoint(pPoint, 0));

        while (points.Count > 0)
        {
            scanPoint p = points.Pop();
            neighbors = HexMap.GetNeighbors(p.point);

            for (int i = neighbors.Length - 1; i >= 0; i--)
            {
                Vector2Int n = neighbors[i];
                if (!result.Contains(n) && types.Contains(pMap[n.x, n.y]))
                {
                    if (p.deep < pDeep)
                        points.Push(new scanPoint(n, p.deep + 1));
                    result.Add(n);
                }
            }
        }

        return result.ToArray();
    }

#endregion Generator

#region position

    /// <summary>
    /// Координаты hex в миру
    /// </summary>
    public static Vector3 HexToWorld(Vector2Int pPoint)
    {
        if (pPoint.y % 2 == 0)
            return new Vector3(scale.x * pPoint.x, 0, scale.y * pPoint.y);
        else
            return new Vector3((0.5f + pPoint.x) * scale.x, 0, scale.y * pPoint.y);
    }

    /// <summary>
    /// Список соседних клекток
    /// </summary>
    public static Vector2Int[] GetNeighbors(Vector2Int pPoint)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        if (pPoint.x > 0)
            neighbors.Add(new Vector2Int(pPoint.x - 1, pPoint.y));
        if (pPoint.x + 1< mapSize)
            neighbors.Add(new Vector2Int(pPoint.x + 1, pPoint.y));
        if (pPoint.y > 0)
            neighbors.Add(new Vector2Int(pPoint.x, pPoint.y - 1));
        if (pPoint.y + 1 < mapSize)
            neighbors.Add(new Vector2Int(pPoint.x, pPoint.y + 1));

        if (pPoint.y % 2 == 0)
        {
            if (pPoint.y > 0 && pPoint.x > 0)
                neighbors.Add(new Vector2Int(pPoint.x - 1, pPoint.y - 1));
            if (pPoint.y + 1 < mapSize && pPoint.x > 0)
                neighbors.Add(new Vector2Int(pPoint.x - 1, pPoint.y + 1));
        }
        else
        {
            if (pPoint.y > 0 && pPoint.x + 1 < mapSize)
                neighbors.Add(new Vector2Int(pPoint.x + 1, pPoint.y - 1));
            if (pPoint.y + 1 < mapSize && pPoint.x + 1 < mapSize)
                neighbors.Add(new Vector2Int(pPoint.x + 1, pPoint.y + 1));
        }

        return neighbors.ToArray();
    }
#endregion position

    private void Awake()
    {
//        test.onClick.AddListener(() => { Start(); });
    }

    void Start()
    {
        HexMapTileType [,] map = MapGenerator.Generate(mapSize);
//        text.text = MapGenerator.Prnt(map);
        GenerateMap(map);
        ObjectPlacer.Parce(map);
    }

}
