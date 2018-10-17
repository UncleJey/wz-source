using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour 
{
	public UnitStats stats = new UnitStats();

	private void Start() {
		TemplateClass template =  Templates.Get("SK-Tiger-Hover-ASCannon");
		stats.Init(template);
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
