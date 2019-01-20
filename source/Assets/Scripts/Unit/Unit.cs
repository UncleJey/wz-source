﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour 
{
	public UnitStats stats = new UnitStats();
	public UnitModel model = new UnitModel();
	public string templateName;

    public Vector2Int position {
        get
        {
            return TheMap.WorldToMap(transform.position);
        }
        set
        {
            transform.position = TheMap.MapToWorld(value.x, value.y);
        }
    }

	private void Start() 
	{
		TemplateClass template = Templates.Get(templateName);
		stats.Init(template);
		model.Init(gameObject, stats);
        //position = new Vector2Int(5, 10);
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

    /*
	void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        if (model != null && model.connectors != null)
        {
            for (int i= model.connectors.Length - 1; i>=0; i--)
            {
                Gizmos.DrawCube(transform.localPosition + model.connectors[i] , new Vector3(1, 1, 1));
            }
        }
    }
    */
}
