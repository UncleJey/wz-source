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
    public HexMapTileType[] livable;

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
    public static Vector2Int[] ScannArea(HexMapTileType[,] pMap, Vector2Int pPoint, HexMapTileType[] types, int pDeep = 254)
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

    /// <summary>
    /// Парсинг карты и обнаружение жилых площадей
    /// возвращает список массивов площадей
    /// </summary>
    public static List<Vector2Int[]> Parce(HexMapTileType[,] pMap, HexMapTileType[] types, int minCount = 4)
    {
        List<Vector2Int[]> areas = new List<Vector2Int[]>();

        // Сначала собираем все жилые точки
        List<Vector2Int> lst = new List<Vector2Int>();
        for (int i = pMap.GetLength(0) - 1; i >= 0; i--)
            for (int j = pMap.GetLength(1) - 1; j >= 0; j--)
                if (types.Contains(pMap[i, j]))
                    lst.Add(new Vector2Int(i, j));

        // пока есть неисследованная область, берём первую точку и вычисляем всю её область
        // затем исключаем эту область из списка и повторяем для оставшегося
        while (lst.Count > 0)
        {
            Vector2Int startPoint = lst[0];
            lst.RemoveAt(0);
            Vector2Int[] area = ScannArea(pMap, startPoint, types);
            for (int i = area.Length - 1; i >= 0; i--)
                lst.Remove(area[i]);

            // если минимальный размер области соответствует то добавляем её в список
            if (area.Length >= minCount)
                areas.Add(area);
        }
        return areas;
    }

    #endregion Generator

    #region position
    /// <summary>
    /// Преобразование координат hex в координаты мира
    /// </summary>
    public static Vector3 HexToWorld(Vector2Int pPoint)
    {
        if (pPoint.y % 2 == 0)
            return new Vector3(scale.x * pPoint.x, 0, scale.y * pPoint.y);
        else
            return new Vector3((0.5f + pPoint.x) * scale.x, 0, scale.y * pPoint.y);
    }

    /// <summary>
    /// Преобразование координат мира в координаты hex
    /// </summary>
    public static Vector2Int WorldToHex(Vector3 pPoint)
    {
        int y = (int)(pPoint.z / scale.y);
        if (y % 2 == 0)
            return new Vector2Int((int)(pPoint.x / scale.x), y);
        else
            return new Vector2Int((int)((pPoint.x - 0.5f) / scale.x), y);
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
        Parce(map, livable);
    }

}
