using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iSelector
{
    /// <summary>
    /// Выбрать
    /// </summary>
    void Select();

    /// <summary>
    /// Отменить выбор
    /// </summary>
    void DeSelect();
}
