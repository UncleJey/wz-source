using System.Collections;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class GraphStatsBlock : MonoBehaviour, iInfoComponent
{

    [SerializeField]
    private UIPolygon polygon;


    /// <summary>
    /// Скрыть
    /// </summary>
    public void Hide()
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// Отобразить
    /// </summary>
    public void Show(TemplateClass pTemplate)
    {
        gameObject.SetActive(true);
        StartCoroutine(showPolygon());
    }

    /// <summary>
    /// показать полигон свойств
    /// </summary>
    IEnumerator showPolygon()
    {
        // по некой причине не отображается если сразу всё делать
        polygon.gameObject.SetActive(true);
        yield return null;
        polygon.DrawPolygon(7);
        yield return null;
        polygon.VerticesDistances[1] = 0.5f;
        polygon.VerticesDistances[2] = 0.75f;
        polygon.VerticesDistances[3] = 0.25f;
        yield return null;
        polygon.SetDirty();
    }
}
