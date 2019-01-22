using UnityEngine;

public class Unit : MonoBehaviour, iMapObject 
{
	public UnitStats stats = new UnitStats();
	public UnitModel model = new UnitModel();
	public string templateName;

	private void Start() 
	{
		TemplateClass template = Templates.Get(templateName);
		stats.Init(template);
		model.Init(gameObject, stats);
	}

	/*
	* Инициализация данных
	*/
	public void InitData(JsonObject pData)
	{
		foreach (string k in pData.Keys)
		{
			Debug.Log(k);
		}
	}

 #region iMapObject
    /// <summary>
    /// Точка на экране в которой находится юнит
    /// </summary>
    public Vector2 screenPoint()
    {
        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
        camPos.y = Screen.height - camPos.y;
        return camPos;
    }

    /// <summary>
    /// Находится внутри области экрана
    /// </summary>
    public bool isInRect(Rect pRect)
    {
        return pRect.Contains(screenPoint());
    }
 #endregion iMapObject
}
