using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Элемент для поиска соседей
/// </summary>
struct ScanPoint
{
    public Vector2Int point;
    public byte deep;

    public ScanPoint(Vector2Int pPoint, int pDeep)
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
    ,village = 7 // деревня, не занятая
    ,city = 8 // город, занятый кем-то
    ,none = 254 // пустота
}

[System.Serializable]
public class HexMapTile
{
    /// <summary>
    /// Название
    /// </summary>
    public string name;
    /// <summary>
    /// Основа
    /// </summary>
    public GameObject bottom;
    /// <summary>
    /// Список декораций
    /// </summary>
    public GameObject[] tops;
    /// <summary>
    /// Установленная декорация
    /// </summary>
    [HideInInspector]
    public GameObject top;
    /// <summary>
    /// Тип клетки
    /// </summary>
    public HexMapTileType type;
    /// <summary>
    /// Код декорации
    /// </summary>
    [HideInInspector]
    public int topId;
    /// <summary>
    /// Количество объектов в серии
    /// </summary>
    int cnt;
    /// <summary>
    /// Максимальное число объектов в серии
    /// </summary>
    public int seriesCount;

    /// <summary>
    /// Умное копирование тайла
    /// </summary>
    /// <returns></returns>
    public HexMapTile Copy ()
    {
        HexMapTile t = new HexMapTile
        {
             type = this.type
            ,seriesCount = this.seriesCount
            ,tops = this.tops
        };

        if (bottom)
            t.bottom = GameObject.Instantiate(bottom);

        if (tops != null)
        {
            if (tops.Length == 1)
            {
                if (tops[0] != null)
                    topId = 0;
            }
            else
            {
                if (seriesCount == 0)
                {
                    if (Random.Range(0, 100) < 80)
                        topId = 0;
                    else
                        topId = Random.Range(0, tops.Length);

                }
                else if ((++cnt > seriesCount && Random.Range(0, 100) > 50) || topId < 0 || topId > tops.Length)
                {
                    topId = Random.Range(0, tops.Length);
                    cnt = 0;
                }
            }
            if (tops[topId])
                t.top = GameObject.Instantiate(tops[topId]);
            t.topId = topId;
        }

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

    /// <summary>
    /// Карта
    /// </summary>
    MapTile [,] _map;

    /// <summary>
    /// Пул подсветки пути
    /// </summary>
    public GroupLayoutPool pathPool;

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

    MapTile generateCell(HexMapTileType pType, int pX, int pY)
    {
        HexMapTile tile = getTileOfType(pType).Copy();
        GameObject g = new GameObject();
        g.transform.SetParent(transform);
        g.transform.localScale = Vector3.one;
        MapTile t = g.AddComponent<MapTile>();
        t.Init(tile, new Vector2Int(pX, pY));
        return t;
    }

    void GenerateMap(HexMapTileType[,] pMap)
    {
        _map = new MapTile[pMap.GetLength(0), pMap.GetLength(1)];

        for (int i=pMap.GetLength(0)-1; i>=0; i--)
            for (int j = pMap.GetLength(1) - 1; j >= 0; j--)
                _map[i,j] = generateCell(pMap[i, j], i, j);
    }

    /// <summary>
    /// Сканирование области на предмет совпадений
    /// </summary>
    public static Vector2Int[] ScannArea(HexMapTileType[,] pMap, Vector2Int pPoint, HexMapTileType[] types, int pDeep = 254)
    {
        Stack<ScanPoint> points = new Stack<ScanPoint>();
        List<Vector2Int> result = new List<Vector2Int>();
        Vector2Int[] neighbors;
        points.Clear();
        points.Push(new ScanPoint(pPoint, 0));

        while (points.Count > 0)
        {
            ScanPoint p = points.Pop();
            neighbors = GetNeighbors(p.point);

            for (int i = neighbors.Length - 1; i >= 0; i--)
            {
                Vector2Int n = neighbors[i];
                if (!result.Contains(n) && types.Contains(pMap[n.x, n.y]))
                {
                    if (p.deep < pDeep)
                        points.Push(new ScanPoint(n, p.deep + 1));
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

    /// <summary>
    /// на указанной области расположить объекты указанного типа
    /// </summary>
    Vector2Int[] PlaceUnits(HexMapTileType[,] pMap, HexMapTileType[] types, HexMapTileType pNewType)
    {
        List<Vector2Int[]> sectors = Parce(pMap, types);
        List<Vector2Int> found = new List<Vector2Int>();

        foreach (Vector2Int[] s in sectors)
        {
            List<Vector2Int> sector = s.ToList<Vector2Int>();

            while (sector.Count > 0)
            {
                Vector2Int p = sector.GetRandom();
                found.Add(p);
                sector.Remove(p);
                Vector2Int[] neighbors = GetNeighbors(p, 3);
                for (int i = neighbors.Length - 1; i >= 0; i--)
                    sector.Remove(neighbors[i]);
            }
        }

        foreach (Vector2Int v in found)
            pMap[v.x, v.y] = pNewType;

        return found.ToArray();
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
    public static Vector2Int[] GetNeighbors(Vector2Int pPoint, int level = 1)
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

        if (level > 1)
        {
            List<Vector2Int> others = new List<Vector2Int>();
            foreach (Vector2Int v in neighbors)
                others.AddRange(GetNeighbors(v, level-1));

            foreach (Vector2Int o in others)
                neighbors.AddOnce(o);
        }

        return neighbors.ToArray();
    }

    struct StepMapTile
    {
        /// <summary>
        /// тайлик
        /// </summary>
        public MapTile tile;
        /// <summary>
        /// Занята врагом
        /// </summary>
        public bool enemy;
        /// <summary>
        /// Можно зайти
        /// </summary>
        public bool canStep;
    }

    StepMapTile getStepMapTile(Vector2Int pPoint, bool canSweam, bool canMount)
    {
        StepMapTile smt = new StepMapTile();

        smt.tile = GetTileAtPoint(pPoint);
        if (smt.tile)
        {
            if (smt.tile.UsedBy != null)
                smt.canStep = false;
            else 
                switch (smt.tile.data.type)
                {
                    case HexMapTileType.city:
                        smt.canStep = true;
                    break;
                    case HexMapTileType.forest:
                        smt.canStep = true;
                    break;
                    case HexMapTileType.ground:
                        smt.canStep = true;
                    break;
                    case HexMapTileType.mount:
                        smt.canStep = canMount;
                    break;
                    case HexMapTileType.village:
                        smt.canStep = true;
                    break;
                    case HexMapTileType.water:
                        smt.canStep = canSweam;
                    break;
                    default:
                        Debug.LogError("unsupported type");
                        smt.canStep = false;
                    break;
                }
        }
        return smt;
    }

    /// <summary>
    /// Список доступных к перемещение клеток
    /// </summary>
    /// <param name="pPoint">Начальная точка</param>
    /// <param name="pRange">Максимальная дистанция</param>
    /// <param name="canSweam">Может плавать</param>
    /// <param name="canMount">Может лазать по горам</param>
    /// <returns></returns>
    public Vector2Int[] GetPath(Vector2Int pPoint, int pRange, bool canSweam, bool canMount)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();



        return neighbors.ToArray();
    }
    #endregion position
    #region tiles
    public MapTile GetTileAtPoint(Vector2Int pPoint)
    {
        if (pPoint.x >= 0 && pPoint.y >= 0 && pPoint.x < _map.GetLength(0) && pPoint.y < _map.GetLength(1))
            return _map[pPoint.x, pPoint.y];
        return null;
    }
    #endregion tiles

    void Start()
    {
        HexMapTileType[,] map = MapGenerator.Generate(mapSize);
        PlaceUnits(map, livable, HexMapTileType.village);

        GenerateMap(map);
    }

}
