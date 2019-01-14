using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
* Набор статов
*/
public class UnitStats 
{
	public Dictionary<StatType, BaseDataClass> stats = new Dictionary<StatType, BaseDataClass> ();
	TemplateClass template;

	public void Init(TemplateClass pTemplate)
	{
#if DEBUG_UNIT
		Debug.Log("init by template "+pTemplate.name+" ["+pTemplate.id+"]\n"+pTemplate.ToString());
#endif
		template = pTemplate;
		foreach(KeyValuePair<StatType, string> el in template.elements)
		{
			AddStat(el.Key, el.Value);
		}
	}

	public void AddStat(StatType pType, string pID)
	{
		StatsBase stat = StatsBase.Get(pType);
		if (stat != null && !string.IsNullOrEmpty(pID))
		{
			stats[pType] = stat.GetData(pID);
#if DEBUG_UNIT
			Debug.Log("init "+pType+" "+stats[pType].ToString());
#endif
		}

	}

	/*
	* Получить стату из набора
	* pStatType - тип
	*/
	public BaseDataClass GetStat (StatType pStatType) 
	{
		if (stats.ContainsKey(pStatType))
		{
			return (stats[pStatType]);
		}
		return null;
	}

}