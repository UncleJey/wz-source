using UnityEngine;

public struct kletka
{
    public byte lifeTime; // время жизни
    public HexMapTileType state;
}

public static class MapGenerator
{
    static int[,] masiv = new int[4, 2]; // массив для записи свободных позиций для деления клетки
    static int vibor = 0; // переменная для выбора деления
    static kletka[,] mas;

    public static HexMapTileType[,] Generate(int n) // метод деления клеток
    {
        mas = new kletka[n, n];

        for (int p = 0; p < 5; p++)
            initPoint(Random.Range(0, n), Random.Range(0, n));

        for (int p = n << 1; p>=0; p--)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    switch (mas[i, j].state)
                    {
                        case HexMapTileType.mount:
                            mas[i, j].lifeTime++;
                            if (mas[i, j].lifeTime >= 2)
                                mas[i, j].state = HexMapTileType.forest;
                        break;
                        case HexMapTileType.forest:
                            Divide(i, j); 
                            mas[i, j].lifeTime++;
                            if (mas[i, j].lifeTime >= 6)
                                mas[i, j].state = HexMapTileType.water;
                        break;
                        case HexMapTileType.water:
                            mas[i, j].lifeTime++;
                            if (mas[i, j].lifeTime > 12)
                                diePoint(i, j);
                            else if (mas[i, j].lifeTime > 9 && Random.value < 0.8f)
                                diePoint(i, j);
                        break;
                        default:
                        break;
                    }
                }
            }
        }

        HexMapTileType[,] res = new HexMapTileType[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                res[i, j] = mas[i, j].state;
        }
        mas = null;
        return res;
    }

    /// <summary>
    /// Клетка погибла
    /// </summary>
    static void diePoint(int i, int j)
    {
        mas[i, j].lifeTime = 0;
        mas[i, j].state = HexMapTileType.ground;
    }

    /// <summary>
    /// Инициализация точки
    /// </summary>
    static void initPoint(int x, int y)
    {
        mas[x, y].state = HexMapTileType.mount;
        mas[x, y].lifeTime = 0;
    }


    /// <summary>
    /// Деление
    /// </summary>
    private static void Divide(int i, int j)
    {
        int sum = 0; 
        int n = mas.GetLength(0);

        int addPoint(int ii, int jj)
        {
            if (mas[ii, jj].state == HexMapTileType.ground)
            {
                masiv[sum, 0] = ii; masiv[sum, 1] = jj;
                return 1;
            }
            return 0;
        }

        if (i - 1 >= 0) // сверху
            sum += addPoint(i - 1, j); 
        if (i + 1 < n) // снизу
            sum += addPoint(i + 1, j);
        if (j - 1 >= 0) // слева
            sum += addPoint(i, j - 1);
        if (j + 1 < n) // справа
            sum += addPoint(i, j + 1);

        if (sum > 0) // если есть свободное место для деления клетки
        {
            int k1 = Random.Range(0, sum); // получение случайного местя для деления
            initPoint(masiv[k1, 0], masiv[k1, 1]);
            initPoint(i, j);
        }
    }

    public static string Prnt(HexMapTileType[,] map)
    {
        string exColor = "";
        string res = "";
        int n = map.GetLength(0);

        void addPoint(string pColor)
        {
            if (pColor != exColor)
            {
                if (!string.IsNullOrEmpty(exColor))
                {
                    res += "</color>";
                }
                if (!string.IsNullOrEmpty(pColor))
                {
                    res += "<color=" + pColor + ">";
                }
            }
            if (!string.IsNullOrEmpty(pColor))
            {
                res += "#";
            }
            exColor = pColor;
        }

        for (int i = 0; i < n; i++)
        { 
            for (int j = 0; j < n; j++)
            {
                switch (map[i,j])
                {
                    case HexMapTileType.mount:
                        addPoint("red");
                    break;
                    case HexMapTileType.water:
                        addPoint("blue");
                    break;
                    case HexMapTileType.ground:
                        addPoint("silver");
                    break;
                    case HexMapTileType.forest:
                        addPoint("green");
                    break;
                    default:
                        addPoint("yellow");
                    break;
                }
            }
            addPoint("");
            res += "\n";
        }
        return res;
    }
}
