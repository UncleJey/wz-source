using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Базовый класс списков
*/
public abstract class StatsBase : MonoBehaviour {
	//public string name;
	public string model;
	public StatType type;
	protected JsonObject stats;

	protected static Dictionary<StatType, StatsBase> bases = new Dictionary<StatType, StatsBase> ();

	public static StatsBase Get (StatType pType) 
	{
		StatType typ = pType;
		if (pType == StatType.Wpn1 || pType == StatType.Wpn2 || pType == StatType.Wpn3 || pType == StatType.Wpn4)
		{
			typ = StatType.Wpn;
		}

		if (bases.ContainsKey(typ))
			return bases[typ];
		else
		{
			Debug.LogError("have no stat "+typ);
			return null;
		}
	}

	public virtual void Awake () {
#if DEBUG_STATS
		Debug.Log ("loaded " + gameObject.name + ": " + (stats == null ? "null" : stats.Count.ToString ()));
#endif
		// чистим память от данных инициализации
		stats.Clear ();
		stats = null;
	}

	public abstract Sprite icon (string pName);

	public abstract BaseDataClass GetData (string pClassName);

	public abstract List<BaseDataClass> GetList ();
}