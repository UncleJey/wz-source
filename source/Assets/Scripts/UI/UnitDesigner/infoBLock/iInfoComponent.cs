using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iInfoComponent 
{

    /// <summary>
    /// Отобразить
    /// </summary>
    void Show(TemplateClass pTemplate);

    /// <summary>
    /// Скрыть
    /// </summary>
    void Hide();
}
