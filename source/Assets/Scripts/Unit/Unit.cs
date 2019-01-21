using UnityEngine;

public class Unit : MonoBehaviour 
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
   
}
