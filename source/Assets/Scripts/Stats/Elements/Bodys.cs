using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class BodyIcons
{
#if UNITY_EDITOR
	public string name;
#endif
	public BodyEnums type;
	public Sprite sprite;
}

public class Bodys : StatsBase 
{
	private static Dictionary<string, BodyClass> props = new Dictionary<string, BodyClass> ();
	private static Bodys instance;
	[SerializeField]
	private BodyIcons[] icons;

	public override void Awake()
	{
		instance = this;
		type = StatType.Body;
		bases [type] = this;

		stats = Stats.GetStat (type);	
		foreach (string s in stats.Keys) 
		{
			JsonObject stat = stats[s] as JsonObject;
			BodyClass prp = new BodyClass (stat);
			props[s] = prp;
		}
		base.Awake();
	}

	/// <summary>
	/// Найти нужную иконку
	/// </summary>
	/// <param name="pName">P name.</param>
	public override Sprite icon (string pName)
	{
		foreach (BodyIcons i in icons)
			if (i.type.ToString ().Equals (pName))
				return i.sprite;
		return null;
	}

	/// <summary>
	/// Найти экземпляр данных
	/// </summary>
	/// <param name="pClassName">P class name.</param>
	public static BodyClass Get(string pClassName)
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
		foreach (BodyClass u in props.Values)
			//TODO: ДОбавить условия
			if (u.designable)
				list.Add (u);

		return list;
	}

}
