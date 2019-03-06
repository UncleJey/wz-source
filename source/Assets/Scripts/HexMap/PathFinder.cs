using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
    /// <summary>
    /// Стоимость прогулки по клетке
    /// </summary>
    public int troughtPrice;
    /// <summary>
    /// Координаты точки
    /// </summary>
    public Vector2Int point;
    /// <summary>
    /// Может пройти свободно (false - остановится)
    /// </summary>
    public bool canTrought;
}

/// <summary>
/// Класс поиска пути
/// </summary>
public class PathFinder 
{
    public HexMap hexMap;
    public bool canSweam, canMount;
    List<Vector2Int> neighbors = new List<Vector2Int>();

    public  PathFinder(HexMap pMap)
    {
        hexMap = pMap;
    }

    StepMapTile getStepMapTile(Vector2Int pPoint)
    {
        StepMapTile smt = new StepMapTile();
        smt.troughtPrice = 1; // TODO: заполнить
        smt.tile = hexMap.GetTileAtPoint(pPoint);
        smt.point = pPoint;
        smt.enemy = false;

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
        smt.canTrought = smt.canStep; //TODO: логику добавить
        return smt;
    }

    /// <summary>
    /// Список доступных к перемещение клеток
    /// </summary>
    /// <param name="pPoint">Начальная точка</param>
    /// <param name="pRange">Максимальная дистанция</param>
    /// <param name="canSweam">Может плавать</param>
    /// <param name="canMount">Может лазать по горам</param>
    public Vector2Int[] GetPathSmart(Vector2Int pPoint, int pRange, bool pCanSweam, bool pCanMount)
    {
        canSweam = pCanSweam;
        canMount = pCanMount;
        neighbors.Clear();
        GetPath(pPoint, pRange, "");
        return neighbors.ToArray();
    }

    private void GetPath(Vector2Int pPoint, int pRange, string stepped)
    {
        neighbors.AddOnce(pPoint);
        List<StepMapTile> tiles = new List<StepMapTile>();

        Vector2Int [] region = HexMap.GetNeighbors(pPoint, 1);
        bool haveEnemies = false;

        foreach (Vector2Int r in region)
        {
            StepMapTile smt = getStepMapTile(r);
            if (smt.enemy) 
            {
                haveEnemies = true;
            }
            tiles.Add(smt);
        }

        if (pRange >= 1) 
        {
            foreach (StepMapTile t in tiles) 
            {
                if (t.canStep && stepped.IndexOf(t.point.ToString()) == -1)
                {
                    if (!haveEnemies && pRange - t.troughtPrice > 0 && t.canTrought) // если может пройти через клетку, то смотрим - куда
                    {
                        GetPath(t.point, pRange - t.troughtPrice, stepped + t.point.ToString());
                    }
                }
            }
        }
    }

}