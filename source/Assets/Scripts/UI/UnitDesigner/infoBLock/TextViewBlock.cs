using UnityEngine;
using UnityEngine.UI;

public class TextViewBlock : MonoBehaviour, iInfoComponent
{
    /// <summary>
	/// Сводная информация
	/// </summary>
	[SerializeField]
    private Text infoText;

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
        if (pTemplate != null)
        {
            infoText.text = pTemplate.ToString();
        }
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

}


