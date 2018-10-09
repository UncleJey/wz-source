using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PropIcons
{
#if UNITY_EDITOR
	public string name;
#endif
	public PropEnums type;
	public Sprite sprite;
}

public class Propulsions : StatsBase 
{
	private static Dictionary<string, PropulsionClass> props = new Dictionary<string, PropulsionClass> ();
	private static Propulsions instance;
	[SerializeField]
	private PropIcons[] icons;

	public override void Awake()
	{
		instance = this;
		props.Clear ();
		type = StatType.Propulsion;
		bases [type] = this;

		stats = Stats.GetStat (StatType.Propulsion);	
		foreach (string s in stats.Keys) 
		{
			JsonObject stat = stats[s] as JsonObject;
			PropulsionClass prp = new PropulsionClass (stat);
			props[s] = prp;
		}
		base.Awake();
	}

	public override Sprite icon (string pName)
	{
		foreach (PropIcons i in icons)
			if (i.type.ToString ().Equals (pName))
				return i.sprite;
		return null;
	}

	/// <summary>
	/// Найти экземпляр данных
	/// </summary>
	public static PropulsionClass Get(string pClassName)
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
		foreach (PropulsionClass u in props.Values) 
		{
			//TODO: ДОбавить условия
			if (u.designable)
				list.Add (u);
		}
		return list;
	}
}
