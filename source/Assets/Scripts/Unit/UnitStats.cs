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
		Debug.Log("init by template "+pTemplate.name+" ["+pTemplate.id+"]\n"+pTemplate.ToString());
#endif
		template = pTemplate;
		AddStat(StatType.Body, template.body);
		AddStat(StatType.Propulsion, template.propulsion);
		AddStat(StatType.Repair, template.repair);
		// AddStat(StatType. template.construct))  TODO: добавить
		// if (!string.IsNullOrEmpty(template.brain))
		AddStat(StatType.Sys, template.sensor);
		for (int i=0; i<4; i++)
			AddStat(StatType.Wpn, template.GetWeapon(i));
	}

	public void AddStat(StatType pType, string pID)
	{
		StatsBase stat = StatsBase.Get(pType);
		if (stat != null && !string.IsNullOrEmpty(pID))
		{
			BaseDataClass d = AddStat(stat.GetData(pID));
#if DEBUG_UNIT
			Debug.Log("init "+pType+" "+d.ToString());
#endif
		}

	}

	/*
	* Добавить стату в набор
	*/
	public BaseDataClass AddStat (BaseDataClass pData) 
	{
		_stats.Add (pData);
		return pData;
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