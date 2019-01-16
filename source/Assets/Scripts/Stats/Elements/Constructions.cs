using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConstructionIcons
{
#if UNITY_EDITOR
	public string name;
#endif
	public ConstructionEnums type;
	public Sprite sprite;
}

/*
* Список радаров
*/
public class Constructions : StatsBase 
{
	private static Dictionary<string, ConstructionClass> props = new Dictionary<string, ConstructionClass> ();
	private static Constructions instance;
	[SerializeField]
	private ConstructionIcons[] icons;

	public override void Awake()
	{
		instance = this;
		props.Clear ();
		type = StatType.Construction;
		bases [type] = this;

		stats = Stats.GetStat (type);	
		foreach (string s in stats.Keys) 
		{
			JsonObject stat = stats[s] as JsonObject;
			ConstructionClass prp = new ConstructionClass (stat);
			props[s] = prp;
		}
		base.Awake();
	}

	public override Sprite icon (string pName)
	{
		foreach (ConstructionIcons i in icons)
			if (i.type.ToString ().Equals (pName))
				return i.sprite;
		return null;
	}

	/// <summary>
	/// Найти экземпляр данных
	/// </summary>
	/// <param name="pClassName">P class name.</param>
	public static ConstructionClass Get(string pClassName)
	{
		if (props.ContainsKey (pClassName))
			return props [pClassName];
		return null;
	}

	/// <summary>
	/// Найти экземпляр данных для отображения
	/// </summary>
	/// <returns>The data.</returns>
	public override BaseDataClass GetData (string pClassName)
	{
		return Get (pClassName) as BaseDataClass;
	}

	/// <summary>
	/// Список доступных классов для использования
	/// </summary>
	public override List<BaseDataClass> GetList ()
	{
		List<BaseDataClass> list = new List<BaseDataClass> ();
		foreach (ConstructionClass u in props.Values)
			//TODO: ДОбавить условия
			if (u.designable)
				list.Add (u);

		return list;
	}
}
