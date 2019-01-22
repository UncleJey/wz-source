using UnityEngine;

public interface iMapObject
{

    /// <summary>
    /// Точка на экране в которой находится юнит
    /// </summary>
    Vector2 screenPoint();

    /// <summary>
    /// Находится внутри области экрана
    /// </summary>
    bool isInRect(Rect pRect);
}
