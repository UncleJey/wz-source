using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
* Набор статов
*/
public class UnitStats 
{
	private List<BaseDataClass> _stats = new List<BaseDataClass> ();
	TemplateClass template;

	public void Init(TemplateClass pTemplate)
	{
#if DEBUG_UNIT
		Debug.Log("init by template "+pTemplate.name+" ["+pTemplate.id+"]");
#endif
		template = pTemplate;
	}

	/*
	* Добавить стату в набор
	*/
	public void AddStat (BaseDataClass pData) 
	{
		_stats.Add (pData);
	}

	/*
	* Получить стату из набора
	* pStatType - тип
	* pNum - номер (могут быть две пушки например)
	*/
	public BaseDataClass GetStat (StatType pStatType, int pNum = 0) 
	{
		int cnt = _stats.Count;
		int nr = 0;
		for (int i=0; i<= cnt; i++)
		{
			BaseDataClass clc = _stats[i];
			if (pStatType.Equals(clc.type))
			{
				if (pNum == nr)
					return clc;
				else
					nr++;
			}
		}
		return null;
	}

}