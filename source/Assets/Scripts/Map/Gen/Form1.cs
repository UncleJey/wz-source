using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public struct kletka
{
    public int t_jizni; // время жизни
    public int sost; // состояние M, G, Z
    public int status; // 0 - клетки нет, 1 - клетка есть
}

public class Form1 : MonoBehaviour
{
    int n = 30; // размер массива
    int m = 60; // размер цикла клеток
    kletka[,] mas;

    int sum = 0; // переменная для определение свободного места для деления
    int[,] masiv = new int[4, 2]; // массив для записи свободных позиций для деления клетки
    int[] masiv2 = new int[4]; // массив для определения свободных позиций для деления клетки
    int vibor = 0; // переменная для выбора деления

    public Text text;
    public Button test;

    private void Awake()
    {
        test.onClick.AddListener(() => { Start(); });
    }

    void Start()
    {
        mas = new kletka[n, n];
        button1_Click();
        text.text = Print();
    }

    void button1_Click() // метод деления клеток
    {
        double temp;
        int x, y;

        for (int p = 0; p < 5; p++)
        {
            x = Random.Range(0, n);
            y = Random.Range(0, n);
            mas[x, y].status = 1;
            mas[x, y].sost = 1;
            mas[x, y].t_jizni = 0;
        }

        for (int p = 0; p < m; p++)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (mas[i, j].status == 1)
                    {
                        if (mas[i, j].sost == 1)
                        {
                            mas[i, j].t_jizni++;
                            if (mas[i, j].t_jizni >= 2) mas[i, j].sost = 2;
                        }
                        else if (mas[i, j].sost == 2)
                        {
                            Delenie(i, j); // вызов метода деления клетки
                            mas[i, j].t_jizni++;
                            if (mas[i, j].t_jizni >= 6) mas[i, j].sost = 3;
                        }
                        else if (mas[i, j].sost == 3)
                        {
                            mas[i, j].t_jizni++;
                            if (mas[i, j].t_jizni > 12)
                            {
                                mas[i, j].t_jizni = 0;
                                mas[i, j].sost = 0;
                                mas[i, j].status = 0;
                            }
                            else if (mas[i, j].t_jizni > 9)
                            {
                                temp = Random.value;
                                if (temp < 0.8f)
                                {
                                    mas[i, j].t_jizni = 0;
                                    mas[i, j].sost = 0;
                                    mas[i, j].status = 0;
                                }

                            }
                        }
                    }
                }
            }
        }
    }

    public string Print()
    {
        string exColor = "";
        string res = "";

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
                int p = 0;
                if (mas[i, j].status == 1)
                {
                    if (mas[i, j].sost == 1)
                    {
                        addPoint("red");
                    }
                    else if (mas[i, j].sost == 2)
                    {
                        addPoint("green");
                    }
                    else if (mas[i, j].sost == 3)
                    {
                        addPoint("blue");
                    }
                    else
                    {
                        addPoint("silver");
                    }
                }
                else
                {
                    addPoint("silver");
                }
            }
            addPoint("");
            res += "\n";
        }
        return res;
    }

    private void Delenie(int i, int j)
    {
        sum = 0; // для суммы свободных ячеек для размножения клеток
        int k1;
        int sum2 = 0; // переменная для выбора деления клетки

        for (k1 = 0; k1 < 4; k1++) masiv2[k1] = 0;

        if (i - 1 >= 0) if (mas[i - 1, j].status == 0) { sum++; masiv2[0] = 1; masiv[0, 0] = i - 1; masiv[0, 1] = j; } // сверху
        if (i + 1 < n) if (mas[i + 1, j].status == 0) { sum++; masiv2[1] = 1; masiv[1, 0] = i + 1; masiv[1, 1] = j; } // снизу
        if (j - 1 >= 0) if (mas[i, j - 1].status == 0) { sum++; masiv2[2] = 1; masiv[2, 0] = i; masiv[2, 1] = j - 1; } // слева
        if (j + 1 < n) if (mas[i, j + 1].status == 0) { sum++; masiv2[3] = 1; masiv[3, 0] = i; masiv[3, 1] = j + 1; } // справа

        if (sum > 0) // если есть свободное место для деления клетки
        {
            vibor = Random.Range(1, sum + 1); // получение случайного местя для деления
            for (k1 = 0; k1 < 4; k1++)
            {
                if (masiv2[k1] == 1)
                    sum2++;
                if (sum2 == vibor) // условие для деления клетки по случайному числу vibor
                {
                    mas[masiv[k1, 0], masiv[k1, 1]].status = 1;
                    mas[masiv[k1, 0], masiv[k1, 1]].t_jizni = 0;
                    mas[masiv[k1, 0], masiv[k1, 1]].sost = 1;

                    mas[i, j].status = 1;
                    mas[i, j].t_jizni = 0;
                    mas[i, j].sost = 1;

                }
            }
        }

    }
}
