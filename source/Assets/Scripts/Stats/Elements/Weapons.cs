using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponIcons
{
#if UNITY_EDITOR
	public string name;
#endif
	public WeaponEnums type;
	public Sprite sprite;
}

public class Weapons : StatsBase 
{
	private static Dictionary<string, WeaponClass> props = new Dictionary<string, WeaponClass> ();
	private static Weapons instance;
	[SerializeField]
	private WeaponIcons[] icons;

	public override void Awake()
	{
		instance = this;
		props.Clear ();
		type = StatType.Wpn;
		bases [type] = this;

		stats = Stats.GetStat (StatType.Wpn);	
		foreach (string s in stats.Keys) 
		{
			JsonObject stat = stats[s] as JsonObject;
			WeaponClass prp = new WeaponClass (stat);
			props[s] =prp;
		}
		base.Awake();
	}

	public override Sprite icon (string pName)
	{
		foreach (WeaponIcons i in icons)
			if (i.type.ToString ().Equals (pName))
				return i.sprite;
		return null;
	}

	/// <summary>
	/// Найти экземпляр данных
	/// </summary>
	/// <param name="pClassName">P class name.</param>
	public static WeaponClass Get(string pClassName)
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
		foreach (WeaponClass u in props.Values) 
		{
			//TODO: ДОбавить условия
			//UNDONE: ДОбавить условия доступности
			if (u.designable)
				list.Add (u);
		}
		return list;
	}
}
