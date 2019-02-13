using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HexMapTileType : byte
{
     ground = 0 // земля
    ,mount = 1 // гора
    ,forest = 2 // лес
    ,water = 3 // вода
    ,coal = 4   // уголь
    ,oil = 5 // нефть
    ,iron = 6 // железо
}

[System.Serializable]
public class HexMapTile
{
    public string name;
    public GameObject bottom;
    public GameObject top;
    public HexMapTileType type;
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
    public Vector2 step = Vector2.one;

    private void Awake()
    {
        test.onClick.AddListener(() => { Start(); });
    }

    void Start()
    {
        HexMapTileType [,] map = MapGenerator.Generate(20);
        text.text = MapGenerator.Prnt(map);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
